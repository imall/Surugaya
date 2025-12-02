using Microsoft.AspNetCore.Mvc;
using Surugaya.Common.Models;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

/// <summary>
/// Surugaya 購買紀錄管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SurugayaPurchaseController : ControllerBase
{
    private readonly ILogger<SurugayaPurchaseController> _logger;
    private readonly SurugayaPurchaseService _purchaseService;

    /// <summary>
    /// 建構函式
    /// </summary>
    public SurugayaPurchaseController(
        ILogger<SurugayaPurchaseController> logger,
        SurugayaPurchaseService purchaseService)
    {
        _logger = logger;
        _purchaseService = purchaseService;
    }

    /// <summary>
    /// 新增購買紀錄
    /// </summary>
    /// <param name="request">購買紀錄請求</param>
    /// <returns>購買紀錄回應</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseHistoryItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddPurchase([FromBody] AddPurchaseRequest request)
    {
        // 驗證請求
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest(new { success = false, message = "商品 URL 為必填欄位" });
        }

        _logger.LogInformation("收到新增購買紀錄請求：URL={Url}, Date={Date}, Note={Note}",
            request.Url, request.Date, request.Note);

        var result = await _purchaseService.AddPurchaseAsync(request);

        return Ok(result);
    }

    /// <summary>
    /// 取得所有購買紀錄
    /// </summary>
    /// <returns>購買紀錄清單</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PurchaseHistoryItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllPurchases()
    {
        var result = await _purchaseService.GetAllPurchasesAsync();

        return Ok(result);
    }

    /// <summary>
    /// 根據 URL 取得購買紀錄
    /// </summary>
    /// <param name="url">商品 URL</param>
    /// <returns>購買紀錄</returns>
    [HttpGet("by-url")]
    [ProducesResponseType(typeof(IEnumerable<PurchaseHistoryItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPurchaseByUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest(new { success = false, message = "商品 URL 為必填欄位" });
        }
        
        var result = await _purchaseService.GetPurchaseByUrlAsync(url);
        
        return Ok(result);
    }

    /// <summary>
    /// 刪除購買紀錄
    /// </summary>
    /// <param name="id">商品 id</param>
    /// <returns>刪除結果</returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletePurchase(long id)
    {
        try
        {
            var result = await _purchaseService.DeletePurchaseAsync(id);

            if (result)
            {
                return Ok(new { success = true, message = "購買紀錄已成功刪除" });
            }
            else
            {
                return NotFound(new { success = false, message = "找不到指定的購買紀錄" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刪除購買紀錄時發生未預期的錯誤");
            return StatusCode(500, new
            {
                success = false,
                message = $"刪除購買紀錄時發生未預期的錯誤：{ex.Message}"
            });
        }
    }
}
