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
    /// 預設為每 6 小時執行一次，從 12:03 開始 (12:03, 18:03, 00:03, 06:03)
    /// </summary>
    public string CronExpression { get; set; } = "3 0/6 * * *";

    /// <summary>
    /// 執行超時時間 (分鐘)
    /// </summary>
    public int TimeoutMinutes { get; set; } = 60;
    
}
