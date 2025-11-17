using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Surugaya.API.Settings;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

/// <summary>
/// 駿河屋爬蟲管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ScraperManagementController : ControllerBase
{
    private readonly SurugayaScraperService _scraperService;
    private readonly SurugayaScraperSettings _settings;
    private readonly ILogger<ScraperManagementController> _logger;

    /// <summary>
    /// 初始化爬蟲管理控制器
    /// </summary>
    /// <param name="scraperService">爬蟲服務</param>
    /// <param name="settings">爬蟲設定</param>
    /// <param name="logger">日誌記錄器</param>
    public ScraperManagementController(
        SurugayaScraperService scraperService,
        IOptions<SurugayaScraperSettings> settings,
        ILogger<ScraperManagementController> logger)
    {
        _scraperService = scraperService;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// 手動執行完整爬蟲任務
    /// </summary>
    /// <returns>執行結果</returns>
    [HttpPost("execute")]
    public async Task<IActionResult> ExecuteManualScraping()
    {
        try
        {
            _logger.LogInformation("手動執行駿河屋爬蟲任務開始");
            var startTime = DateTime.Now;
            
            var results = await _scraperService.ScrapeAllProductInfo();
            
            var endTime = DateTime.Now;
            var duration = endTime - startTime;
            var resultCount = results?.Count() ?? 0;
            
            _logger.LogInformation("手動執行駿河屋爬蟲任務完成 - 處理了 {Count} 個商品", resultCount);
            
            return Ok(new
            {
                Success = true,
                Message = "爬蟲任務執行完成",
                ProcessedCount = resultCount,
                Duration = duration.ToString(@"hh\:mm\:ss"),
                StartTime = startTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = endTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "手動執行駿河屋爬蟲任務時發生錯誤");
            return StatusCode(500, new
            {
                Success = false,
                Message = "爬蟲任務執行失敗",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// 根據 URL 執行單個商品爬蟲
    /// </summary>
    /// <param name="url">商品 URL</param>
    /// <returns>執行結果</returns>
    [HttpPost("execute-single")]
    public async Task<IActionResult> ExecuteSingleProductScraping([FromQuery] string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest(new { Success = false, Message = "URL 不能為空" });
        }

        try
        {
            _logger.LogInformation("手動執行單個商品爬蟲: {Url}", url);
            
            var result = await _scraperService.ScrapeProductInfoByUrl(url);
            
            _logger.LogInformation("單個商品爬蟲完成: {Title}", result.Title);
            
            return Ok(new
            {
                Success = true,
                Message = "單個商品爬蟲完成",
                Product = result
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "執行單個商品爬蟲時發生錯誤: {Url}", url);
            return StatusCode(500, new
            {
                Success = false,
                Message = "單個商品爬蟲失敗",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// 取得爬蟲排程狀態
    /// </summary>
    /// <returns>排程狀態資訊</returns>
    [HttpGet("status")]
    public IActionResult GetScraperStatus()
    {
        var now = DateTime.Now;
        var nextScheduledTime = GetNextScheduledTime(now);
        
        return Ok(new
        {
            Enabled = _settings.Enabled,
            ScheduledTimes = _settings.ScheduledTimes,
            ErrorRetryIntervalMinutes = _settings.ErrorRetryIntervalMinutes,
            TimeoutMinutes = _settings.TimeoutMinutes,
            CurrentTime = now.ToString("yyyy-MM-dd HH:mm:ss"),
            NextScheduledTime = nextScheduledTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            TimeUntilNext = nextScheduledTime?.Subtract(now).ToString(@"hh\:mm\:ss")
        });
    }

    /// <summary>
    /// 計算下次執行時間
    /// </summary>
    /// <param name="current">目前時間</param>
    /// <returns>下次執行時間</returns>
    private DateTime? GetNextScheduledTime(DateTime current)
    {
        if (!_settings.Enabled || _settings.ScheduledTimes == null)
            return null;

        var today = current.Date;
        var scheduledTimes = _settings.ScheduledTimes
            .Select(ParseTimeString)
            .OrderBy(t => t)
            .ToArray();

        // 尋找今天剩餘的執行時間
        foreach (var time in scheduledTimes)
        {
            var scheduledTime = today.Add(time);
            if (scheduledTime > current)
            {
                return scheduledTime;
            }
        }

        // 如果今天沒有剩餘時間，返回明天的第一個執行時間
        return today.AddDays(1).Add(scheduledTimes[0]);
    }

    /// <summary>
    /// 解析時間字串為 TimeSpan
    /// </summary>
    /// <param name="timeString">時間字串 (格式: HH:mm)</param>
    /// <returns>TimeSpan 物件</returns>
    private static TimeSpan ParseTimeString(string timeString)
    {
        if (TimeSpan.TryParseExact(timeString, @"hh\:mm", null, out var result))
        {
            return result;
        }
        throw new ArgumentException($"無效的時間格式: {timeString}，應為 HH:mm 格式");
    }
}
