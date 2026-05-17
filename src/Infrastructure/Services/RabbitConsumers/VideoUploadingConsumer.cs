using Microsoft.Extensions.DependencyInjection;
using Domain.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Common.Interfaces.Services;
using RabbitMQ.Client.Events;
using Domain.Common.Interfaces.Db;
using Domain.Rows.Contents;
using System.Text.Json;
using Domain.Entities;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.Services.RabbitConsumers;

public class VideoUploadingConsumer : IConsumer
{
    private readonly IServiceScopeFactory scopeFactory;

    private readonly IFfmpegService ffmpegService;
    private readonly IR2Service r2Service;
    private IChannel? channel;

    public VideoUploadingConsumer(IServiceScopeFactory scopeFactory, IFfmpegService ffmpegService, IR2Service r2Service)
    {
        this.ffmpegService = ffmpegService;
        this.scopeFactory = scopeFactory;
        this.r2Service = r2Service;
    }

    public async Task Consume(IConnection connection, CancellationToken token)
    {
        channel = await connection.CreateChannelAsync(cancellationToken: token);

        await channel.QueueDeclareAsync(
            queue: "video.upload",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: token);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());

                VideoUploadMessage? message = JsonSerializer.Deserialize<VideoUploadMessage>(body);

                if (message == null)
                {
                    Console.WriteLine("Video.Upload: message is null");

                    await channel.BasicNackAsync(
                        ea.DeliveryTag, false, false);

                    return;
                }

                using var scope = scopeFactory.CreateScope();

                var context = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

                Content? content = await context.Contents
                    .Include(c => c.VideoMeta)
                    .FirstOrDefaultAsync(c => c.Id == message.ContentId);

                if (content == null)
                {
                    Console.WriteLine("Video.Upload: content is null");

                    await channel.BasicNackAsync(
                        ea.DeliveryTag, false, false);

                    return;
                }

                if (message.PhotoPath != null)
                {
                    Console.WriteLine("Video.Upload: get photo from video and average color");

                    string photoUrl = r2Service.SaveImage(message.PhotoPath);

                    content.VideoMeta.PhotoUrl = photoUrl;

                    await content.VideoMeta.SetAverageColor(photoUrl, token);

                    File.Delete(message.PhotoPath);
                }

                if (message.VideoPath != null)
                {
                    Console.WriteLine("Video.Upload: get video duration with ffmpeg");

                    double duration = await ffmpegService.GetVideoDuration(message.VideoPath, token);

                    content.VideoMeta.DurationSeconds = (int)duration;
                    double timeSeconds = Math.Clamp(20, 0, duration / 1.1);

                    if (message.PhotoPath == null)
                    {
                        Console.WriteLine("Video.Upload: get photo from video and average color");

                        string photoUrl = await ffmpegService.GetPhotoFromVideo(
                            message.VideoPath, timeSeconds: timeSeconds, token: token);

                        content.VideoMeta.PhotoUrl = photoUrl;

                        await content.VideoMeta.SetAverageColor(photoUrl, token);
                    }

                    Console.WriteLine("Video.Upload: uploading video with ffmpeg");

                    content.VideoMeta.VideoUrl = await ffmpegService.UploadGeneratedVideos(message.VideoPath, token);

                    File.Delete(message.VideoPath);
                }
                
                Console.WriteLine("Video.Upload: save changes and create index");

                await scope.ServiceProvider.GetRequiredService<IContentRepository>()
                   .CreateSearchIndex(content, content.VideoMeta, token);

                await context.SaveChangesAsync();

                Console.WriteLine("Video.Upload: asc");

                await channel.BasicAckAsync(
                    ea.DeliveryTag, false, token);

                Console.WriteLine("Video.Upload: process succed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"video uploding consumer: in task {ea.DeliveryTag} exception: {ex}");

                await channel.BasicNackAsync(
                    ea.DeliveryTag, false, false);
            }
        };

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 2,
            global: false,
            token);

        await channel.BasicConsumeAsync(
            queue: "video.upload",
            autoAck: false,
            "video.upload-consumer",
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer,
            token);
    }

    public void Dispose()
    {
        channel?.Dispose();
    }
}