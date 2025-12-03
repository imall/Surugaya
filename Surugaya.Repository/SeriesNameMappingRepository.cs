using Surugaya.Repository.Models;
using Client = Supabase.Client;

namespace Surugaya.Repository;

public class SeriesNameMappingRepository(Client supabaseClient)
{
    /// <summary>
    /// 取得所有系列名稱對應資料
    /// </summary>
    /// <returns>系列名稱對應清單</returns>
    public async Task<IEnumerable<SeriesNameMapping>> GetAllMappingsAsync()
    {
        try
        {
            var response = await supabaseClient
                .From<SeriesNameMapping>()
                .Get();

            return response.Models;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得 SeriesNameMapping 資料失敗: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// 新增系列名稱對應
    /// </summary>
    /// <param name="japaneseKey">日文關鍵字</param>
    /// <param name="chineseName">中文名稱</param>
    /// <returns>新增的對應資料</returns>
    public async Task<SeriesNameMapping> InsertMappingAsync(string japaneseKey, string chineseName)
    {
        try
        {
            var newMapping = new SeriesNameMapping
            {
                JapaneseKey = japaneseKey,
                ChineseName = chineseName
            };

            var response = await supabaseClient
                .From<SeriesNameMapping>()
                .Upsert(newMapping); 

            return response.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception($"新增 SeriesNameMapping 資料失敗: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 刪除系列名稱對應
    /// </summary>
    /// <param name="chineseName"></param>
    public async Task DeleteMappingAsync(string chineseName)
    {
        try
        {
            await supabaseClient
                .From<SeriesNameMapping>()
                .Where(x => x.ChineseName == chineseName)
                .Delete();
        }
        catch (Exception ex)
        {
            throw new Exception($"刪除 SeriesNameMapping 資料失敗: {ex.Message}", ex);
        }
    }
}
