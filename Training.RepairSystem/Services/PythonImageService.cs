using System.Diagnostics;
using System.Text.Json;
using Training.RepairSystem.Models.DTOs;
using Training.RepairSystem.Services.Interfaces;

namespace Training.RepairSystem.Services;

public class PythonImageService : IPythonImageService
{
    private static readonly JsonSerializerOptions JsonOptions =
    new()
    {
        PropertyNameCaseInsensitive = true
    };
    // 讀取 appsettings.json。
    private readonly IConfiguration _configuration;
    private readonly ILogger<PythonImageService> _logger;
    public PythonImageService(
        IConfiguration configuration,
        ILogger<PythonImageService> logger
    )
    {
        _configuration = configuration;
        _logger = logger;
    }

    // 呼叫 Python 處理圖片。
    public async Task<PythonImageResult> ProcessImageAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default
    )
    {
        // Python 執行檔。
        var pythonExe =
            _configuration["Python:PythonExe"];

        // process_image.py 路徑。
        var pythonScript =
            _configuration["Python:ScriptPath"];

        // 建立 ProcessStartInfo。
        var startInfo = new ProcessStartInfo
        {
            FileName = pythonExe,

            RedirectStandardOutput = true,

            RedirectStandardError = true,

            UseShellExecute = false,

            CreateNoWindow = true
        };

        // 安全加入參數。
        startInfo.ArgumentList.Add(
            pythonScript!
        );

        startInfo.ArgumentList.Add(
            inputPath
        );

        startInfo.ArgumentList.Add(
            outputPath
        );

        // 建立 Process。
        using var process =
            new Process();

        process.StartInfo = startInfo;

        // 啟動 Python。
        process.Start();

        // 讀 stdout。
        var stdoutTask =
            process.StandardOutput.ReadToEndAsync();

        // 讀 stderr。
        var stderrTask =
            process.StandardError.ReadToEndAsync();

        // 等待 Python 結束。
        await process.WaitForExitAsync(
            cancellationToken
        );

        // 等待兩個輸出完成。
        var stdout =
            await stdoutTask;

        var stderr =
            await stderrTask;

        // Python 發生錯誤。
        if (process.ExitCode != 0)
        {
            _logger.LogError(
                "Python圖片處理失敗，ExitCode:{ExitCode}, Error:{Error}",
                process.ExitCode,
                stderr
            );

            return new PythonImageResult
            {
                Success = false,
                Message = stderr
            };
        }

        // 將 JSON 轉成物件。
        var result =
            JsonSerializer.Deserialize<PythonImageResult>(
                stdout,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        if (result == null)
        {
            _logger.LogError(
                "Python回傳JSON格式錯誤，Output:{Output}",
                stdout
            );

            return new PythonImageResult
            {
                Success = false,
                Message = "Python 回傳格式錯誤"
            };
        }

        return result;
    }
}
