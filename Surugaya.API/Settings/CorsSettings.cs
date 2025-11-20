namespace Surugaya.API.Settings;

/// <summary>
/// CORS 設定
/// </summary>
public class CorsSettings
{
  /// <summary>
  /// 是否允許所有來源
  /// </summary>
  public bool AllowAllOrigins { get; set; } = true;

  /// <summary>
  /// 允許的來源清單 (當 AllowAllOrigins 為 false 時使用)
  /// </summary>
  public string[] AllowedOrigins { get; set; } = Array.Empty<string>();

  /// <summary>
  /// 是否允許認證
  /// </summary>
  public bool AllowCredentials { get; set; } = false;

  /// <summary>
  /// 允許的方法 (空陣列表示允許所有方法)
  /// </summary>
  public string[] AllowedMethods { get; set; } = Array.Empty<string>();

  /// <summary>
  /// 允許的標頭 (空陣列表示允許所有標頭)
  /// </summary>
  public string[] AllowedHeaders { get; set; } = Array.Empty<string>();

  /// <summary>
  /// 預檢快取時間 (秒)
  /// </summary>
  public int PreflightMaxAgeSeconds { get; set; } = 86400; // 24 小時
}
