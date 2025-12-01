namespace Surugaya.API.Configuration;

/// <summary>
/// 樂淘帳號設定
/// </summary>
public class LetaoSettings
{
    /// <summary>
    /// 登入信箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 登入密碼
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Cookie 快取時間（小時），預設 23 小時
    /// </summary>
    public int CookieCacheHours { get; set; } = 23;

    /// <summary>
    /// 加入購物車失敗時的重試次數，預設 2 次
    /// </summary>
    public int MaxRetries { get; set; } = 2;

    /// <summary>
    /// 重試間隔（毫秒），預設 1000 毫秒
    /// </summary>
    public int RetryDelayMilliseconds { get; set; } = 1000;
}
