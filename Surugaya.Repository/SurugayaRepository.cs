using Supabase;
using Surugaya.Common.Models;
using Surugaya.Repository.Models;

namespace Surugaya.Repository;

public class SurugayaRepository(Client supabaseClient)
{
    public async Task<Models.Surugaya> InsertSurugayaAsync(Models.Surugaya surugaya)
    {
        try
        {
            var response = await supabaseClient
                .From<Models.Surugaya>()
                .Insert(surugaya);

            return response.Model!;
        }
        catch (Exception ex)
        {
            throw new Exception($"插入 Surugaya 資料失敗: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<Models.Surugaya>> GetAllSurugayaAsync()
    {
        try
        {
            var response = await supabaseClient
                .From<Models.Surugaya>()
                .Get();

            return response.Models;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得 Surugaya 資料失敗: {ex.Message}", ex);
        }
    }

    public async Task<Models.Surugaya?> GetSurugayaByUrlAsync(string url)
    {
        try
        {
            var response = await supabaseClient
                .From<Models.Surugaya>()
                .Where(x => x.ProductUrl == url)
                .Single();

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception($"根據 ID 取得 Surugaya 資料失敗: {ex.Message}", ex);
        }
    }
    
    public async Task<bool> IsUrlExistAsync(string url)
    {
        try
        {
            var response = await supabaseClient
                .From<Models.Surugaya>()
                .Where(x => x.ProductUrl == url)
                .Get();

            var result = response.Models.Count != 0;

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"根據 ID 取得 Surugaya 資料失敗: {ex.Message}", ex);
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
                .From<Models.Surugaya>()
                .Where(x => x.ProductUrl.Contains(url))
                .Delete();
        }
        catch (Exception ex)
        {
            throw new Exception($"根據 ID 刪除 Surugaya 資料失敗: {ex.Message}", ex);
        }
    }
    
    
}
