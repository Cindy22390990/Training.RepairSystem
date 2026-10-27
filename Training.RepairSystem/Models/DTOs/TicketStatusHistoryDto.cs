namespace Training.RepairSystem.Models.DTOs;

public class TicketStatusHistoryDto
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public string? OldStatus { get; set; }

    public string NewStatus { get; set; } = string.Empty;

    public int ChangedByUserId { get; set; }

    public DateTime ChangedAtUtc { get; set; }

    public string? Note { get; set; }
}