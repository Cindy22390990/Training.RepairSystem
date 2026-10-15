namespace Training.RepairSystem.Services.Interfaces;

public interface IPhotoService
{

    // 儲存使用者上傳照片
    Task<string?> SavePhotoAsync(
        IFormFile file,
        CancellationToken cancellationToken = default
    );
}
