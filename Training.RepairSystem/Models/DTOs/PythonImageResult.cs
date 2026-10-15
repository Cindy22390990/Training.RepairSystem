namespace Training.RepairSystem.Models.DTOs;

public class PythonImageResult
{
    // 是否成功。
    public bool Success { get; set; }

    // Python 產生的預覽圖。
    public string? PreviewPath { get; set; }

    /// 失敗訊息。
    public string? Message { get; set; }
}
