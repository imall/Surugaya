namespace Surugaya.Common.Models;

/// <summary>
/// 新增購買紀錄請求
/// </summary>
public class AddPurchaseRequest
{
  /// <summary>
  /// 商品 URL
  /// </summary>
  public string Url { get; set; } = string.Empty;

  /// <summary>
  /// 購買日期（選填，預設為當下時間）
  /// </summary>
  public DateTime? Date { get; set; }

  /// <summary>
  /// 備註
  /// </summary>
  public string? Note { get; set; }
}


/// <summary>
/// 購買歷史項目
/// </summary>
public class PurchaseHistoryItem
{
  /// <summary>
  /// 商品 id  流水號
  /// </summary>
  public long Id { get; set; }

  /// <summary>
  /// 商品 URL 
  /// </summary>
  public string Url { get; set; } = string.Empty;

  /// <summary>
  /// 購買日期
  /// </summary>
  public DateTime Date { get; set; }

  /// <summary>
  /// 備註
  /// </summary>
  public string? Note { get; set; }
}
