using Microsoft.AspNetCore.Mvc;
using Surugaya.Common.Models;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

/// <summary>
/// 樂淘購物車管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LetaoCartController : ControllerBase
{
    private readonly ILogger<LetaoCartController> _logger;
    private readonly LetaoCartService _letaoCartService;

    /// <summary>
    /// 建構函式
    /// </summary>
    public LetaoCartController(
        ILogger<LetaoCartController> logger,
        LetaoCartService letaoCartService)
    {
        _logger = logger;
        _letaoCartService = letaoCartService;
    }

    /// <summary>
    /// 加入商品到樂淘購物車
    /// </summary>
    /// <param name="request">加入購物車請求參數</param>
    /// <returns>加入購物車結果</returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(LetaoAddToCartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddToCart([FromBody] LetaoAddToCartRequest request)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(request.Url))
            {
                return BadRequest(new { message = "商品 URL 為必填欄位" });
            }

            // 驗證 ImageUrl（可能是由 ProductId 自動生成或手動提供的）
            if (string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                return BadRequest(new { message = "請提供 ProductId 或 ImageUrl" });
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest(new { message = "商品標題為必填欄位" });
            }

            if (string.IsNullOrWhiteSpace(request.UnitPrice))
            {
                return BadRequest(new { message = "商品單價為必填欄位" });
            }

            // 設定預設數量
            if (string.IsNullOrWhiteSpace(request.Quantity))
            {
                request.Quantity = "1";
            }

            _logger.LogInformation("收到加入購物車請求：{Title}", request.Title);

            // 執行加入購物車
            var result = await _letaoCartService.AddToCartAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(result.StatusCode, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加入購物車時發生未預期的錯誤");
            return StatusCode(500, new LetaoAddToCartResponse
            {
                Success = false,
                Message = $"加入購物車時發生未預期的錯誤：{ex.Message}",
                StatusCode = 500
            });
        }
    }

    /// <summary>
    /// 清除登入快取（用於測試或強制重新登入）
    /// </summary>
    [HttpPost("clear-cache")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult ClearCache([FromServices] LetaoAuthService authService)
    {
        authService.ClearCache();
        return Ok(new { message = "已清除登入快取" });
    }
}
