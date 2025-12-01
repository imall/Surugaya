namespace Surugaya.Common.Models;

/// <summary>
/// 加入樂淘購物車請求參數
/// </summary>
public class LetaoAddToCartRequest
{
  private string? _productId;
  private string _imageUrl = string.Empty;

  /// <summary>
  /// 商品 URL
  /// </summary>
  public string Url { get; set; } = string.Empty;

  /// <summary>
  /// 商品 ID（設定後會自動生成圖片 URL）
  /// </summary>
  public string? ProductId
  {
    get => _productId;
    set
    {
      _productId = value;
      if (!string.IsNullOrWhiteSpace(value))
      {
        _imageUrl = $"https://cdn.suruga-ya.jp/database/pics_light/game/{value}.jpg";
      }
    }
  }

  /// <summary>
  /// 商品圖片 URL（可手動設定，或由 ProductId 自動生成）
  /// </summary>
  public string? ImageUrl
  {
    get => _imageUrl;
    set => _imageUrl = value;
  }

  /// <summary>
  /// 商品標題
  /// </summary>
  public string Title { get; set; } = string.Empty;

  /// <summary>
  /// 商品規格（選填）
  /// </summary>
  public string? Spec { get; set; } = string.Empty;

  /// <summary>
  /// 單價
  /// </summary>
  public string UnitPrice { get; set; } = string.Empty;

  /// <summary>
  /// 數量（預設為 1）
  /// </summary>
  public string? Quantity { get; set; } = "1";

  /// <summary>
  /// 備註（選填）
  /// </summary>
  public string? Comment { get; set; } = string.Empty;
}
