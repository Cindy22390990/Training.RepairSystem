using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Training.RepairSystem.Models.DTOs;
using Training.RepairSystem.Services.Interfaces;

namespace Training.RepairSystem.Controllers;

[ApiController]
// 設定 API 路由。
//POST/api/Auth/login
[Route("api/[controller]")]
public class RepairTicketController : ControllerBase
{
    private readonly IRepairTicketService _repairTicketService;
    private readonly ILogger<RepairTicketController> _logger;
    public RepairTicketController(
        IRepairTicketService repairTicketService,
        ILogger<RepairTicketController> logger
    )
    {  
        _repairTicketService = repairTicketService;
        _logger = logger;
    }

    // 1. 建立報修工單_Employee
    // POST /api/RepairTicket/create
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(
            [FromForm] CreateRepairTicketRequest request,
            CancellationToken cancellationToken = default
    )
    // 呼叫 Service 建立維修單。
    {
        // 從目前登入使用者的 Claims 裡尋找 User ID。
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier
    );
        // 檢查是否有找到使用者 ID。
        if (userIdClaim == null)
        {
            return Unauthorized();
        }
        // 把 Claim 裡的文字 ID 轉成 int。
        int userId =
            int.Parse(userIdClaim.Value);
        var result =
           await _repairTicketService.CreateAsync(
                request,
                userId,
               cancellationToken
           );
        // 如果建立失敗：
        if (result == null)
        {
            return BadRequest();
        }

        // 建立成功。
        return Ok(result);
    }

    // 2. 查詢自己的報修列表_Employee
    //GET/api/RepairTicket
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetMyTickets(
    CancellationToken cancellationToken
    )
    {
        // 從 JWT Token 中取得 User ID。
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier
            );


        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        // Claim 的資料都是 string。
        int userId =
            int.Parse(userIdClaim.Value);


        var result =
            await _repairTicketService.GetMyTicketsAsync(
                userId,
                cancellationToken
            );


        return Ok(result);
    }

    //3. 查詢單筆工單_Employee/Maintenance
    //GET /api/RepairTicket/{id}
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
    int id,
    CancellationToken cancellationToken
    )
    {
        // 從 JWT Token 取得登入者 ID。
        var userIdClaim =
        User.FindFirst(
            ClaimTypes.NameIdentifier
        );

        // 從 JWT Token 取得登入者 ID。
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        // Claim 裡的資料都是文字。
        int userId =
            int.Parse(userIdClaim.Value);
        var role =
            User.FindFirst(
                ClaimTypes.Role
            )?.Value;
        // 呼叫 Service 查詢維修單。
        var result =
            await _repairTicketService.GetByIdAsync(
                id,
                userId,
                role,
                cancellationToken
            );

        // 查不到維修單。
        if (result == null)
        {
            return NotFound();
        }


        return Ok(result);
    }

    // 4. 維修人員查看全部工單_Maintenance
    //GET/api/RepairTicket/maintenance
    [Authorize(Roles = "Maintenance")]
    //GET /api/RepairTicket/maintenance
    [HttpGet("maintenance")]
    public async Task<IActionResult> GetAll(
    CancellationToken cancellationToken
    )
    {
        var result =
            await _repairTicketService.GetAllAsync(
                cancellationToken
            );
        return Ok(result);
    }

    //5. 更新工單狀態_Maintenance
    //PUT/api/RepairTicket/{id}/status
    [Authorize(Roles = "Maintenance")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateRepairTicketStatusRequest request,
        CancellationToken cancellationToken
    )
    {
        // 從 JWT 取得登入者 ID
        var userIdClaim =
            User.FindFirst(
                ClaimTypes.NameIdentifier
            );


        if (userIdClaim == null)
        {
            return Unauthorized();
        }


        int userId =
            int.Parse(
                userIdClaim.Value
            );

        //驗證
        var allowedStatuses = new[]
        {
            "Open",
            "InProgress",
            "Resolved"
        };

        if (!allowedStatuses.Contains(request.Status))
        {
            return BadRequest(
                "Status 不合法"
            );
        }

        var result =
        await _repairTicketService.UpdateStatusAsync(
            id,
            request.Status,
            userId,
            cancellationToken
        );

        // Service 回傳 null。
        if (result == null)
        {
            return NotFound();
        }
        _logger.LogInformation(
            "更新工單狀態成功，TicketId:{TicketId}, Status:{Status}",
            id,
            request.Status
        );


        return Ok(result);
    }
}
