using OpenCvSharp;
using Training.RepairSystem.Services.Interfaces;

namespace Training.RepairSystem.Services;

public class PhotoService : IPhotoService
{

    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<PhotoService> _logger;

    public PhotoService(
        IWebHostEnvironment environment,
        ILogger<PhotoService> logger
    )
    {
        _environment = environment;
        _logger = logger;
    }



    // 儲存照片
    public async Task<string?> SavePhotoAsync(
        IFormFile file,
        CancellationToken cancellationToken = default
    )
    {

        // 沒有上傳照片
        if (file == null || file.Length == 0)
        {
            return null;
        }


        // 限制大小 5MB
        if (file.Length > 5 * 1024 * 1024)
        {
            _logger.LogWarning(
                "圖片大小超過限制"
            );

            return null;
        }



        // 允許副檔名
        var allowedExtensions =
            new[]
            {
                ".jpg",
                ".jpeg",
                ".png"
            };

        var extension =
            Path.GetExtension(file.FileName)
            .ToLower();

        if (!allowedExtensions.Contains(extension))
        {
            _logger.LogWarning(
                "圖片格式不允許:{Extension}",
                extension
            );

            return null;
        }


        // 驗證圖片是否真的可讀

        using var inputStream =
           file.OpenReadStream();


        using var memoryStream =
            new MemoryStream();


        await inputStream.CopyToAsync(
            memoryStream,
            cancellationToken
        );


        var bytes =
            memoryStream.ToArray();


        using var mat =
            Cv2.ImDecode(
                bytes,
                ImreadModes.Color
            );


        if (mat.Empty())
        {
            _logger.LogWarning(
                "圖片內容無法解析"
            );

            return null;
        }



        if (mat.Empty())
        {
            _logger.LogWarning(
                "圖片內容無法解析"
            );

            return null;
        }
        // 建立 Storage/Uploads
        var uploadFolder =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "Storage",
                "Uploads"
            );


        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }



        // 伺服器產生檔名
        var fileName =
            $"{Guid.NewGuid()}{extension}";


        var filePath =
            Path.Combine(
                uploadFolder,
                fileName
            );



        using (
            var fileStream =
            new FileStream(
                filePath,
                FileMode.Create
            )
        )
        {
            await file.CopyToAsync(
                fileStream,
                cancellationToken
            );
        }



        return
            $"Storage/Uploads/{fileName}";
        
    }
}
