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
    private IChannel? channel;

    public VideoUploadingConsumer(IServiceScopeFactory scopeFactory, IFfmpegService ffmpegService)
    {
        this.ffmpegService = ffmpegService;
        this.scopeFactory = scopeFactory;
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

                Content? content = await context.Contents.FindAsync(message.ContentId);

                if (content == null)
                {
                    Console.WriteLine("Video.Upload: content is null");

                    await channel.BasicNackAsync(
                        ea.DeliveryTag, false, false);

                    return;
                }

                content.ContentUrl = await ffmpegService.UploadGeneratedVideos(message.VideoPath);                

                int duration = await GetVideoDuration(message.VideoPath);

                VideoMetaData metaData = new()
                {
                    Content = content,
                    DurationSeconds = duration
                };

                context.VideoMetas.Add(metaData);
                context.ContentVectors.AddRange(await context.Genres
                    .Select(genre => new ContentGenreVector()
                    {
                        Content = content,
                        GenreId = genre.Id
                    })
                    .AsNoTracking()
                    .ToArrayAsync(token)
                );

                await context.SaveChangesAsync();

                await scope.ServiceProvider.GetRequiredService<IContentRepository>()
                    .CreateSearchIndex(content, metaData, token);

                await channel.BasicAckAsync(
                    ea.DeliveryTag, false, token);
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
            prefetchCount: 1,
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

    private async Task<int> GetVideoDuration(string? videoPath)
    {
        if (videoPath != null)
        {
            int duration = await ffmpegService.GetVideoDuration(videoPath);

            File.Delete(videoPath);

            return duration;
        }

        return 0;
    }

    public void Dispose()
    {
        channel?.Dispose();
    }
}