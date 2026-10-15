using Training.RepairSystem.Models;
using Training.RepairSystem.Models.DTOs;


namespace Training.RepairSystem.Services.Interfaces
{
    public interface IRepairTicketService
    {
        // 1. 建立報修工單
        // POST /api/RepairTicket/create
        Task<RepairTicket?> CreateAsync(
            CreateRepairTicketRequest request,
            int userId,
            CancellationToken cancellationToken = default
        );

        // 2. 查詢自己的報修列表
        // GET /api/RepairTicket
        Task<List<RepairTicket>> GetMyTicketsAsync(
            int userId,
            CancellationToken cancellationToken = default
        );

        // 3. 查詢單筆工單
        // GET /api/RepairTicket/{id}
        Task<RepairTicket?> GetByIdAsync(
            int id,
            int userId,
            string? role,
            CancellationToken cancellationToken = default
        );

        // 4. 維修人員查看全部工單
        // GET /api/RepairTicket/maintenance
        Task<List<RepairTicket>> GetAllAsync(
            CancellationToken cancellationToken = default
        );

        // 5. 更新工單狀態
        // PUT /api/RepairTicket/{id}/status
        Task<RepairTicket?> UpdateStatusAsync(
            int id,
            string status,
            CancellationToken cancellationToken = default
        );
        

    }
}
