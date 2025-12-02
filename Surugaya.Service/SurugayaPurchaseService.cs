using Microsoft.Extensions.Logging;
using Surugaya.Common.Models;
using Surugaya.Repository;
using Surugaya.Repository.Models;

namespace Surugaya.Service;

public class SurugayaPurchaseService(
    SurugayaPurchaseRepository purchaseRepository,
    ILogger<SurugayaPurchaseService> logger)
{
  /// <summary>
  /// 新增購買紀錄
  /// </summary>
  public async Task<PurchaseHistoryItem> AddPurchaseAsync(AddPurchaseRequest request)
  {
    // 建立購買紀錄
    var purchase = new SurugayaPurchaseDataModel
    {
      Url = request.Url,
      Date = request.Date ?? DateTime.UtcNow,
      Note = request.Note
    };

    // 儲存到資料庫
    var result = await purchaseRepository.InsertPurchaseAsync(purchase);

    logger.LogInformation("成功新增購買紀錄：URL={Url}, Date={Date}", request.Url, purchase.Date);

    return new PurchaseHistoryItem()
    {
      Url = result.Url,
      Date = result.Date,
      Note = result.Note
    };
  }

  /// <summary>
  /// 取得所有購買紀錄
  /// </summary>
  public async Task<IEnumerable<PurchaseHistoryItem>> GetAllPurchasesAsync()
  {
    var purchases = await purchaseRepository.GetAllPurchasesAsync();

    return purchases.Select(p => new PurchaseHistoryItem
    {
      Id = p.Id,
      Url = p.Url,
      Date = p.Date,
      Note = p.Note
    });
  }

  /// <summary>
  /// 根據 URL 取得購買紀錄
  /// </summary>
  public async Task<IEnumerable<PurchaseHistoryItem>> GetPurchaseByUrlAsync(string url)
  {
    var purchase = await purchaseRepository.GetPurchaseByUrlAsync(url);

    return purchase.Select(p => new PurchaseHistoryItem
    {
      Id = p.Id,
      Url = p.Url,
      Date = p.Date,
      Note = p.Note
    });
  }

  /// <summary>
  /// 刪除購買紀錄
  /// </summary>
  public async Task<bool> DeletePurchaseAsync(long id)
  {
    try
    {
      await purchaseRepository.DeletePurchaseAsync(id);
      return true;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "刪除購買紀錄失敗：{Message}", ex.Message);
      return false;
    }
  }

  /// <summary>
  /// 根據 ID 更新購買紀錄的日期和備註 (支援部分更新)
  /// </summary>
  public async Task<PurchaseHistoryItem> UpdatePurchaseByIdAsync(long id, UpdatePurchaseRequest request)
  {
    try
    {
      
      var result = await purchaseRepository.UpdatePurchaseByIdAsync(id, request.Date, request.Note);

      return new PurchaseHistoryItem
      {
        Id = result.Id,
        Url = result.Url,
        Date = result.Date,
        Note = result.Note
      };
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "更新購買紀錄失敗：{Message}", ex.Message);
      throw;
    }
  }
}
