using Domain.Common.Interfaces.Services;
using System.Globalization;
using System.Diagnostics;

namespace Infrastructure.Services;

public class FfmpegService : IFfmpegService
{
    public readonly IR2Service r2Service;

    public FfmpegService(IR2Service r2Service)
    {
        this.r2Service = r2Service;
    }

    public async Task<string> UploadGeneratedVideos(string videoPath, CancellationToken token = default)
    {
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            await GenerateHlsVariants(videoPath, tempDir, token);

            string folderKey = $"videos/{Guid.NewGuid()}";

            string url = r2Service.SaveVideo(tempDir, folderKey);

            return url;
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    private async Task GenerateHlsVariants(string videoPath, string outputDir, CancellationToken token = default)
    {
        Directory.CreateDirectory(outputDir);

        // video height
        int height = await GetVideoHeight(videoPath, token);

        int result = await GetVideoFps(videoPath, token);

        int fps = result > 60 ? 60 : result;

        var variants = new List<int>();

        if (height >= 1080)
            variants.AddRange([360, 480, 720, 1080]);
        else if (height >= 720)
            variants.AddRange([360, 480, 720]);
        else if (height >= 480)
            variants.AddRange([360, 480]);
        else if (height >= 360)
            variants.AddRange([360]);

        int count = variants.Count;

        string split = $"[0:v]split={count}";

        for (int i = 0; i < count; i++)
            split += $"[v{i}]";

        split += ";";

        string filters = "";

        for (int i = 0; i < count; i++)
            filters += $"[v{i}]fps={fps},scale=-2:{variants[i]}[v{variants[i]}];";

        string maps = "";
        var streamMaps = new List<string>();

        int gap = fps * 2;

        for (int i = 0; i < count; i++)
        {
            maps += $"-map \"[v{variants[i]}]\" ";
            maps += "-map 0:a:0 ";

            maps += $"-c:v:{i} libx264 -preset veryfast ";
            maps += $"-b:v:{i} {GetBitrate(variants[i], fps)} ";

            maps += $"-c:a:{i} aac -b:a:{i} 128k ";

            maps += $"-g {gap} -keyint_min {gap} -sc_threshold 0 ";

            streamMaps.Add($"v:{i},a:{i}");
        }

        string streamMap = string.Join(" ", streamMaps);

        string args =
            $"-i \"{videoPath}\" " +
            $"-filter_complex \"{split}{filters}\" " +
            maps +
            "-f hls " +
            "-hls_time 4 " +
            "-hls_playlist_type vod " +
            "-hls_segment_filename \"" + Path.Combine(outputDir, "stream_%v_%03d.ts") + "\" " +
            "-master_pl_name master.m3u8 " +
            $"-var_stream_map \"{streamMap}\" " +
            $"{Path.Combine(outputDir, "stream_%v.m3u8")}";

        await RunProcess(args, token: token);
    }

    private async Task<int> GetVideoHeight(string videoPath, CancellationToken token = default)
    {
        string output = await RunProcess(
            $"-v error -select_streams v:0 -show_entries stream=height -of csv=p=0 \"{videoPath}\"",
            "ffprobe", token);
        return int.Parse(output.Trim());
    }

    private async Task<int> GetVideoFps(string videoPath, CancellationToken token = default)
    {
        string output = await RunProcess(
            $"-v error -select_streams v:0 " +
            $"-show_entries stream=r_frame_rate " +
            $"-of default=noprint_wrappers=1:nokey=1 " +
            $"\"{videoPath}\"",
            "ffprobe",
            token);

        string value = output.Trim();

        if (value.Contains('/'))
        {
            string[] parts = value.Split('/');

            double numerator = double.Parse(parts[0]);
            double denominator = double.Parse(parts[1]);

            return (int)Math.Round(numerator / denominator);
        }

        return (int)Math.Round(double.Parse(value));
    }

    public async Task<string> RunProcess(string args, string fileName = "ffmpeg", CancellationToken token = default)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = args,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi) ?? throw new Exception("failed to start ffmpeg");

        var errorTask = process.StandardError.ReadToEndAsync(token);
        var outputTask = process.StandardOutput.ReadToEndAsync(token);

        await process.WaitForExitAsync(token);

        string error = await errorTask;
        string output = await outputTask;

        if (process.ExitCode != 0)
            throw new Exception($"{fileName} failed with code: {process.ExitCode}\n{error}");

        Console.WriteLine($"{fileName} finished process: {output}");

        return output;
    }

    public async Task<double> GetVideoDuration(string videoPath, CancellationToken token = default)
    {
        string output = await RunProcess(
            $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{videoPath}\"",
            "ffprobe", token);

        double seconds = double.Parse(output, CultureInfo.InvariantCulture);

        return Math.Round(seconds);
    }

    public async Task<string> GetPhotoFromVideo(string videoPath, string outputPath = "/storage/images/previewPhoto.jpeg", double timeSeconds = 5, CancellationToken token = default)
    {
        string finalOutput = Path.Combine(
            Path.GetDirectoryName(outputPath)!,
            $"{Guid.NewGuid()}{Path.GetExtension(outputPath)}");

        await RunProcess(
            $"-ss {timeSeconds} -i \"{videoPath}\" -frames:v 1 \"{finalOutput}\"", token: token);

        return finalOutput;
    }

    private string GetBitrate(int height, int fps)
    {
        double baseRate = height switch
        {
            360 => 800,
            480 => 1400,
            720 => 2800,
            1080 => 6000,
            _ => 800
        };

        double factor = fps switch
        {
            <= 30 => 1.0,
            <= 60 => 1.5,
            _ => 1.5
        };

        return $"{(int)(baseRate * factor)}k";
    }
}