# 樂淘購物車功能實作說明

## 📋 專案架構概覽

此功能實現了自動加入商品到樂淘購物車的完整流程，包含登入、Cookie 快取管理、錯誤處理和自動重試機制。

## 🗂️ 檔案結構

### 1. Models (Surugaya.Common/Models/)
定義資料模型和設定類別：

- **LetaoSettings.cs** - 樂淘帳號設定（Email、Password、快取時間等）
- **LetaoAddToCartRequest.cs** - 加入購物車請求參數
- **LetaoAddToCartResponse.cs** - 加入購物車回應
- **LetaoLoginResponse.cs** - 登入 API 回應模型

### 2. Services (Surugaya.Service/)
商業邏輯層：

- **LetaoAuthService.cs** - 認證服務
  - 處理登入流程
  - Cookie 快取管理
  - 自動更新過期的 Cookie
  
- **LetaoCartService.cs** - 購物車服務
  - 加入購物車主要邏輯
  - Session 失效檢測
  - 自動重試機制

### 3. Controllers (Surugaya.API/Controllers/)
API 端點：

- **LetaoCartController.cs** - REST API 控制器
  - `POST /api/LetaoCart/add` - 加入商品到購物車
  - `POST /api/LetaoCart/clear-cache` - 清除登入快取

### 4. Configuration
設定管理：

- **appsettings.json** - 設定檔（不要儲存敏感資訊）
- **環境變數** - 生產環境使用（推薦）
- **User Secrets** - 開發環境使用（推薦）

## 🔄 工作流程

```
前端請求
    ↓
LetaoCartController
    ↓
LetaoCartService
    ↓
檢查 Cookie 是否有效?
    ├─ 是 → 使用快取的 Cookie
    └─ 否 → LetaoAuthService.Login
              ↓
         快取新的 Cookie
              ↓
         執行加入購物車
              ↓
         Session 失效?
              ├─ 是 → 清除快取 → 重試 (最多 2 次)
              └─ 否 → 回傳結果
```

## 🎯 核心功能

### 1. 智能登入與快取
- 首次呼叫時自動登入
- Cookie 快取 23 小時（可設定）
- 過期自動更新
- 執行緒安全的快取管理

### 2. 錯誤處理與重試
- 自動偵測 Session 失效
- 最多重試 2 次（可設定）
- 重試間隔 1 秒（可設定）
- 詳細的錯誤日誌

### 3. 安全性
- 帳號密碼使用環境變數
- 不將敏感資訊寫入程式碼
- 支援多種設定方式

## 📝 使用步驟

### 第一步：設定環境變數

**開發環境 (使用 User Secrets)**
```bash
cd Surugaya.API
dotnet user-secrets set "LetaoSettings:Email" "your-email@example.com"
dotnet user-secrets set "LetaoSettings:Password" "your-password"
```

**生產環境 (使用環境變數)**
```bash
# PowerShell
$env:LetaoSettings__Email = "your-email@example.com"
$env:LetaoSettings__Password = "your-password"
```

### 第二步：啟動應用程式
```bash
cd Surugaya.API
dotnet run
```

### 第三步：呼叫 API
```bash
curl -X POST "https://localhost:5001/api/LetaoCart/add" \
  -H "Content-Type: application/json" \
  -d '{
    "url": "https://www.suruga-ya.jp/product/detail/873910587",
    "imageUrl": "https://cdn.suruga-ya.jp/database/pics_light/game/873910587.jpg",
    "title": "商品標題",
    "unitPrice": "3,970",
    "quantity": "1"
  }'
```

## 🧪 測試

### 測試加入購物車
使用 Swagger UI 或 Postman 測試 `POST /api/LetaoCart/add`

### 測試快取機制
1. 第一次呼叫：應該會看到「重新登入」的日誌
2. 第二次呼叫：應該會看到「使用快取的 Cookie」的日誌
3. 呼叫清除快取：`POST /api/LetaoCart/clear-cache`
4. 再次呼叫：應該會重新登入

## 📊 日誌說明

- `✅ 使用快取的 Cookie` - 使用快取，無需登入
- `🔑 Cookie 不存在或已過期，重新登入...` - 正在重新登入
- `✅ 登入成功！GUID: xxx` - 登入成功
- `🔄 嘗試第 X 次加入購物車...` - 正在重試
- `⚠️ 第 X 次嘗試失敗：Session 已失效` - Session 失效
- `🗑️ 已清除 Cookie 快取` - 快取已清除
- `✅ 成功加入購物車！` - 操作成功

## ⚙️ 設定參數

| 參數 | 說明 | 預設值 |
|------|------|--------|
| Email | 樂淘登入信箱 | (必填) |
| Password | 樂淘登入密碼 | (必填) |
| CookieCacheHours | Cookie 快取時間（小時） | 23 |
| MaxRetries | 最大重試次數 | 2 |
| RetryDelayMilliseconds | 重試間隔（毫秒） | 1000 |

## 🔒 安全性建議

1. ✅ **絕對不要**將帳號密碼寫在 `appsettings.json` 並提交到 Git
2. ✅ 開發環境使用 User Secrets
3. ✅ 生產環境使用環境變數或 Azure Key Vault
4. ✅ 定期更換密碼
5. ✅ 使用強密碼

## 📚 相關文件

- [API 使用說明](LETAO_API_USAGE.md)
- [環境變數設定說明](LETAO_ENV_SETUP.md)

## 🐛 常見問題

### Q1: 加入購物車失敗，顯示「Session 已失效」
**A**: 這是正常的重試機制，系統會自動重新登入並重試。如果持續失敗，請檢查帳號密碼是否正確。

### Q2: 如何查看日誌？
**A**: 日誌會輸出到控制台，可以在啟動應用程式的終端機中查看。

### Q3: Cookie 快取時間可以調整嗎？
**A**: 可以，在 `appsettings.json` 或環境變數中設定 `CookieCacheHours`。

### Q4: 可以同時處理多個請求嗎？
**A**: 可以，`LetaoAuthService` 使用了執行緒鎖來確保 Cookie 管理的執行緒安全。

## 🎉 完成！

現在你已經完成了樂淘購物車功能的實作，可以透過 API 自動加入商品到購物車了！
