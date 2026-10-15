using System.ComponentModel.DataAnnotations;

namespace Training.RepairSystem.Models.DTOs;

public class CreateRepairTicketRequest
{
    // 設備名稱
    [Required]
    [StringLength(200)]
    public string EquipmentName { get; set; } = string.Empty;

    // 故障描述
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    // 優先級
    [Required]
    [RegularExpression(
        "Low|Normal|High",
        ErrorMessage = "Priority只能是Low、Normal、High"
    )]
    public string Priority { get; set; } = "Normal";

    //照片上傳 + Python/OpenCV
    public IFormFile? File { get; set; }
}