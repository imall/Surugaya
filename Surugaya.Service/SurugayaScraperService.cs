using HtmlAgilityPack;
using Surugaya.Common.Models;
using Surugaya.Repository;
using Surugaya.Repository.Models;
using Surugaya.Service.Utils;

namespace Surugaya.Service;

/// <summary>
/// 爬蟲服務
/// </summary>
public class SurugayaScraperService(SurugayaUrlsRepository repo, SurugayaDetailsRepository detailRepo, ScraperUtil scraper)
{
    public async Task<SurugayaDetailModel> ScrapeProductInfoByUrl(string url)
    {
        var surugaya = await repo.GetSurugayaByUrlAsync(url);
        var product = await scraper.ScrapeProductAsync(surugaya.ProductUrl);

        product.LastUpdated = surugaya.CreatedAt;
        var dto = await detailRepo.InsertOrUpdateSurugayaAsync(product);

        var result = new SurugayaDetailModel
        {
            Url = dto.Url,
            Title = dto.Title,
            ImageUrl = dto.ImageUrl,
            CurrentPrice = dto.CurrentPrice,
            SalePrice = dto.SalePrice,
            Status = dto.Status,
            LastUpdated = dto.LastUpdated
        };

        return result;
    }

    public async Task<IEnumerable<SurugayaDetailModel>> ScrapeAllProductInfo()
    {
        // 取得所有 願望清單 資料
        var data = await repo.GetAllSurugayaAsync();
        // 擷取 url
        var urlDatas = data.Select(x => new { url = x.ProductUrl, createAt = x.CreatedAt }).ToArray();
        var products = new List<SurugayaDetail>();

        // 逐一爬取每個商品
        for (var i = 0; i < urlDatas.Length; i++)
        {
            try
            {
                Console.WriteLine($"[{i + 1}/{urlDatas.Length}] 正在處理...");
                var product = await scraper.ScrapeProductAsync(urlDatas[i].url);
                product.LastUpdated = urlDatas[i].createAt;
                products.Add(product);
                Console.WriteLine($"✓ 完成: {product.Title}\n");

                if (i >= urlDatas.Length - 1) continue;

                Console.WriteLine("等待 0.5 秒...\n");
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 錯誤: {ex.Message}\n");
            }
        }

        var dto = await detailRepo.InsertOrUpdateSurugayaAsync(products);
        return dto.Select(x =>
        {
            var uri = new Uri(x.Url);
            return new SurugayaDetailModel
            {
                Id = int.Parse(uri.Segments.Last()),
                Url = x.Url,
                Title = x.Title,
                ImageUrl = x.ImageUrl,
                CurrentPrice = x.CurrentPrice,
                SalePrice = x.SalePrice,
                Status = x.Status,
                LastUpdated = x.LastUpdated
            };
        });
    }
}
