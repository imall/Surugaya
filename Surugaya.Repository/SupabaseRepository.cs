using Supabase;
using Surugaya.Common.Models;
using Surugaya.Repository.Models;

namespace Surugaya.Repository;

public class SupabaseRepository(Client supabaseClient)
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

    public async Task DeleteFromUrlAsync(string url)
    {
        try
        {
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
