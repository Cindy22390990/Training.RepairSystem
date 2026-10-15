using Training.RepairSystem.Models;
namespace Training.RepairSystem.Services.Interfaces;

public interface ITokenService
{
    string CreateToken(UserAccount user);
}
