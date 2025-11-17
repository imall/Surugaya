using Microsoft.AspNetCore.Mvc;
using Surugaya.Common.Models;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

/// <summary>
/// 整理詳細資料
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SurugayaScraperController(SurugayaScraperService scraperService) : ControllerBase
{
    
    /// <summary>
    /// 讀取已記錄的願望清單 url ，並寫入資料庫
    /// </summary>
    /// <returns>商品資訊列表</returns>
    [HttpGet("scrape-products")]
    public async Task<ActionResult<IEnumerable<SurugayaDetailModel>>> ScrapeProductInfo()
    {
        try
        {
            var products = await scraperService.ScrapeAllProductInfo();
            return Ok(products);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "爬取商品資訊時發生內部錯誤", message = ex.Message });
        }
    }
   
}
