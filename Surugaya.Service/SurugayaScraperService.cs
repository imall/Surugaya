using Microsoft.Extensions.Logging;
using Surugaya.Common.Mapper;
using Surugaya.Common.Models;
using Surugaya.Repository;
using Surugaya.Repository.Models;
using Surugaya.Service.Utils;

namespace Surugaya.Service;

/// <summary>
/// 爬蟲服務
/// </summary>
public class SurugayaScraperService(
    SurugayaUrlsRepository repo, 
    SurugayaDetailsRepository detailRepo, 
    SurugayaCategoryRepository categoryRepository, 
    ScraperUtil scraper,
    ILogger<SurugayaScraperService> logger)
{
    public async Task<SurugayaDetailModel> ScrapeProductInfoByUrl(string url)
    {
        var surugaya = await repo.GetSurugayaByUrlAsync(url);
        
        if (surugaya == null)
        {
            throw new InvalidOperationException($"找不到 URL: {url}");
        }
        
        var product = await scraper.ScrapeProductAsync(surugaya.ProductUrl);

        product.LastUpdated = surugaya.CreatedAt;
        var dto = await detailRepo.InsertOrUpdateSurugayaAsync(product);
        var uri = new Uri(dto.Url);

        // 智能匹配作品名稱
        var seriesName = SeriesNameMapper.GetSeriesName(dto.Title);

        if (!string.IsNullOrEmpty(seriesName))
        {
            try
            {
                await categoryRepository.UpsertSeriesNameAsync(url, seriesName);
                Console.WriteLine($"✓ 已更新作品名稱: {dto.Title} -> {seriesName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ 更新作品名稱失敗: {dto.Title} - {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"⚠ 未找到匹配的作品名稱: {dto.Title}");
        }
        
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

    public async Task<IEnumerable<SurugayaDetailModel>> ScrapeAllProductInfo(Action<string>? writeLog = null)
    {
        // 取得所有 願望清單 資料
        var data = await repo.GetAllSurugayaAsync();
        // 擷取 url
        var urlDatas = data.Select(x => new { url = x.ProductUrl, createAt = x.CreatedAt }).ToArray();
        var products = new List<SurugayaDetail>();

        writeLog?.Invoke($"開始爬取 {urlDatas.Length} 個商品");
        logger.LogInformation("開始爬取 {Count} 個商品", urlDatas.Length);

        // 逐一爬取每個商品
        for (var i = 0; i < urlDatas.Length; i++)
        {
            try
            {
                var message = $"[{i + 1}/{urlDatas.Length}] 正在處理...";
                Console.WriteLine(message);
                writeLog?.Invoke(message);
                
                var product = await scraper.ScrapeProductAsync(urlDatas[i].url);
                product.LastUpdated = urlDatas[i].createAt;
                products.Add(product);
                
                var successMessage = $"✓ 完成: {product.Title}";
                Console.WriteLine(successMessage);
                writeLog?.Invoke(successMessage);
                logger.LogInformation("已爬取商品 [{Index}/{Total}]: {Title}", i + 1, urlDatas.Length, product.Title);

                if (i >= urlDatas.Length - 1) continue;

                Console.WriteLine("等待 0.5 秒...");
                await Task.Delay(500);
            }
            catch (Exception ex)
            {
                var errorMessage = $"✗ 錯誤: {ex.Message}";
                Console.WriteLine(errorMessage);
                writeLog?.Invoke(errorMessage);
                logger.LogError(ex, "爬取商品時發生錯誤 [{Index}/{Total}]", i + 1, urlDatas.Length);
            }
        }

        writeLog?.Invoke($"開始批量更新 {products.Count} 個商品資料到資料庫");
        logger.LogInformation("開始批量更新 {Count} 個商品資料到資料庫", products.Count);
        
        var detailsDto = await detailRepo.InsertOrUpdateSurugayaAsync(products);

        writeLog?.Invoke("開始更新作品名稱");
        logger.LogInformation("開始更新作品名稱");

        // 批量更新作品名稱
        var successCount = 0;
        var failCount = 0;
        var skipCount = 0;
        
        foreach (var detail in detailsDto)
        {

            // 智能匹配作品名稱
            var seriesName = SeriesNameMapper.GetSeriesName(detail.Title);

            if (!string.IsNullOrEmpty(seriesName))
            {
                try
                {
                    await categoryRepository.UpsertSeriesNameAsync(detail.Url, seriesName);
                    var message = $"✓ 已更新作品名稱: {detail.Title} -> {seriesName}";
                    Console.WriteLine(message);
                    writeLog?.Invoke(message);
                    successCount++;
                }
                catch (Exception ex)
                {
                    var message = $"✗ 更新作品名稱失敗: {detail.Title} - {ex.Message}";
                    Console.WriteLine(message);
                    writeLog?.Invoke(message);
                    logger.LogError(ex, "更新作品名稱失敗: {Title}", detail.Title);
                    failCount++;
                }
            }
            else
            {
                var message = $"⚠ 未找到匹配的作品名稱: {detail.Title}";
                Console.WriteLine(message);
                writeLog?.Invoke(message);
                skipCount++;
            }
        }

        var summaryMessage = $"作品名稱更新完成 - 成功: {successCount}, 失敗: {failCount}, 跳過: {skipCount}";
        writeLog?.Invoke(summaryMessage);
        logger.LogInformation(summaryMessage);

        return detailsDto.Select(x =>
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
