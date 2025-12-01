using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Surugaya.Common.Models;

namespace Surugaya.Service;

/// <summary>
/// 樂淘認證服務（處理登入和 Cookie 快取）
/// </summary>
public class LetaoAuthService(
  ILogger<LetaoAuthService> logger,
  LetaoSettings settings)
{
  // 全域變數：快取的 Cookie
  private static CookieContainer? _cachedCookies = null;
  private static DateTime _cookiesExpireTime = DateTime.MinValue;
  private static readonly object _lockObject = new object();

  /// <summary>
  /// 取得有效的 Cookie（自動處理快取和重新登入）
  /// </summary>
  public async Task<CookieContainer> GetValidCookieAsync()
  {
    lock (_lockObject)
    {
      // 檢查快取是否有效
      if (_cachedCookies != null && DateTime.Now < _cookiesExpireTime)
      {
        logger.LogInformation("✅ 使用快取的 Cookie");
        return _cachedCookies;
      }
    }

    // Cookie 不存在或已過期，重新登入
    logger.LogInformation("🔑 Cookie 不存在或已過期，重新登入...");
    var cookieContainer = await LoginAndGetCookiesAsync();

    lock (_lockObject)
    {
      _cachedCookies = cookieContainer;
      _cookiesExpireTime = DateTime.Now.AddHours(settings.CookieCacheHours);
    }

    return cookieContainer;
  }

  /// <summary>
  /// 清除快取的 Cookie
  /// </summary>
  public void ClearCache()
  {
    lock (_lockObject)
    {
      _cachedCookies = null;
      _cookiesExpireTime = DateTime.MinValue;
      logger.LogInformation("🗑️ 已清除 Cookie 快取");
    }
  }

  /// <summary>
  /// 登入並取得 Cookies
  /// </summary>
  private async Task<CookieContainer> LoginAndGetCookiesAsync()
  {
    var cookieContainer = new CookieContainer();
    var handler = new HttpClientHandler
    {
      CookieContainer = cookieContainer,
      UseCookies = true,
      AllowAutoRedirect = true
    };

    using var client = new HttpClient(handler);
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

    // 1. 先訪問登入頁面
    await client.GetAsync("https://www.letao.com.tw/login.php");
    await Task.Delay(500);

    // 2. 準備登入請求
    client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
    client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
    client.DefaultRequestHeaders.Add("Origin", "https://www.letao.com.tw");
    client.DefaultRequestHeaders.Add("Referer", "https://www.letao.com.tw/login.php");

    var loginDataJson = new
    {
      email = settings.Email,
      password = settings.Password,
      persistent = 1
    };

    var loginDataJsonString = JsonSerializer.Serialize(loginDataJson);

    var formData = new Dictionary<string, string>
        {
            { "action", "loginByEmail" },
            { "data", loginDataJsonString }
        };

    // 3. 發送登入請求
    var response = await client.PostAsync(
        "https://www.letao.com.tw/config/login_api.php",
        new FormUrlEncodedContent(formData)
    );

    var result = await response.Content.ReadAsStringAsync();

    // 4. 解析登入回應
    var loginResponse = JsonSerializer.Deserialize<LetaoLoginResponse>(result, new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    });

    if (loginResponse?.Code != "200")
    {
      logger.LogError("❌ 登入失敗: {Result}", result);
      throw new Exception($"登入失敗: {result}");
    }

    var guid = loginResponse.Details?.Data?.Guid;
    var guidExpired = loginResponse.Details?.Data?.GuidExpired;

    if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(guidExpired))
    {
      logger.LogError("❌ 登入回應缺少 GUID 或過期時間");
      throw new Exception("登入回應格式錯誤");
    }

    logger.LogInformation("✅ 登入成功！GUID: {Guid}", guid);

    // 5. 手動設定 GUID Cookie
    var uri = new Uri("https://www.letao.com.tw");
    var guidCookie = new Cookie("guid", guid)
    {
      Domain = ".letao.com.tw",
      Path = "/",
      Expires = DateTime.Parse(guidExpired)
    };

    cookieContainer.Add(uri, guidCookie);

    return cookieContainer;
  }
}
