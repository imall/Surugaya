using Microsoft.AspNetCore.Mvc;
using Surugaya.Common.Models;
using Surugaya.Repository.Models;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

/// <summary>
/// 處理完成的願望清單資料 
/// </summary>
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
    
    /// <summary>
    /// 編輯用途分類
    /// </summary>
    /// <param name="id"></param>
    /// <param name="purposeCategory"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}/purposeCategory")]
    public async Task<SurugayaDetailModel> UpdatePurposeCategory(int id, PurposeCategoryEnum purposeCategory)
    {
        return await service.UpdatePurposeCategoryAsync(id, purposeCategory);
    }
    
    /// <summary>
    /// 編輯作品名稱
    /// </summary>
    /// <param name="id"></param>
    /// <param name="seriesName"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}/seriesName")]
    public async Task<SurugayaDetailModel> UpdateSeriesName(int id, string seriesName)
    {
        return await service.UpdateSeriesNameAsync(id, seriesName);
    }
}
