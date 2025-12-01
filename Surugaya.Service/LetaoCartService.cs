using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Surugaya.Common.Models;

namespace Surugaya.Service;

/// <summary>
/// 樂淘購物車服務
/// </summary>
public class LetaoCartService(
  ILogger<LetaoCartService> logger,
  LetaoSettings settings,
  LetaoAuthService authService)
{
  /// <summary>
  /// 智能加入購物車（自動處理失效 Cookie）
  /// </summary>
  public async Task<LetaoAddToCartResponse> AddToCartAsync(LetaoAddToCartRequest request)
  {
    for (int attempt = 1; attempt <= settings.MaxRetries; attempt++)
    {
      try
      {
        logger.LogInformation("🔄 嘗試第 {Attempt} 次加入購物車...", attempt);

        // 取得有效的 Cookie（自動處理登入和快取）
        var cookieContainer = await authService.GetValidCookieAsync();

        // 執行加入購物車
        var result = await ExecuteAddToCartAsync(request, cookieContainer);

        // 檢查是否因為 Cookie 失效而失敗
        if (IsSessionExpired(result))
        {
          logger.LogWarning("⚠️ 第 {Attempt} 次嘗試失敗：Session 已失效", attempt);

          // 清除快取，強制下次重新登入
          authService.ClearCache();

          if (attempt >= settings.MaxRetries)
            return new LetaoAddToCartResponse
            {
              Success = false,
              Message = "Session 已失效，已達最大重試次數",
              StatusCode = result.StatusCode,
              RawResponse = result.RawResponse
            };
          
          logger.LogInformation("🔄 準備重試...");
          
          await Task.Delay(settings.RetryDelayMilliseconds);
          continue;

        }

        // 成功
        if (result.Success)
        {
          logger.LogInformation("✅ 成功加入購物車！");
          return result;
        }

        // 其他失敗原因
        logger.LogWarning("❌ 加入購物車失敗：{Message}", result.Message);
        return result;
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "❌ 第 {Attempt} 次嘗試發生錯誤", attempt);

        if (attempt >= settings.MaxRetries)
          return new LetaoAddToCartResponse
          {
            Success = false,
            Message = $"加入購物車發生錯誤：{ex.Message}",
            StatusCode = 500
          };
        // 清除快取
        authService.ClearCache();
        await Task.Delay(settings.RetryDelayMilliseconds);
      }
    }

    return new LetaoAddToCartResponse
    {
      Success = false,
      Message = "已達最大重試次數",
      StatusCode = 500
    };
  }

  /// <summary>
  /// 執行加入購物車
  /// </summary>
  private async Task<LetaoAddToCartResponse> ExecuteAddToCartAsync(
      LetaoAddToCartRequest request,
      CookieContainer cookieContainer)
  {
    var handler = new HttpClientHandler
    {
      CookieContainer = cookieContainer,
      UseCookies = true
    };

    using var client = new HttpClient(handler);
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
    client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
    client.DefaultRequestHeaders.Add("Origin", "https://www.letao.com.tw");
    client.DefaultRequestHeaders.Add("Referer", "https://www.letao.com.tw/jpshopping/ext_shop_left.php");

    // 清理價格格式（移除逗號和空格）
    var cleanedPrice = request.UnitPrice?.Replace(",", "").Replace(" ", "").Trim() ?? "0";

    var formData = new Dictionary<string, string>
        {
            { "save", "add" },
            { "url", request.Url },
            { "img", request.ImageUrl },
            { "title", request.Title },
            { "spec", request.Spec },
            { "unit_price", cleanedPrice },
            { "quantity", request.Quantity },
            { "comment", request.Comment }
        };

    var response = await client.PostAsync(
        "https://www.letao.com.tw/jpshopping/cart.php",
        new FormUrlEncodedContent(formData)
    );

    var apiResult = await response.Content.ReadAsStringAsync();
    var decoded = System.Text.RegularExpressions.Regex.Unescape(apiResult);

    logger.LogDebug("HTTP Status Code: {StatusCode}", response.StatusCode);
    logger.LogDebug("API Response: {Response}", decoded);

    return new LetaoAddToCartResponse
    {
      Success = response.IsSuccessStatusCode,
      Message = response.IsSuccessStatusCode ? "加入購物車成功" : "加入購物車失敗",
      StatusCode = (int)response.StatusCode,
      RawResponse = decoded
    };
  }

  /// <summary>
  /// 判斷 Session 是否失效
  /// </summary>
  private bool IsSessionExpired(LetaoAddToCartResponse response)
  {
    // 1. HTTP 狀態碼檢查
    if (response.StatusCode == 401 || response.StatusCode == 403)
    {
      logger.LogWarning("❌ HTTP 狀態碼顯示未授權");
      return true;
    }

    var content = response.RawResponse ?? string.Empty;

    // 2. 回應內容關鍵字檢查
    if (content.Contains("未登入") ||
        content.Contains("請先登入") ||
        content.Contains("登入逾時") ||
        content.Contains("請重新登入"))
    {
      logger.LogWarning("❌ 回應內容包含登入失效關鍵字");
      return true;
    }

    // 3. JSON 回應檢查
    try
    {
      var json = JsonDocument.Parse(content);

      if (json.RootElement.TryGetProperty("code", out var codeElement))
      {
        var code = codeElement.GetString();

        // 假設 code 不是 200 或 0 就是失敗
        if (code != "200" && code != "0")
        {
          logger.LogWarning("❌ API 回傳失敗代碼: {Code}", code);

          // 嘗試取得錯誤訊息
          if (json.RootElement.TryGetProperty("message", out var msgElement))
          {
            logger.LogWarning("   錯誤訊息: {Message}", msgElement.GetString());
          }

          return true;
        }
      }
    }
    catch
    {
      // JSON 解析失敗，可能被重導向到 HTML 頁面
      if (content.Contains("<html") || content.Contains("<!DOCTYPE"))
      {
        logger.LogWarning("❌ 回應是 HTML 頁面（可能被重導向到登入頁）");
        return true;
      }
    }

    return false;
  }
}
