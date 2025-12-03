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
  /// 根據多個 URL 批次取得購買紀錄
  /// </summary>
  public async Task<IEnumerable<SurugayaPurchase>> GetPurchaseByUrlsAsync(IEnumerable<string> urls)
  {
    try
    {
      var urlList = urls.ToList();
      if (!urlList.Any())
      {
        return Enumerable.Empty<SurugayaPurchase>();
      }

      var response = await supabaseClient
          .From<SurugayaPurchase>()
          .Filter("url", Constants.Operator.In, urlList)
          .Order(x => x.Date, Constants.Ordering.Descending)
          .Get();

      return response.Models;
    }
    catch (Exception ex)
    {
      throw new Exception($"批次取得購買紀錄失敗: {ex.Message}", ex);
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
  /// 根據 ID 更新購買紀錄的日期和備註
  /// </summary>
  /// <param name="id">購買紀錄 ID</param>
  /// <param name="date">購買日期 (null 表示不更新)</param>
  /// <param name="note">備註 (null 表示不更新,傳入空字串可清空備註)</param>
  public async Task<SurugayaPurchase> UpdatePurchaseByIdAsync(long id, DateTime? date, string? note)
  {
    try
    {
      // 先取得現有資料
      var existing = await supabaseClient
          .From<SurugayaPurchase>()
          .Where(x => x.Id == id)
          .Single();

      if (existing == null)
      {
        throw new Exception($"找不到 ID 為 {id} 的購買紀錄");
      }

      // 只更新有提供的欄位
      if (date.HasValue)
      {
        existing.Date = date.Value;
      }

      if (note!= null)
      {
        existing.Note = note;
      }

      // 儲存變更
      var response = await supabaseClient
          .From<SurugayaPurchase>()
          .Update(existing);

      return response.Models.First();
    }
    catch (Exception ex)
    {
      throw new Exception($"更新購買紀錄失敗: {ex.Message}", ex);
    }
  }
}
