namespace Training.RepairSystem.Models;

public class TicketStatusHistory
{
    public int Id { get; set; }

    // 對應哪一張工單
    public int TicketId { get; set; }


    public string? OldStatus { get; set; }


    public string NewStatus { get; set; } = string.Empty;


    public int ChangedByUserId { get; set; }


    public DateTime ChangedAtUtc { get; set; }


    public string? Note { get; set; }


    public RepairTicket Ticket { get; set; } = null!;
}