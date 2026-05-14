namespace Domain.Common.Interfaces.Services;

public interface IFfmpegService
{
    Task<string> UploadGeneratedVideos(string videoPath, CancellationToken token = default);
    Task<string> RunProcess(string args, string fileName = "ffmpeg", CancellationToken token = default);
    Task<int> GetVideoDuration(string videoPath, CancellationToken token = default);
    Task<string> GetPhotoFromVideo(string videoPath, string outputPath = "/storage/images/previewPhoto.jpeg", int timeSeconds = 5, CancellationToken token = default);
}