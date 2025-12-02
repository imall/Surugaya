using Supabase;
using Supabase.Postgrest;
using Surugaya.Repository.Models;
using Client = Supabase.Client;

namespace Surugaya.Repository;

public class SurugayaPurchaseRepository(Client supabaseClient)
{
  /// <summary>
  /// 新增購買紀錄
  /// </summary>
  public async Task<SurugayaPurchaseDataModel> InsertPurchaseAsync(SurugayaPurchaseDataModel purchase)
  {
    try
    {
      var response = await supabaseClient
          .From<SurugayaPurchaseDataModel>()
          .Insert(purchase);

      return response.Model!;
    }
    catch (Exception ex)
    {
      throw new Exception($"插入購買紀錄失敗: {ex.Message}", ex);
    }
  }

  /// <summary>
  /// 取得所有購買紀錄
  /// </summary>
  public async Task<IEnumerable<SurugayaPurchase>> GetAllPurchasesAsync()
  {
    try
    {
      var response = await supabaseClient
          .From<SurugayaPurchase>()
          .Order(x => x.Date, Constants.Ordering.Descending)
          .Get();

      return response.Models;
    }
    catch (Exception ex)
    {
      throw new Exception($"取得購買紀錄失敗: {ex.Message}", ex);
    }
  }

  /// <summary>
  /// 根據 URL 取得購買紀錄
  /// </summary>
  public async Task<IEnumerable<SurugayaPurchase>> GetPurchaseByUrlAsync(string url)
  {
    try
    {
      var response = await supabaseClient
          .From<SurugayaPurchase>()
          .Where(x => x.Url == url)
          .Get();

      return response.Models;
    }
    catch (Exception ex)
    {
      throw new Exception($"取得購買紀錄失敗: {ex.Message}", ex);
    }
  }

  /// <summary>
  /// 刪除購買紀錄
  /// </summary>
  public async Task DeletePurchaseAsync(long id)
  {
    try
    {
      await supabaseClient
          .From<SurugayaPurchase>()
          .Where(x => x.Id == id)
          .Delete();
    }
    catch (Exception ex)
    {
      throw new Exception($"刪除購買紀錄失敗: {ex.Message}", ex);
    }
  }

  /// <summary>
  /// 更新購買紀錄
  /// </summary>
  public async Task<SurugayaPurchase> UpdatePurchaseAsync(SurugayaPurchase purchase)
  {
    try
    {
      var response = await supabaseClient
          .From<SurugayaPurchase>()
          .Update(purchase);

      return response.Models.First();
    }
    catch (Exception ex)
    {
      throw new Exception($"更新購買紀錄失敗: {ex.Message}", ex);
    }
  }
}
