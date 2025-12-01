using System.Text.Json;

namespace Surugaya.Common.Models;

/// <summary>
/// 樂淘 API 回應模型
/// </summary>
public class LetaoApiResponse
{
  /// <summary>
  /// 回應代碼（200 表示成功）
  /// </summary>
  public int Code { get; set; }

  /// <summary>
  /// 回應訊息
  /// </summary>
  public string Message { get; set; } = string.Empty;
}