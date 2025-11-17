using Supabase;
using Surugaya.Common.Models;
using Surugaya.Repository.Models;

namespace Surugaya.Repository;

public class SurugayaRepository(Client supabaseClient)
{
    public async Task<SurugayaDataModel> InsertSurugayaAsync(SurugayaDataModel surugayaDataModel)
    {
        try
        {
            var response = await supabaseClient
                .From<SurugayaDataModel>()
                .Insert(surugayaDataModel);

            return response.Model!;
        }
        catch (Exception ex)
        {
            throw new Exception($"插入 SurugayaDataModel 資料失敗: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<SurugayaDataModel>> GetAllSurugayaAsync()
    {
        try
        {
            var response = await supabaseClient
                .From<SurugayaDataModel>()
                .Get();

            return response.Models;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得 SurugayaDataModel 資料失敗: {ex.Message}", ex);
        }
    }

    public async Task<SurugayaDataModel?> GetSurugayaByUrlAsync(string url)
    {
        try
        {
            var response = await supabaseClient
                .From<SurugayaDataModel>()
                .Where(x => x.ProductUrl == url)
                .Single();

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception($"根據 ID 取得 SurugayaDataModel 資料失敗: {ex.Message}", ex);
        }
    }


    public async Task<bool> IsUrlExistAsync(string url)
    {
        try
        {
            var response = await supabaseClient
                .From<SurugayaDataModel>()
                .Where(x => x.ProductUrl == url)
                .Get();

            var result = response.Models.Count != 0;

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"根據 ID 取得 SurugayaDataModel 資料失敗: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 透過 id 刪除資料
    /// </summary>
    /// <param name="id">駿河屋的商品 Id</param>
    public async Task DeleteFromIdAsync(int id)
    {
        try
        {

            var url = $"{ProjectConst.BaseUrl}/{id}";
            
            await supabaseClient
                .From<SurugayaDataModel>()
                .Where(x => x.ProductUrl == url)
                .Delete();
        }
        catch (Exception ex)
        {
            throw new Exception($"根據 ID 刪除 SurugayaDataModel 資料失敗: {ex.Message}", ex);
        }
    }
}
