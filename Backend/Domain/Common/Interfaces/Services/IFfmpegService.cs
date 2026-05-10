namespace Domain.Common.Interfaces.Services;

public interface IFfmpegService
{
    Task<string> UploadGeneratedVideos(string inputFile);
    Task<int> GetVideoDuration(string filePath);
}