# 系列名稱對應功能說明

## 概述

此功能將原本硬編碼在 `SeriesNameMapper.cs` 中的日文動漫作品名稱對應資料移至 Supabase 資料庫，提供更靈活的管理方式。

## 架構說明

### 1. 資料庫層 (Repository)

**檔案位置**: `Surugaya.Repository/Models/SeriesNameMapping.cs`

定義資料庫模型：
```csharp
[Table("SeriesNameMapping")]
public class SeriesNameMapping : BaseModel
{
    [PrimaryKey("id", false)]
    public long Id { get; set; }

    [Column("japanese_key")]
    public string JapaneseKey { get; set; }

    [Column("chinese_name")]
    public string ChineseName { get; set; }
}
```

**檔案位置**: `Surugaya.Repository/SeriesNameMappingRepository.cs`

提供基本的 CRUD 操作：
- `GetAllMappingsAsync()` - 取得所有對應資料
- `GetMappingByKeyAsync(string)` - 根據日文關鍵字查詢
- `InsertMappingAsync(string, string)` - 新增對應
- `UpdateMappingAsync(long, string)` - 更新對應
- `DeleteMappingAsync(long)` - 刪除對應

### 2. 服務層 (Service)

**檔案位置**: `Surugaya.Service/SeriesNameMappingService.cs`

提供業務邏輯與快取機制：
- 快取機制：資料會快取 1 小時，減少資料庫查詢次數
- `GetSeriesNameAsync(string)` - 從日文標題取得中文作品名稱
- `RefreshCacheAsync()` - 手動重新整理快取

### 3. API 層 (Controller)

**檔案位置**: `Surugaya.API/Controllers/SeriesNameMappingController.cs`

提供 RESTful API 端點：

| 方法 | 路徑 | 說明 |
|-----|------|-----|
| GET | `/api/SeriesNameMapping` | 取得所有對應 |
| GET | `/api/SeriesNameMapping/search?japaneseTitle=xxx` | 搜尋對應的中文名稱 |
| POST | `/api/SeriesNameMapping` | 新增對應 |
| PUT | `/api/SeriesNameMapping/{id}` | 更新對應 |
| DELETE | `/api/SeriesNameMapping/{id}` | 刪除對應 |
| POST | `/api/SeriesNameMapping/refresh-cache` | 重新整理快取 |

## 資料庫初始化

### 1. 執行 SQL 腳本

在 Supabase SQL Editor 中執行 `database/init_series_name_mapping.sql`：

```bash
# 檔案位置
database/init_series_name_mapping.sql
```

此腳本會：
1. 建立 `SeriesNameMapping` 資料表
2. 建立索引以提升查詢效能
3. 插入所有預設的對應資料

### 2. 驗證資料

執行以下 SQL 確認資料已正確插入：

```sql
SELECT COUNT(*) as total_mappings FROM SeriesNameMapping;
SELECT * FROM SeriesNameMapping ORDER BY id;
```

## API 使用範例

### 1. 取得所有對應

```bash
GET /api/SeriesNameMapping
```

回應：
```json
{
  "Re：ゼロから始める異世界生活": "Re:從零開始的異世界生活",
  "リゼロ": "Re:從零開始的異世界生活",
  "SAO": "刀劍神域",
  ...
}
```

### 2. 搜尋對應的中文名稱

```bash
GET /api/SeriesNameMapping/search?japaneseTitle=リゼロの新刊が発売
```

回應：
```json
{
  "japaneseTitle": "リゼロの新刊が発売",
  "seriesName": "Re:從零開始的異世界生活"
}
```

### 3. 新增對應

```bash
POST /api/SeriesNameMapping
Content-Type: application/json

{
  "japaneseKey": "鬼滅の刃",
  "chineseName": "鬼滅之刃"
}
```

### 4. 更新對應

```bash
PUT /api/SeriesNameMapping/1
Content-Type: application/json

{
  "chineseName": "更新後的中文名稱"
}
```

### 5. 刪除對應

```bash
DELETE /api/SeriesNameMapping/1
```

### 6. 重新整理快取

```bash
POST /api/SeriesNameMapping/refresh-cache
```

## 程式碼使用範例

### 在服務中使用

```csharp
public class YourService
{
    private readonly SeriesNameMappingService _mappingService;

    public YourService(SeriesNameMappingService mappingService)
    {
        _mappingService = mappingService;
    }

    public async Task ProcessTitle(string japaneseTitle)
    {
        // 取得對應的中文名稱
        var chineseName = await _mappingService.GetSeriesNameAsync(japaneseTitle);
        
        if (!string.IsNullOrEmpty(chineseName))
        {
            // 找到對應的名稱
            Console.WriteLine($"中文名稱: {chineseName}");
        }
        else
        {
            // 找不到對應
            Console.WriteLine("找不到對應的系列名稱");
        }
    }
}
```

## 匹配邏輯

系統會按照關鍵字長度從長到短進行匹配，確保更具體的關鍵字優先匹配。

例如：
- 標題: "転生したら第七王子だったので、気ままに魔術を極めます 第1巻"
- 會先匹配 "転生したら第七王子だったので、気ままに魔術を極めます" (完整名稱)
- 而非 "転生したら第七王子" (簡稱)

## 快取機制

- 服務層使用記憶體快取，預設 1 小時過期
- 新增、更新、刪除操作會自動清除快取
- 可透過 API 手動重新整理快取

## 效能考量

1. **索引**: 資料庫已在 `japanese_key` 欄位建立索引
2. **快取**: Service 層使用 Singleton 生命週期並實作快取機制
3. **批次查詢**: Repository 提供批次取得所有資料的方法

## 注意事項

1. `SeriesNameMapper.cs` 現在只保留 `GetDefaultMappings()` 方法供資料庫初始化使用
2. 所有實際的查詢應改用 `SeriesNameMappingService`
3. Service 註冊為 Singleton 以維持快取，Repository 為 Scoped
4. 確保 Supabase 連線設定正確

## 遷移指南

如果您的程式碼原本使用 `SeriesNameMapper.GetSeriesName()`:

**舊程式碼**:
```csharp
var seriesName = SeriesNameMapper.GetSeriesName(japaneseTitle);
```

**新程式碼**:
```csharp
// 透過依賴注入取得 SeriesNameMappingService
public YourClass(SeriesNameMappingService mappingService)
{
    _mappingService = mappingService;
}

// 使用非同步方法
var seriesName = await _mappingService.GetSeriesNameAsync(japaneseTitle);
```

## 資料庫結構

```sql
CREATE TABLE SeriesNameMapping (
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    japanese_key TEXT NOT NULL UNIQUE,
    chinese_name TEXT NOT NULL
);

CREATE INDEX idx_series_mappings_key ON SeriesNameMapping(japanese_key);
```

## 疑難排解

### 1. 快取未更新

執行以下 API 重新整理快取：
```bash
POST /api/SeriesNameMapping/refresh-cache
```

### 2. 資料庫連線失敗

檢查 `appsettings.json` 中的 Supabase 設定：
```json
{
  "Supabase": {
    "Url": "your-supabase-url",
    "AnonKey": "your-anon-key",
    "ServiceKey": "your-service-key"
  }
}
```

### 3. 查詢不到資料

確認資料已正確匯入：
```sql
SELECT COUNT(*) FROM SeriesNameMapping;
```
