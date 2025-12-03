using Surugaya.Repository;
using Surugaya.Repository.Models;
using Surugaya.Common.Models;

namespace Surugaya.Service;

/// <summary>
/// 作品名稱服務
/// </summary>
/// <param name="repository"></param>
public class SeriesNameMappingService(SeriesNameMappingRepository repository)
{
    private Dictionary<string, string>? _cachedMappings;

    /// <summary>
    /// 取得所有系列名稱對應（帶快取）
    /// </summary>
    public async Task<Dictionary<string, string>> GetAllMappingsAsync()
    {
        var mappings = await repository.GetAllMappingsAsync();
        
        _cachedMappings = mappings.ToDictionary(
            m => m.JapaneseKey,
            m => m.ChineseName
        );
        
        
        return _cachedMappings;
    }

    /// <summary>
    /// 從日文標題取得中文作品名稱
    /// </summary>
    /// <param name="japaneseTitle">包含日文作品名的標題</param>
    /// <returns>對應的中文作品名稱，找不到則回傳空字串</returns>
    public async Task<string> GetSeriesNameAsync(string japaneseTitle)
    {
        if (string.IsNullOrWhiteSpace(japaneseTitle))
            return string.Empty;

        var mappings = await GetAllMappingsAsync();

        // 按照鍵的長度從長到短排序（優先匹配更具體的關鍵字）
        var sortedMappings = mappings.OrderByDescending(x => x.Key.Length);

        foreach (var kvp in sortedMappings)
        {
            if (japaneseTitle.Contains(kvp.Key))
            {
                return kvp.Value;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 新增系列名稱對應
    /// </summary>
    public async Task<SeriesNameMappingModel> AddMappingAsync(string japaneseKey, string chineseName)
    {
        var result = await repository.InsertMappingAsync(japaneseKey, chineseName);
        
        return new SeriesNameMappingModel
        {
            JapaneseKey = result.JapaneseKey,
            ChineseName = result.ChineseName
        };
    }
    
    /// <summary>
    /// 刪除系列名稱對應
    /// </summary>
    public async Task DeleteMappingAsync(string chineseName)
    {
        await repository.DeleteMappingAsync(chineseName);
    }
}
