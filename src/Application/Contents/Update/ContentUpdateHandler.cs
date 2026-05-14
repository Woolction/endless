using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Domain.Common.Interfaces.Services;
using Application.Contents.Dtos;
using Domain.Common.Interfaces.Db;
using Domain.Entities;
using MediatR;
using Application.Dtos;

namespace Application.Contents.Update;

public class ContentUpdateHandler : IRequestHandler<ContentUpdateCommand, Result<ContentDto>>
{
    private readonly ILogger<ContentUpdateHandler> logger;
    private readonly IAppDbContext context;

    private readonly IFfmpegService ffmpegService;
    private readonly IR2Service r2Service;
    public ContentUpdateHandler(IAppDbContext context, ILogger<ContentUpdateHandler> logger, IFfmpegService ffmpegService, IR2Service r2Service)
    {
        this.context = context;
        this.logger = logger;

        this.ffmpegService = ffmpegService;
        this.r2Service = r2Service;
    }

    public async Task<Result<ContentDto>> Handle(ContentUpdateCommand request, CancellationToken cancellationToken)
    {
        User? user = await context.Users.FindAsync(request.UserId, cancellationToken);

        if (user == null)
            return Result<ContentDto>.Failure(404, "User not found");

        var content = await context.Contents
            .Where(content =>
                content.Id == request.ContentId &&
                content.CreatorId == request.UserId)
            .Include(c => c.VideoMeta)
            .Select(content => new
            {
                c = content,
                SaversCount = content.Savers.Count,
                LikersCount = content.Likers.Count,
                CommentsCount = content.Comments.Count,
                DisLikersCount = content.DisLikers.Count
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (content == null)
            return Result<ContentDto>.Failure(404, "Content not found");

        string videoUrl = null!;
        string videoPath = null!;

        if (request.ContentFile != null && request.ContentFile.Length != 0)
        {
            videoPath = await r2Service.SaveFormFileAsync(request.ContentFile, "Video", token: cancellationToken);
            videoUrl = await ffmpegService.UploadGeneratedVideos(videoPath, cancellationToken);
        }

        string photoUrl = null!;

        if (request.PrewievPhoto != null && request.PrewievPhoto.Length != 0)
        {
            string photoPath = await r2Service.SaveFormFileAsync(request.PrewievPhoto, "Images", ".jpeg", cancellationToken);
            photoUrl = r2Service.SaveImage(photoPath);

            File.Delete(photoPath);
        }

        content.c.Title = request.Title;
        content.c.ContentType = request.ContentType;

        content.c.VideoMeta.VideoUrl = videoUrl;
        content.c.VideoMeta.PhotoUrl = photoUrl;

        if (videoPath != null)
        {
            content.c.VideoMeta.DurationSeconds = await ffmpegService.GetVideoDuration(videoPath);

            File.Delete(videoPath);
        }

        await context.SaveChangesAsync();

        logger.LogInformation("Content {ContentId} updated successfully",
            request.ContentId);

        ContentDto contentDto = new(
            content.c.Id, content.c.ChannelId, content.c.CreatorId,
            content.c.Title, content.c.Slug, content.c.Description,
            content.c.CreatedDate, content.c.ContentType.ToString(),
            content.c.VideoMeta.DurationSeconds, content.c.VideoMeta.VideoUrl,
            new PreviewPhotoDto(content.c.VideoMeta.PhotoUrl, content.c.VideoMeta.R, content.c.VideoMeta.G, content.c.VideoMeta.B),
            content.SaversCount, content.LikersCount, content.CommentsCount, content.DisLikersCount, content.c.ViewsCount);

        return Result<ContentDto>.Success(200, contentDto);
    }
}