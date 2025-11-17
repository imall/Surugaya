namespace Surugaya.API.Settings;

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
    /// 執行時間設定 (24小時制，格式: HH:mm)
    /// 預設為 00:00, 06:00, 12:00, 18:00
    /// </summary>
    public string[] ScheduledTimes { get; set; } = 
    {
        "00:00",
        "06:00", 
        "12:00",
        "18:00"
    };

    /// <summary>
    /// 錯誤重試間隔 (分鐘)
    /// </summary>
    public int ErrorRetryIntervalMinutes { get; set; } = 5;

    /// <summary>
    /// 執行超時時間 (分鐘)
    /// </summary>
    public int TimeoutMinutes { get; set; } = 60;
}
