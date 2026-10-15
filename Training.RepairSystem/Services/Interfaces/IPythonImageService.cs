using Training.RepairSystem.Models.DTOs;

namespace Training.RepairSystem.Services.Interfaces;

public interface IPythonImageService
{
    // 呼叫 Python/OpenCV 處理圖片。
    Task<PythonImageResult> ProcessImageAsync(
        string inputPath,
        string outputPath,
        CancellationToken cancellationToken = default
    );
}