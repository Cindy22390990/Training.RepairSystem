using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Training.RepairSystem.Models;
using Training.RepairSystem.Services.Interfaces;

namespace Training.RepairSystem.Services;

public class TokenService : ITokenService
{
    // 建立 TokenService 時，.NET 會自動把 IConfiguration 傳進來。
    public TokenService(
        IConfiguration configuration
    )
    {
        _configuration = configuration;
    }
    //宣告一個私有、唯讀的設定檔物件變數。
    private readonly IConfiguration _configuration;
    // 建立 JWT Token 的方法。
    public string CreateToken(UserAccount user)
    {
        var claims = new[]
        {
            // 使用者ID
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),

            // 帳號
            new Claim(
                JwtRegisteredClaimNames.UniqueName,
                user.UserName
            ),

            // 角色
            new Claim(
                ClaimTypes.Role,
                user.Role
            )
        };


        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );

        // 從 appsettings.json 取得 JWT 加密金鑰。
        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

        // 建立 Token 簽章資訊。
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );


        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}

