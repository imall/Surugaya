using Surugaya.Common.Models;
using Surugaya.Repository;
using Surugaya.Repository.Models;

namespace Surugaya.Service;

public class SurugayaCategoryService(SurugayaCategoryRepository detailsRepository, SurugayaRepository repository)
{
    /// <summary>
    /// 新增或更新目的
    /// </summary>
    /// <param name="url"></param>
    /// <param name="purposeCategory"></param>
    /// <returns></returns>
    public async Task<SurugayaDetailModel> UpdatePurposeCategoryAsync(int url, PurposeCategoryEnum purposeCategory)
    {
        var dto = await detailsRepository.UpsertPurposeCategoryAsync(url, purposeCategory);
        var uri = new Uri(dto.Url);
        return new SurugayaDetailModel
        {
            Id = int.Parse(uri.Segments.Last()),
            Url = dto.Url,
            PurposeCategory = dto.PurposeCategory.ToString(),
            SeriesName = dto.SeriesName
        };
    }

    /// <summary>
    /// 新增或更新作品名稱
    /// </summary>
    /// <param name="id"></param>
    /// <param name="seriesName"></param>
    /// <returns></returns>
    public async Task<SurugayaDetailModel> UpdateSeriesNameAsync(int id, string seriesName)
    {
        var dto = await detailsRepository.UpsertSeriesNameAsync(id, seriesName);
        var uri = new Uri(dto.Url);
        return new SurugayaDetailModel
        {
            Id = int.Parse(uri.Segments.Last()),
            Url = dto.Url,
            PurposeCategory = dto.PurposeCategory.ToString(),
            SeriesName = dto.SeriesName
        };
    }


    /// <summary>
    /// 新增或更新作品名稱跟目的
    /// </summary>
    /// <param name="id"></param>
    /// <param name="purposeCategory"></param>
    /// <param name="seriesName"></param>
    /// <returns></returns>
    public async Task<SurugayaDetailModel> UpsertCategoryAndSeriesAsync(int id, PurposeCategoryEnum purposeCategory, string? seriesName)
    {
        var dto = await detailsRepository.UpsertCategoryAndSeriesAsync(id, purposeCategory, seriesName);
        var uri = new Uri(dto.Url);
        return new SurugayaDetailModel
        {
            Id = int.Parse(uri.Segments.Last()),
            Url = dto.Url,
            PurposeCategory = dto.PurposeCategory.ToString(),
            SeriesName = dto.SeriesName
        };
    }
}
