using Microsoft.AspNetCore.Mvc;
using Surugaya.Common.Models;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

/// <summary>
/// 爬蟲 api
/// </summary>
/// <param name="service"></param>
[ApiController]
[Route("api/[controller]")]
public class SurugayaScraperController(SurugayaService service,SurugayaScraperService scraperService) : ControllerBase
{
    
    /// <summary>
    /// 爬取所有 Surugaya 商品資訊，並寫入資料庫
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
