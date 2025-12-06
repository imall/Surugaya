using Microsoft.Extensions.Logging;
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
    SeriesNameMappingService seriesNameMappingService,
    ILogger<SurugayaScraperService> logger)
{
    
    /// <summary>
    /// 透過 url 爬蟲並更新資料庫
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<SurugayaDetailModel> ScrapeProductInfoByUrl(string url)
    {
        var surugaya = await repo.GetSurugayaByUrlAsync(url);
        
        if (surugaya == null)
        {
            throw new InvalidOperationException($"找不到 URL: {url}");
        }
        
        var detail = await scraper.ScrapeProductAsync(surugaya.ProductUrl);

        detail.LastUpdated = surugaya.CreatedAt;
        var dto = await detailRepo.InsertOrUpdateSurugayaAsync(detail);

        // 智能匹配作品名稱
        var seriesName = await seriesNameMappingService.GetSeriesNameAsync(dto.Title);

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
            LastUpdated = dto.LastUpdated,
            SeriesName = seriesName
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
        
        
        // 配置爬取速度
        var config = new ScraperConfig
        {
            MinDelayMs = 2000,      // 最小延遲 2 秒
            MaxDelayMs = 5000,      // 最大延遲 5 秒
            ErrorDelayMs = 30000,   // 錯誤後延遲 30 秒
            BatchSize = 10,         // 每 10 個商品後休息
            BatchDelayMs = 60000    // 批次間休息 1 分鐘
        };

        // 逐一爬取每個商品
        for (var i = 0; i < urlDatas.Length; i++)
        {
            try
            {
                var message = $"[{i + 1}/{urlDatas.Length}] 正在處理...";
                writeLog?.Invoke(message);
                
                var product = await scraper.ScrapeProductAsync(urlDatas[i].url);
                product.LastUpdated = urlDatas[i].createAt;
                products.Add(product);
                
                var successMessage = $"✓ 完成: {product.Title}";
                writeLog?.Invoke(successMessage);
                logger.LogInformation("已爬取商品 [{Index}/{Total}]: {Title}", i + 1, urlDatas.Length, product.Title);

                if (i >= urlDatas.Length - 1) continue;
                
                // 隨機延遲（模擬人類行為）
                var delay = Random.Shared.Next(config.MinDelayMs, config.MaxDelayMs);
                var delaySeconds = delay / 1000.0;
                Console.WriteLine($"等待 {delaySeconds:F1} 秒...");
                writeLog?.Invoke($"等待 {delaySeconds:F1} 秒...");
                await Task.Delay(delay);

                // 每處理一批商品後，休息更長時間
                if ((i + 1) % config.BatchSize != 0) continue;
                
                var batchDelaySeconds = config.BatchDelayMs / 1000;
                var batchMessage = $"已完成 {i + 1} 個商品，休息 {batchDelaySeconds} 秒...";
                Console.WriteLine(batchMessage);
                writeLog?.Invoke(batchMessage);
                await Task.Delay(config.BatchDelayMs);
            }
            catch (Exception ex)
            {
                var errorMessage = $"✗ 錯誤: {ex.Message}";
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
        var mapping =  await seriesNameMappingService.GetAllMappingsAsync();
        // 為了效能，先排好 key 長度順序
        foreach (var detail in detailsDto)
        {
            var match = mapping.FirstOrDefault(x => detail.Title.Contains(x.Key));

            if (match.Value != null)
            {
                try
                {
                    await categoryRepository.UpsertSeriesNameAsync(detail.Url, match.Value);

                    var message = $"✓ 已更新作品名稱: {detail.Title} -> {match.Value}";
                    writeLog?.Invoke(message);
                    successCount++;
                }
                catch (Exception ex)
                {
                    var message = $"✗ 更新作品名稱失敗: {detail.Title} - {ex.Message}";
                    writeLog?.Invoke(message);
                    logger.LogError(ex, "更新作品名稱失敗: {Title}", detail.Title);
                    failCount++;
                }
            }
            else
            {
                var message = $"⚠ 未找到匹配的作品名稱: {detail.Title}";
                writeLog?.Invoke(message);
                skipCount++;
            }
        }

        var summaryMessage = $"作品名稱更新完成 - 成功: {successCount}, 失敗: {failCount}, 跳過: {skipCount}";
        writeLog?.Invoke(summaryMessage);

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


public class ScraperConfig
{
    public int MinDelayMs { get; set; } = 2000;
    public int MaxDelayMs { get; set; } = 5000;
    public int ErrorDelayMs { get; set; } = 30000;
    public int BatchSize { get; set; } = 10;
    public int BatchDelayMs { get; set; } = 60000;
}