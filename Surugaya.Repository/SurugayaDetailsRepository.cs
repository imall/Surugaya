using Supabase.Postgrest;
using Surugaya.Common.Models;
using Surugaya.Repository.Models;
using Client = Supabase.Client;

namespace Surugaya.Repository;

public class SurugayaDetailsRepository(Client supabaseClient)
{
    /// <summary>
    /// 處理多筆資料的插入或更新
    /// </summary>
    /// <param name="products"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IEnumerable<SurugayaDetailDataModel>> InsertOrUpdateSurugayaAsync(IEnumerable<SurugayaDetailDataModel> products)
    {
        try
        {
            var response = await supabaseClient
                .From<SurugayaDetailDataModel>()
                .Upsert(products.ToList(), new QueryOptions
                {
                    OnConflict = "url" // 以 url 作為衝突判斷依據
                });

            return response.Models;
        }
        catch (Exception ex)
        {
            throw new Exception($"插入或更新 SurugayaDetailDataModel 資料失敗: {ex.Message}", ex);
        }
    }


    /// <summary>
    /// 新增單筆清單資料
    /// </summary>
    /// <param name="products"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<SurugayaDetailDataModel> InsertOrUpdateSurugayaAsync(SurugayaDetailDataModel products)
    {
        try
        {
            var response = await supabaseClient
                .From<SurugayaDetailDataModel>()
                .Upsert(products);

            return response.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception($"插入或更新 SurugayaDetailDataModel 資料失敗: {ex.Message}", ex);
        }
    }


    /// <summary>
    /// 取得 urls 清單的所有細節資料
    /// </summary>
    /// <param name="urls"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IEnumerable<SurugayaDetailDataModel>> GetAllInUrlAsync(IEnumerable<string> urls)
    {
        try
        {
            var urlList = urls.ToList();

            var response = await supabaseClient
                .From<SurugayaDetailDataModel>()
                .Filter("url", Constants.Operator.In, urlList)
                .Get();

            return response.Models;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得 SurugayaDetailDataModel 資料失敗: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 依照 url 刪除資料
    /// </summary>
    /// <param name="id">商品 Id</param>
    /// <exception cref="Exception"></exception>
    public async Task DeleteFromIdAsync(int id)
    {
        try
        {
            var url = $"{ProjectConst.BaseUrl}/{id}";
            
            await supabaseClient
                .From<SurugayaDetailDataModel>()
                .Where(x => x.Url.Contains(url))
                .Delete();
        }
        catch (Exception ex)
        {
            throw new Exception($"根據 ID 刪除 SurugayaDetailDataModel 資料失敗: {ex.Message}", ex);
        }
    }
}
