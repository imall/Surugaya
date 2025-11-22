using Supabase;
using Surugaya.Common.Models;
using Surugaya.Repository.Models;

namespace Surugaya.Repository;

public class SurugayaUrlsRepository(Client supabaseClient)
{
    public async Task<SurugayaUrls> InsertSurugayaAsync(SurugayaUrls surugayaUrls)
    {
        try
        {
            var response = await supabaseClient
                .From<SurugayaUrls>()
                .Insert(surugayaUrls);

            return response.Model!;
        }
        catch (Exception ex)
        {
            throw new Exception($"插入 Surugaya 資料失敗: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<SurugayaUrls>> GetAllSurugayaAsync()
    {
        try
        {
            var response = await supabaseClient
                .From<SurugayaUrls>()
                .Get();

            return response.Models;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得 Surugaya 資料失敗: {ex.Message}", ex);
        }
    }

    public async Task<SurugayaUrls?> GetSurugayaByUrlAsync(string url)
    {
        try
        {
            var response = await supabaseClient
                .From<SurugayaUrls>()
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
                .From<SurugayaUrls>()
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
                .From<SurugayaUrls>()
                .Where(x => x.ProductUrl.Contains(url))
                .Delete();
        }
        catch (Exception ex)
        {
            throw new Exception($"根據 ID 刪除 Surugaya 資料失敗: {ex.Message}", ex);
        }
    }


    /// <summary>
    /// 透過 url 刪除資料
    /// </summary>
    /// <param name="url">駿河屋的商品 Id</param>
    public async Task DeleteFromUrlAsync(string url)
    {
        try
        {
            await supabaseClient
                .From<SurugayaUrls>()
                .Where(x => x.ProductUrl == url)
                .Delete();
        }
        catch (Exception ex)
        {
            throw new Exception($"根據 url 刪除 Surugaya 資料失敗: {ex.Message}", ex);
        }
    }
}
