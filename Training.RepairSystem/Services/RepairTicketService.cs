using System.Net.Sockets;
using Training.RepairSystem.Models;
using Training.RepairSystem.Models.DTOs;
using Training.RepairSystem.Repositories.Interfaces;
using Training.RepairSystem.Services.Interfaces;

namespace Training.RepairSystem.Services;

public class RepairTicketService : IRepairTicketService
{
    private readonly IRepairTicketRepository _repository;

    private readonly IPhotoService _photoService;

    // Python/OpenCV服務
    private readonly IPythonImageService _pythonImageService;

    private readonly ILogger<RepairTicketService> _logger;

    public RepairTicketService(
    IRepairTicketRepository repository,
    IPhotoService photoService,
    IPythonImageService pythonImageService,
    ILogger<RepairTicketService> logger
)
    {
        _repository = repository;

        _photoService = photoService;

        _pythonImageService = pythonImageService;

        _logger = logger;
    }

    // 1. 建立報修工單
    // POST /api/RepairTicket/create
    public async Task<RepairTicket?> CreateAsync(
        CreateRepairTicketRequest request,
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        // 儲存原始照片路徑。
        string? photoPath = null;


        // 儲存 Preview 圖路徑。
        string? previewPath = null;

        // 儲存 Employee 上傳的原始圖片
        if (request.File != null)
        {
            var allowedExtensions =
                new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png"
                };


            var extension =
                Path.GetExtension(
                    request.File.FileName
                )
                .ToLower();


            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning(
                    "圖片格式錯誤，FileName:{FileName}",
                    request.File.FileName
                );

                return null;
            }


            if (request.File.Length > 5 * 1024 * 1024)
            {
                _logger.LogWarning(
                    "圖片超過5MB"
                );

                return null;
            }
            photoPath =
                await _photoService.SavePhotoAsync(
                    request.File,
                    cancellationToken
                );
            if (string.IsNullOrEmpty(photoPath))
            {
                return null;
            }

            // 原圖完整路徑。
            var inputPath =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                photoPath
            );

            // Preview 圖位置。
            var previewFileName =
                $"preview_{Guid.NewGuid()}.png";


            var outputPath =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                previewFileName
            );

            var pythonResult =
                    await _pythonImageService.ProcessImageAsync(
                        inputPath,
                        outputPath,
                        cancellationToken
                    );



            // Python失敗。
            if (!pythonResult.Success)
            {
                _logger.LogError(
                    "Python圖片處理失敗，原因:{Message}",
                    pythonResult.Message
                );

                previewPath = null;
            }
                else
                {
                    previewPath =
                        Path.Combine(
                            "uploads",
                            previewFileName
                        )
                        .Replace("\\", "/");
                }

        }

            // 建立一個新的 RepairTicket 物件。
            var ticket = new RepairTicket
            {
                // 設定維修單擁有者。
                OwnerUserId = userId,


                // 設定設備名稱。
                EquipmentName = request.EquipmentName,


                // 設定故障描述。
                Description = request.Description,


                // 設定優先級。
                Priority = request.Priority,


                // 新建立立時狀態。
                Status = "Open",


                // 原始照片。
                PhotoPath = photoPath,


                // Python產生的Preview。
                PreviewPath = previewPath,


                // 建立時間。
                CreatedAtUtc = DateTime.UtcNow,


                // 更新時間。
                UpdatedAtUtc = DateTime.UtcNow
            };

            // 呼叫 Repository 新增資料。
            await _repository.AddAsync(
                ticket,
                cancellationToken
 
            );
            _logger.LogInformation(
                "建立報修工單成功，TicketId:{TicketId}, UserId:{UserId}",
                ticket.Id,
                userId
            );

                // 回傳新增完成後的維修單。
                return ticket;
    }

    // 2. 查詢自己的報修列表
    // GET /api/RepairTicket
    public async Task<List<RepairTicket>> GetMyTicketsAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        // 呼叫 Repository 層。
        var tickets =
            await _repository.GetMyTicketsAsync(
                userId,
                cancellationToken
            );

        _logger.LogInformation(
            "查詢自己的報修工單成功，UserId:{UserId}, Count:{Count}",
            userId,
            tickets.Count
        );

        return tickets;
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

        // 呼叫 Repository 的查詢方法。
        return await _repository.GetByIdAsync(
            id,
            userId,
            role,
            cancellationToken
        );
    }

    // 4. 維修人員查看全部工單
    // GET /api/RepairTicket/maintenance
    public async Task<List<RepairTicket>> GetAllAsync(
            CancellationToken cancellationToken = default
    )
    {
        return await _repository.GetAllAsync(
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
        // 呼叫 Repository 層的 UpdateStatusAsync 。
        var ticket =
            await _repository.UpdateStatusAsync(
            id,
            status,
            userId,
            cancellationToken
            );


        if (ticket == null)
        {
            _logger.LogWarning(
                "更新工單失敗，找不到工單，TicketId:{TicketId}",
                id
            );

            return null;
        }


        _logger.LogInformation(
            "更新工單狀態成功，TicketId:{TicketId}, Status:{Status}, UserId:{UserId}",
            id,
            status,
            userId
        );


        return ticket;
    }
    public async Task<List<TicketStatusHistoryDto>> GetStatusHistoryAsync(
        int ticketId,
        int userId,
        string? role,
        CancellationToken cancellationToken = default
    )
        {
            _logger.LogInformation(
            "查詢歷程 TicketId:{TicketId}, UserId:{UserId}, Role:{Role}",
            ticketId,
            userId,
            role
        );
        var ticket =
            await _repository.GetByIdAsync(
                ticketId,
                userId,
                role,
                cancellationToken
            );

        if (ticket == null)
        {
            return new List<TicketStatusHistoryDto>();
        }


        var histories =
            await _repository.GetStatusHistoryAsync(
                ticketId,
                userId,
                role,
                cancellationToken
            );


        return histories
            .Select(x => new TicketStatusHistoryDto
            {
                Id = x.Id,

                TicketId = x.TicketId,

                OldStatus = x.OldStatus,

                NewStatus = x.NewStatus,

                ChangedByUserId = x.ChangedByUserId,

                ChangedAtUtc = x.ChangedAtUtc,

                Note = x.Note

            })
            .ToList();
            }

}
