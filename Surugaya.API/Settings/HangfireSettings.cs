namespace Surugaya.API.Settings;

/// <summary>
/// Hangfire 設定
/// </summary>
public class HangfireSettings
{
    /// <summary>
    /// 伺服器名稱
    /// </summary>
    /// <remarks>識別執行 Hangfire 工作的伺服器</remarks>
    public string ServerName { get; set; } = string.Empty;

    /// <summary>
    /// 工作執行緒數量
    /// </summary>
    /// <remarks>設定同時可以執行多少個背景工作</remarks>
    public int WorkerCount { get; set; } = 10;

    /// <summary>
    /// 工作佇列
    /// </summary>
    /// <remarks>定義工作的優先順序和分類</remarks>
    public string[] Queues { get; set; } = new List<string>().ToArray();

    /// <summary>
    /// 資料庫 Schema 名稱
    /// </summary>
    public string SchemaName { get; set; } = "hangfire";
}
