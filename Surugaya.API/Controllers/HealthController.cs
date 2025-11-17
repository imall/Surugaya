using Microsoft.AspNetCore.Mvc;

namespace Surugaya.API.Controllers;

/// <summary>
/// 健康檢查控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// 健康檢查端點
    /// </summary>
    /// <returns>健康狀態</returns>
    [HttpGet]
    public IActionResult Check()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Service = "Surugaya API"
        });
    }
}
