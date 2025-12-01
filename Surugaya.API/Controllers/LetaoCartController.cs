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
  /// 加入商品到樂淘購物車（支援單筆或多筆）
  /// </summary>
  /// <param name="requests">加入購物車請求參數（陣列）</param>
  /// <returns>加入購物車結果</returns>
  [HttpPost("add")]
  [ProducesResponseType(typeof(LetaoBatchAddToCartResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public async Task<IActionResult> AddToCart([FromBody] IEnumerable<LetaoAddToCartRequest> requests)
  {
    try
    {
      var requestList = requests?.ToList();

      // 驗證請求是否為空
      if (requestList == null || !requestList.Any())
      {
        return BadRequest(new { message = "請求不能為空" });
      }

      _logger.LogInformation("收到批次加入購物車請求，共 {Count} 筆商品", requestList.Count);

      var response = new LetaoBatchAddToCartResponse
      {
        TotalCount = requestList.Count
      };

      // 逐筆處理每個商品
      foreach (var request in requestList)
      {
        var itemResult = new LetaoAddToCartItemResult
        {
          Title = request.Title,
          Url = request.Url
        };

        try
        {
          // 驗證必填欄位
          var validationError = ValidateRequest(request);
          if (validationError != null)
          {
            itemResult.Success = false;
            itemResult.Message = validationError;
            itemResult.StatusCode = 400;
            response.Results.Add(itemResult);
            response.FailedCount++;
            _logger.LogWarning("商品 {Title} 驗證失敗: {Error}", request.Title, validationError);
            continue;
          }
          
          _logger.LogInformation("正在加入商品：{Title}", request.Title);

          // 執行加入購物車
          var result = await _letaoCartService.AddToCartAsync(request);

          itemResult.Success = result.Success;
          itemResult.Message = result.Message;
          itemResult.StatusCode = result.StatusCode;

          if (result.Success)
          {
            response.SuccessCount++;
            _logger.LogInformation("✅ 商品 {Title} 加入成功", request.Title);
          }
          else
          {
            response.FailedCount++;
            _logger.LogWarning("❌ 商品 {Title} 加入失敗: {Message}", request.Title, result.Message);
          }

          response.Results.Add(itemResult);
        }
        catch (Exception ex)
        {
          itemResult.Success = false;
          itemResult.Message = $"處理時發生錯誤：{ex.Message}";
          itemResult.StatusCode = 500;
          response.Results.Add(itemResult);
          response.FailedCount++;
          _logger.LogError(ex, "處理商品 {Title} 時發生錯誤", request.Title);
        }
      }

      _logger.LogInformation("批次處理完成：總數 {Total}，成功 {Success}，失敗 {Failed}",
          response.TotalCount, response.SuccessCount, response.FailedCount);

      // 如果有任何失敗，回傳 BadRequest (400)，否則回傳 Ok (200)
      if (response.AllSuccess)
      {
        return Ok(response);
      }
      else
      {
        return BadRequest(response);
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "批次加入購物車時發生未預期的錯誤");
      return StatusCode(500, new
      {
        success = false,
        message = $"批次加入購物車時發生未預期的錯誤：{ex.Message}"
      });
    }
  }

  /// <summary>
  /// 驗證請求參數
  /// </summary>
  private string? ValidateRequest(LetaoAddToCartRequest request)
  {
    if (string.IsNullOrWhiteSpace(request.Url))
    {
      return "商品 URL 為必填欄位";
    }

    if (string.IsNullOrWhiteSpace(request.ImageUrl))
    {
      return "請提供 ProductId 或 ImageUrl";
    }

    if (string.IsNullOrWhiteSpace(request.Title))
    {
      return "商品標題為必填欄位";
    }

    if (string.IsNullOrWhiteSpace(request.UnitPrice))
    {
      return "商品單價為必填欄位";
    }

    return null;
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
