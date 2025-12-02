using Surugaya.Common.Models;
using Surugaya.Repository;
using Surugaya.Repository.Models;

namespace Surugaya.Service;

public class SurugayaDetailsService(
    SurugayaDetailsRepository detailsRepository,
    SurugayaCategoryRepository categoryRepository,
    SurugayaUrlsRepository urlsRepository,
    SurugayaPurchaseRepository purchaseRepository)
{
    public async Task<IEnumerable<SurugayaDetailModel>> GetAllInUrlAsync()
    {
        var dto = await urlsRepository.GetAllSurugayaAsync();

        var urls = dto.Select(x => x.ProductUrl).ToList();

        var details = await detailsRepository.GetAllInUrlAsync(urls);
        var categories = await categoryRepository.GetAllCategoryAsync();

        // 批次取得所有購買紀錄 (避免 N+1 查詢問題)
        var purchases = await purchaseRepository.GetPurchaseByUrlsAsync(urls);

        // 將 categories 轉換為字典，以 URL 作為 key
        var categoriesDict = categories.ToDictionary(c => c.Url);

        // 將 purchases 按 URL 分組
        var purchasesDict = purchases
            .GroupBy(p => p.Url)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => new PurchaseHistoryItem
                {
                    Id = p.Id,
                    Url = p.Url,
                    Date = p.Date,
                    Note = p.Note
                }).ToList()
            );

        return details.Select(x =>
        {
            var uri = new Uri(x.Url);

            // 從字典中找到對應的 category
            categoriesDict.TryGetValue(uri.ToString(), out var category);

            // 從字典中找到對應的購買紀錄
            purchasesDict.TryGetValue(x.Url, out var purchaseHistory);

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
                SeriesName = category?.SeriesName ?? string.Empty,
                PurchaseHistory = purchaseHistory ?? []
            };
        });
    }
}
