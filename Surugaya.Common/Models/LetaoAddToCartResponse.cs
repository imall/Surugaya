namespace Surugaya.Common.Models;

/// <summary>
/// 加入購物車 API 回應
/// </summary>
public class LetaoAddToCartResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// HTTP 狀態碼
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// API 原始回應
    /// </summary>
    public string? RawResponse { get; set; }
}
