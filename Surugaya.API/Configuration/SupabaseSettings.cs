namespace Surugaya.API.Configuration;

public class SupabaseSettings
{
    public string Url { get; set; } = string.Empty;
    public string AnonKey { get; set; } = string.Empty;
    public string? ServiceKey { get; set; }
        
    /// <summary>
    /// 取得用於 API 操作的金鑰，優先使用服務金鑰
    /// </summary>
    public string GetApiKey() => !string.IsNullOrEmpty(ServiceKey) ? ServiceKey : AnonKey;
}