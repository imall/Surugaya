using Surugaya.Common.Models;
using Surugaya.Repository;
using Surugaya.Repository.Models;

namespace Surugaya.Service;

public class SurugayaCategoryService(SurugayaCategoryRepository detailsRepository, SurugayaUrlsRepository urlsRepository)
{
    /// <summary>
    /// 新增或更新目的
    /// </summary>
    /// <param name="url"></param>
    /// <param name="purposeCategory"></param>
    /// <returns></returns>
    public async Task<SurugayaCategoryModel> UpdatePurposeCategoryAsync(string url, PurposeCategoryEnum purposeCategory)
    {
        var dto = await detailsRepository.UpsertPurposeCategoryAsync(url, purposeCategory);
        var uri = new Uri(dto.Url);
        return new SurugayaCategoryModel
        {
            Id = int.Parse(uri.Segments.Last()),
            Url = dto.Url,
            PurposeCategoryId = dto.PurposeCategory,
            PurposeCategory = dto.PurposeCategory.ToString(),
            SeriesName = dto.SeriesName
        };
    }
    
    /// <summary>
    /// 新增或更新作品名稱
    /// </summary>
    /// <param name="url"></param>
    /// <param name="seriesName"></param>
    /// <returns></returns>
    public async Task<SurugayaCategoryModel> UpdateSeriesNameAsync(string url, string seriesName)
    {
        var dto = await detailsRepository.UpsertSeriesNameAsync(url, seriesName);
        var uri = new Uri(dto.Url);
        return new SurugayaCategoryModel
        {
            Id = int.Parse(uri.Segments.Last()),
            Url = dto.Url,
            PurposeCategoryId = dto.PurposeCategory,
            PurposeCategory = dto.PurposeCategory.ToString(),
            SeriesName = dto.SeriesName
        };
    }
    
    /// <summary>
    /// 新增或更新作品名稱跟目的
    /// </summary>
    /// <param name="url"></param>
    /// <param name="purposeCategory"></param>
    /// <param name="seriesName"></param>
    /// <returns></returns>
    public async Task<SurugayaCategoryModel> UpsertPurposeAndSeriesAsync(string url, PurposeCategoryEnum purposeCategory, string? seriesName)
    {
        var dto = await detailsRepository.UpsertCategoryAndSeriesAsync(url, purposeCategory, seriesName);
        var uri = new Uri(dto.Url);
        return new SurugayaCategoryModel
        {
            Id = int.Parse(uri.Segments.Last()),
            Url = dto.Url,
            PurposeCategoryId = dto.PurposeCategory,
            PurposeCategory = dto.PurposeCategory.ToString(),
            SeriesName = dto.SeriesName
        };
    }
}
