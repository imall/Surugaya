namespace Surugaya.API.Configuration;

/// <summary>
/// 駿河屋爬蟲排程設定
/// </summary>
public class SurugayaScraperSettings
{
    /// <summary>
    /// 是否啟用背景排程
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Cron 表達式 (用於 Hangfire)
    /// 預設為每天 11:00 開始,每 6 小時執行一次 (11:00, 17:00, 23:00, 05:00)
    /// 範例:
    /// - "0 11,17,23,5 * * *" : 每天 11:00, 17:00, 23:00, 05:00 執行
    /// - "0 */6 * * *" : 每 6 小時執行一次 (從 00:00 開始)
    /// - "0 0,6,12,18 * * *" : 每天 00:00, 06:00, 12:00, 18:00 執行
    /// - "0 0 * * *" : 每天午夜執行
    /// </summary>
    public string CronExpression { get; set; } = "0 11,17,23,5 * * *";

    /// <summary>
    /// 執行超時時間 (分鐘)
    /// </summary>
    public int TimeoutMinutes { get; set; } = 60;
    
}
