using Microsoft.AspNetCore.Mvc;

namespace Training.RepairSystem.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    [HttpGet("/error")]
    public IActionResult Error()
    {
        return Problem(
            title: "系統發生錯誤",
            detail: "請稍後再試",
            statusCode: 500
        );
    }
}