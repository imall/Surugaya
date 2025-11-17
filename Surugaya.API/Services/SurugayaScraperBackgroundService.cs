using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Surugaya.API.Settings;
using Surugaya.Service;

namespace Surugaya.API.Services;

/// <summary>
/// 駿河屋爬蟲背景排程服務
/// 根據設定的時間執行爬蟲任務
/// </summary>
public class SurugayaScraperBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SurugayaScraperBackgroundService> _logger;
    private readonly SurugayaScraperSettings _settings;
    private readonly TimeSpan[] _scheduledTimes;

    /// <summary>
    /// 初始化駿河屋爬蟲背景服務
    /// </summary>
    /// <param name="serviceProvider">服務提供者</param>
    /// <param name="logger">日誌記錄器</param>
    /// <param name="options">爬蟲設定選項</param>
    public SurugayaScraperBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<SurugayaScraperBackgroundService> logger,
        IOptions<SurugayaScraperSettings> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _settings = options.Value;
        
        // 解析設定的執行時間
        _scheduledTimes = _settings.ScheduledTimes
            .Select(ParseTimeString)
            .OrderBy(t => t)
            .ToArray();
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

    /// <summary>
    /// 執行背景服務主邏輯
    /// </summary>
    /// <param name="stoppingToken">取消權杖</param>
    /// <returns>非同步任務</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("駿河屋爬蟲背景服務已停用");
            return;
        }

        _logger.LogInformation("駿河屋爬蟲背景服務已啟動，排程時間: {Times}", 
            string.Join(", ", _settings.ScheduledTimes));
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.Now;
                var nextRunTime = GetNextScheduledTime(now);
                var delay = nextRunTime - now;

                _logger.LogInformation("下次執行時間: {NextRunTime}, 等待時間: {Delay}", 
                    nextRunTime.ToString("yyyy-MM-dd HH:mm:ss"), delay);

                // 等待到下次執行時間
                await Task.Delay(delay, stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await ExecuteScrapingTask();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("駿河屋爬蟲背景服務已停止");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "駿河屋爬蟲背景服務執行時發生錯誤");
                
                // 發生錯誤時等待指定時間後重試
                await Task.Delay(TimeSpan.FromMinutes(_settings.ErrorRetryIntervalMinutes), stoppingToken);
            }
        }
    }

    private DateTime GetNextScheduledTime(DateTime current)
    {
        var today = current.Date;
        
        // 尋找今天剩餘的執行時間
        foreach (var time in _scheduledTimes)
        {
            var scheduledTime = today.Add(time);
            if (scheduledTime > current)
            {
                return scheduledTime;
            }
        }
        
        // 如果今天沒有剩餘時間，返回明天的第一個執行時間
        return today.AddDays(1).Add(_scheduledTimes[0]);
    }

    private async Task ExecuteScrapingTask()
    {
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(_settings.TimeoutMinutes));
        
        try
        {
            _logger.LogInformation("開始執行駿河屋爬蟲任務 - {DateTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            using var scope = _serviceProvider.CreateScope();
            var scraperService = scope.ServiceProvider.GetRequiredService<SurugayaScraperService>();
            
            var startTime = DateTime.Now;
            var results = await scraperService.ScrapeAllProductInfo();
            var endTime = DateTime.Now;
            var duration = endTime - startTime;

            var resultCount = results?.Count() ?? 0;
            
            _logger.LogInformation("駿河屋爬蟲任務完成 - 處理了 {Count} 個商品，耗時: {Duration}", 
                resultCount, duration.ToString(@"hh\:mm\:ss"));
        }
        catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
        {
            _logger.LogWarning("駿河屋爬蟲任務執行超時 ({TimeoutMinutes} 分鐘)", _settings.TimeoutMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "執行駿河屋爬蟲任務時發生錯誤");
        }
    }

    /// <summary>
    /// 停止背景服務
    /// </summary>
    /// <param name="stoppingToken">取消權杖</param>
    /// <returns>非同步任務</returns>
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("正在停止駿河屋爬蟲背景服務...");
        await base.StopAsync(stoppingToken);
        _logger.LogInformation("駿河屋爬蟲背景服務已停止");
    }
}
