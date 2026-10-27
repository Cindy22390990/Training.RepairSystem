using Microsoft.EntityFrameworkCore;
using Training.RepairSystem.Data;
using Training.RepairSystem.Models;
using Training.RepairSystem.Repositories.Interfaces;

namespace Training.RepairSystem.Repositories;

public class RepairTicketRepository : IRepairTicketRepository
{
    private readonly AppDbContext _context;
    public RepairTicketRepository(
        AppDbContext context
    )
    {
        _context = context;
    }
    // 1. 建立報修工單
    // POST /api/RepairTicket/create
    public async Task AddAsync(
        RepairTicket ticket,
        CancellationToken cancellationToken = default
    )
    {
        //Service新增維修單，Repository放進資料庫
        //把物件放進 EF Core 的追蹤清單
        _context.RepairTickets.Add(ticket);
        //存到資料庫
        await _context.SaveChangesAsync(cancellationToken);
    }

    // 2. 查詢自己的報修列表
    // GET /api/RepairTicket
    public async Task<List<RepairTicket>> GetMyTicketsAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    // 使用 Entity Framework Core 查詢資料庫。
    {
        return await _context.RepairTickets
            .Where(
                // x:代表 RepairTickets 裡的一筆資料。
                x => x.OwnerUserId == userId
            )
            //按建立時間排序。
            .OrderByDescending(
                x => x.CreatedAtUtc
            )
            // 執行查詢。
            .ToListAsync(
                cancellationToken
            );
    }

    // 3. 查詢單筆工單
    // GET /api/RepairTicket/{id}
    public async Task<RepairTicket?> GetByIdAsync(
    int id,
    int userId,
    string? role,
    CancellationToken cancellationToken = default
    )
    {
        // Maintenance 可以查看全部工單
        if (role == "Maintenance")
        {
            return await _context.RepairTickets
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken
                );
        }
        // Employee只能查看自己的工單
        return await _context.RepairTickets
            .FirstOrDefaultAsync(
                x => 
                x.Id == id &&
                x.OwnerUserId == userId,
                cancellationToken
            );
    }

    // 4. 維修人員查看全部工單
    // GET /api/RepairTicket/maintenance
    public async Task<List<RepairTicket>> GetAllAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _context.RepairTickets
            //按建立時間排序。
            .OrderByDescending(
                x => x.CreatedAtUtc
            )
            // 執行查詢。
            .ToListAsync(
                cancellationToken
            );
    }

    // 5. 更新工單狀態
    // PUT /api/RepairTicket/{id}/status
    public async Task<RepairTicket?> UpdateStatusAsync(
        int id,
        string status,
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        // 從資料庫的 RepairTickets 表查詢資料。
        var ticket =
            await _context.RepairTickets
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken
            );


        if (ticket == null)
        {
            return null;
        }

        var allowedStatuses = new[]
        {
            "Open",
            "InProgress",
            "Resolved"
        };


        if (!allowedStatuses.Contains(status))
        {
            return null;
        }
        // 原本狀態
        var oldStatus = ticket.Status;

        // 修改維修單狀態。
        ticket.Status = status;

        // 更新最後修改時間。
        ticket.UpdatedAtUtc = DateTime.UtcNow;


        // 建立狀態歷程紀錄
        var history = new TicketStatusHistory
        {
            TicketId = ticket.Id,

            OldStatus = oldStatus,

            NewStatus = status,

            ChangedByUserId = userId,

            ChangedAtUtc = DateTime.UtcNow
        };


        // 加入狀態歷程資料表
        _context.TicketStatusHistories.Add(history);


        // 一次儲存工單 + 歷程
        await _context.SaveChangesAsync(
            cancellationToken
        );

        return ticket;
    }
    
    
}
