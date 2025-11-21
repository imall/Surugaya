# Hangfire 遷移說明

## 概述

已將駿河屋爬蟲從 `BackgroundService` 遷移到 `Hangfire` 架構。

## 變更內容

### 1. 服務類別重構

**原檔案:** `SurugayaScraperBackgroundService.cs`  
**新類別:** `SurugayaScraperJob`

- 移除 `BackgroundService` 繼承
- 改用 Hangfire Job 架構
- 新增 `Hangfire.Server.PerformContext` 支援
- 加入 Hangfire Console 輸出功能

### 2. 設定檔調整

**檔案:** `SurugayaScraperSettings.cs`

新增欄位:
- `CronExpression`: Cron 表達式，用於設定執行排程

標記為棄用:
- `ScheduledTimes`: 改用 CronExpression
- `ErrorRetryIntervalMinutes`: Hangfire 自動處理重試

### 3. Job 註冊

**檔案:** `RecurringJobsRegistrar.cs`

新增駿河屋爬蟲的 Hangfire Recurring Job 註冊:
```csharp
recurringJobManager.AddOrUpdate<SurugayaScraperJob>(
    recurringJobId: "surugaya-scraper-job",
    methodCall: service => service.ExecuteAsync(null),
    cronExpression: scraperSettings.CronExpression,
    options: new RecurringJobOptions
    {
        TimeZone = TimeZoneInfo.Local
    });
```

### 4. DI 容器調整

**檔案:** `Program.cs`

- 移除: `builder.Services.AddHostedService<SurugayaScraperBackgroundService>()`
- 新增: `builder.Services.AddScoped<SurugayaScraperJob>()`

## 配置更新

需要更新 `appsettings.json` 中的 `SurugayaScraper` 設定:

### 舊配置 (已棄用)
```json
{
  "SurugayaScraper": {
    "Enabled": true,
    "ScheduledTimes": ["00:00", "06:00", "12:00", "18:00"],
    "ErrorRetryIntervalMinutes": 5,
    "TimeoutMinutes": 60
  }
}
```

### 新配置
```json
{
  "SurugayaScraper": {
    "Enabled": true,
    "CronExpression": "0 0,6,12,18 * * *",
    "TimeoutMinutes": 60
  }
}
```

## Cron 表達式範例

| 表達式 | 說明 |
|--------|------|
| `0 */6 * * *` | 每 6 小時執行一次 (預設) |
| `0 0,6,12,18 * * *` | 每天 00:00, 06:00, 12:00, 18:00 執行 |
| `0 0 * * *` | 每天午夜執行 |
| `0 */4 * * *` | 每 4 小時執行一次 |
| `0 8,20 * * *` | 每天 08:00, 20:00 執行 |
| `*/30 * * * *` | 每 30 分鐘執行一次 |

Cron 格式: `分 時 日 月 星期`

## 優勢

### 使用 Hangfire 的好處:
1. **視覺化管理**: 可透過 `/hangfire` Dashboard 監控任務狀態
2. **自動重試**: 內建失敗重試機制
3. **任務歷史**: 保存執行歷史記錄
4. **手動觸發**: 可在 Dashboard 手動執行任務
5. **靈活排程**: 使用標準 Cron 表達式
6. **分散式支援**: 支援多實例部署
7. **持久化**: 任務狀態儲存在資料庫中

## 使用方式

### 1. 啟動應用程式
```bash
dotnet run
```

### 2. 存取 Hangfire Dashboard
瀏覽器開啟: `http://localhost:<port>/hangfire`

### 3. 檢視定期任務
在 Dashboard 中選擇 "Recurring Jobs" 頁面，可以看到:
- `health-check-job`: 健康檢查任務 (每 12 分鐘)
- `surugaya-scraper-job`: 駿河屋爬蟲任務 (依 CronExpression 設定)

### 4. 手動執行
在 Recurring Jobs 頁面，點擊任務右側的 "Trigger now" 按鈕即可手動觸發

### 5. 檢視執行記錄
在 Dashboard 的 "Jobs" 頁面可以看到所有任務的執行歷史和狀態

## 注意事項

1. 確保 Hangfire 資料庫連線正常
2. 舊的 `ScheduledTimes` 設定已標記為棄用，但仍保留以維持相容性
3. 建議使用 `CronExpression` 設定執行排程
4. Hangfire 會自動處理重試，無需手動設定 `ErrorRetryIntervalMinutes`

## 移除舊檔案 (可選)

如果確認遷移成功，可以考慮:
1. 從配置檔中移除已棄用的欄位
2. 將 `SurugayaScraperBackgroundService.cs` 重新命名為 `SurugayaScraperJob.cs` 以反映實際用途
