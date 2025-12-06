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
    /// </summary>
    public string CronExpression { get; set; } = "5 0,12 * * *";

    /// <summary>
    /// 執行超時時間 (分鐘)
    /// </summary>
    public int TimeoutMinutes { get; set; } = 60;
    
}
