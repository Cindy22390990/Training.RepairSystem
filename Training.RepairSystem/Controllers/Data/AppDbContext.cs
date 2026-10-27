using Microsoft.EntityFrameworkCore;
using Training.RepairSystem.Models;


namespace Training.RepairSystem.Data;


public class AppDbContext : DbContext
{

    public AppDbContext(
        DbContextOptions<AppDbContext> options
    )
        : base(options)
    {

    }
    //OnModelCreating() 的用途是設定 EF Core 資料模型與初始化資料
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserAccount -> RepairTicket
        modelBuilder.Entity<RepairTicket>()
           .HasOne(x => x.OwnerUser)
           .WithMany(x => x.RepairTickets)
           .HasForeignKey(x => x.OwnerUserId)
           .OnDelete(DeleteBehavior.Restrict);

        // RepairTicket -> TicketStatusHistory
        modelBuilder.Entity<TicketStatusHistory>()
            .HasOne(x => x.Ticket)
            .WithMany(x => x.StatusHistories)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<UserAccount> UserAccounts { get; set; }

    public DbSet<RepairTicket> RepairTickets { get; set; }
    public DbSet<TicketStatusHistory> TicketStatusHistories
    {
        get;
        set;
    }

}