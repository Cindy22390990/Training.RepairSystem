using Training.RepairSystem.Models.DTOs;

namespace Training.RepairSystem.Services.Interfaces;

public interface IAccountService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
