using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Training.RepairSystem.Data;
using Training.RepairSystem.Models;
using Training.RepairSystem.Models.DTOs;
using Training.RepairSystem.Services.Interfaces;


namespace Training.RepairSystem.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<UserAccount> _passwordHasher;
    private readonly ITokenService _tokenService;
    //DI 要怎麼知道給什麼？靠 Constructor。
    public AccountService(
        AppDbContext context,
        ITokenService tokenService
    )
    {
        _context = context;
        _tokenService = tokenService;
        //這個 PasswordHasher 是用來處理 UserAccount 的密碼。
        _passwordHasher = new PasswordHasher<UserAccount>();
    }
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        // 1. 根據帳號查詢使用者
        var user = await _context.UserAccounts
            .FirstOrDefaultAsync(
                x => x.UserName == request.UserName
        );
        // 2. 帳號不存在
        if (user == null)
        {
            return null;
        }
        // 3. 驗證密碼
        var passwordValid =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);
        // 4. 密碼錯誤
        if (passwordValid != PasswordVerificationResult.Success)
        {
            return null;
        }
        // 產生 JWT
        var token = _tokenService.CreateToken(user);
        //成功登入後回傳
        return new LoginResponse
        {
                UserName = user.UserName,
                Role = user.Role,
                Token = token
        };
    }
}
