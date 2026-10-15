using System.ComponentModel.DataAnnotations;

namespace Training.RepairSystem.Models;

public class RepairTicket
{
    public int Id { get; set; }

    // 建立工單的使用者
    public int OwnerUserId { get; set; }

    //一張工單可以知道：「我是誰建立的。」
    public UserAccount? OwnerUser { get; set; }

    // 設備名稱
    [StringLength(200)]
    public string EquipmentName { get; set; } = string.Empty;

    // 故障描述
    [StringLength(1000)]
    public string Description { get; set;} = string.Empty;
    
    // 優先級
    [StringLength(20)]
    public string Priority { get; set; } = "Normal";

    // 狀態
    [StringLength(20)]
    public string Status { get; set; } = "Open";

    // 原始照片路徑
    [StringLength(500)]
    public string? PhotoPath { get; set; }

    // Python/OpenCV產生預覽圖路徑
    [StringLength(500)]
    public string? PreviewPath { get; set; }

    // 建立時間
    public DateTime CreatedAtUtc { get; set; }

    // 更新時間
    public DateTime UpdatedAtUtc { get; set; }

}
