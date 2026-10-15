using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Training.RepairSystem.Models;

[Index(nameof(UserName), IsUnique = true)]
public class UserAccount
{
    public int Id { get; set; }
    [StringLength(100)]
    public string UserName { get; set; } = string.Empty;
    [StringLength(500)]
    public string PasswordHash { get; set; }= string.Empty;
    [StringLength(30)]
    public string Role {  get; set; }= "Employee";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    //一個 UserAccount 裡面，可以找到他的所有工單。
    public ICollection<RepairTicket> RepairTickets { get; set; } = new List<RepairTicket>();
}
