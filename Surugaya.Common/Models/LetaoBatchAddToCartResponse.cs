namespace Surugaya.Common.Models;

/// <summary>
/// 批次加入購物車回應
/// </summary>
public class LetaoBatchAddToCartResponse
{
    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 成功數量
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失敗數量
    /// </summary>
    public int FailedCount { get; set; }

    /// <summary>
    /// 每筆商品的結果
    /// </summary>
    public List<LetaoAddToCartItemResult> Results { get; set; } = new();

    /// <summary>
    /// 整體是否成功（所有項目都成功）
    /// </summary>
    public bool AllSuccess => FailedCount == 0;
}

/// <summary>
/// 單筆商品加入購物車的結果
/// </summary>
public class LetaoAddToCartItemResult
{
    /// <summary>
    /// 商品標題
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 商品 URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

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
}
