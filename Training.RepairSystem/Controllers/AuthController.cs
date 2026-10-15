using Microsoft.AspNetCore.Mvc;
using Training.RepairSystem.Models.DTOs;
using Training.RepairSystem.Services.Interfaces;

namespace Training.RepairSystem.Controllers;

// 標記這是一個 Web API Controller。
[ApiController]
// 設定 API 路由。
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // 宣告一個私有、唯讀的帳號服務物件。
    private readonly IAccountService _accountService;
    // 建構子注入。
    public AuthController(
        IAccountService accountService
    )
    {
        _accountService = accountService;
    }
    // 設定這個方法處理 POST 請求。
    [HttpPost("login")]
    public async Task<IActionResult> Login(
           LoginRequest request
       )
    {
        // 呼叫 AccountService 的登入功能。
        var result =
            await _accountService.LoginAsync(request);

        // 如果登入失敗：
        if (result == null)
        {
            return Unauthorized();
        }

        // 登入成功。
        return Ok(result);
    }
}