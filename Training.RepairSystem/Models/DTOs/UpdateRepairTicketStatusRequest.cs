namespace Training.RepairSystem.Models.DTOs;

public class UpdateRepairTicketStatusRequest
{
    // 用來存放新的維修狀態。
    public string Status { get; set; } = string.Empty;
}