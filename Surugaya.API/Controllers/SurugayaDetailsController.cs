using Microsoft.AspNetCore.Mvc;
using Surugaya.Common.Models;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SurugayaDetailsController(SurugayaDetailsService service, ILogger<SurugayaController> logger) : ControllerBase
{

    /// <summary>
    /// 取得所有清單
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SurugayaDetailModel>>> GetAllInUrlAsync()
    {
        try
        {
            var result = await service.GetAllInUrlAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "取得 SurugayaDetailModel 資料時發生錯誤");
            return StatusCode(500, new { error = "取得資料時發生內部錯誤", message = ex.Message });
        }
    }
}
