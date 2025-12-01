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
  public async Task<PurchaseHistoryResponse> AddPurchaseAsync(AddPurchaseRequest request)
  {
    try
    {
      // 建立購買紀錄
      var purchase = new SurugayaPurchase
      {
        Url = request.Url,
        Date = request.Date ?? DateTime.UtcNow,
        Note = request.Note
      };

      // 儲存到資料庫
      var result = await purchaseRepository.InsertPurchaseAsync(purchase);

      logger.LogInformation("成功新增購買紀錄：URL={Url}, Date={Date}", request.Url, purchase.Date);

      // 取得所有購買紀錄
      var allPurchases = await purchaseRepository.GetAllPurchasesAsync();

      return new PurchaseHistoryResponse
      {
        Success = true,
        Message = "購買紀錄已成功新增",
        PurchaseHistory = allPurchases.Select(p => new PurchaseHistoryItem
        {
          Id = p.Id,
          Url = p.Url,
          Date = p.Date,
          Note = p.Note
        })
              .ToList()
      };
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "新增購買紀錄失敗：{Message}", ex.Message);
      return new PurchaseHistoryResponse
      {
        Success = false,
        Message = $"新增購買紀錄失敗：{ex.Message}"
      };
    }
  }

  /// <summary>
  /// 取得所有購買紀錄
  /// </summary>
  public async Task<PurchaseHistoryResponse> GetAllPurchasesAsync()
  {
    try
    {
      var purchases = await purchaseRepository.GetAllPurchasesAsync();

      return new PurchaseHistoryResponse
      {
        Success = true,
        PurchaseHistory = purchases.Select(p => new PurchaseHistoryItem
        {
          Id = p.Id,
          Url = p.Url,
          Date = p.Date,
          Note = p.Note
        })
              .ToList()
      };
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "取得購買紀錄失敗：{Message}", ex.Message);
      return new PurchaseHistoryResponse
      {
        Success = false,
        Message = $"取得購買紀錄失敗：{ex.Message}"
      };
    }
  }

  /// <summary>
  /// 根據 URL 取得購買紀錄
  /// </summary>
  public async Task<PurchaseHistoryResponse> GetPurchaseByUrlAsync(string url)
  {
    try
    {
      var purchase = await purchaseRepository.GetPurchaseByUrlAsync(url);

      if (purchase == null)
      {
        return new PurchaseHistoryResponse
        {
          Success = false,
          Message = "找不到該商品的購買紀錄"
        };
      }

      return new PurchaseHistoryResponse
      {
        Success = true,
        PurchaseHistory = new List<PurchaseHistoryItem>
                {
                    new()
                    {
                        Id = purchase.Id,
                        Url = purchase.Url,
                        Date = purchase.Date,
                        Note = purchase.Note
                    }
                }
      };
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "取得購買紀錄失敗：{Message}", ex.Message);
      return new PurchaseHistoryResponse
      {
        Success = false,
        Message = $"取得購買紀錄失敗：{ex.Message}"
      };
    }
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
}
