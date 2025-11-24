using Surugaya.Common.Models;
using Surugaya.Repository;
using Surugaya.Repository.Models;

namespace Surugaya.Service;

public class SurugayaDetailsService(SurugayaDetailsRepository detailsRepository, SurugayaCategoryRepository categoryRepository, SurugayaUrlsRepository urlsRepository)
{
    public async Task<IEnumerable<SurugayaDetailModel>> GetAllInUrlAsync()
    {
        var dto = await urlsRepository.GetAllSurugayaAsync();

        var urls = dto.Select(x => x.ProductUrl);

        var details = await detailsRepository.GetAllInUrlAsync(urls);
        var categories = await categoryRepository.GetAllCategoryAsync();

        // 將 categories 轉換為字典，以 URL 作為 key
        var categoriesDict = categories.ToDictionary(c => c.Url);

        return details.Select(x =>
        {
            var uri = new Uri(x.Url);
            var baseUrl = uri.GetLeftPart(UriPartial.Path);
    
            // 從字典中找到對應的 category
            categoriesDict.TryGetValue(baseUrl, out var category);
            
            return new SurugayaDetailModel
            {
                Id = int.Parse(uri.Segments.Last()),
                Url = x.Url,
                Title = x.Title,
                ImageUrl = x.ImageUrl,
                CurrentPrice = x.CurrentPrice,
                SalePrice = x.SalePrice,
                Status = x.Status,
                LastUpdated = x.LastUpdated,
                PurposeCategoryId = category?.PurposeCategory ?? PurposeCategoryEnum.未分類,
                PurposeCategory = category?.PurposeCategory.ToString() ?? string.Empty,
                SeriesName = category?.SeriesName ?? string.Empty
            };
        });
    }
}
