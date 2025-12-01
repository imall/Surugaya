# 樂淘購物車功能環境變數設定說明

## 設定方式

### 方法 1: 使用 appsettings.json（不推薦用於生產環境）
在 `appsettings.json` 中直接設定（僅適用於開發環境）：

```json
"LetaoSettings": {
  "Email": "your-email@example.com",
  "Password": "your-password",
  "CookieCacheHours": 23,
  "MaxRetries": 2,
  "RetryDelayMilliseconds": 1000
}
```

### 方法 2: 使用環境變數（推薦）

#### Windows (PowerShell)
```powershell
$env:LetaoSettings__Email = "your-email@example.com"
$env:LetaoSettings__Password = "your-password"
```

#### Windows (命令提示字元)
```cmd
set LetaoSettings__Email=your-email@example.com
set LetaoSettings__Password=your-password
```

#### Linux/macOS
```bash
export LetaoSettings__Email="your-email@example.com"
export LetaoSettings__Password="your-password"
```

### 方法 3: 使用 User Secrets（開發環境推薦）

1. 在專案目錄執行：
```bash
dotnet user-secrets init
dotnet user-secrets set "LetaoSettings:Email" "your-email@example.com"
dotnet user-secrets set "LetaoSettings:Password" "your-password"
```

2. 查看已設定的 secrets：
```bash
dotnet user-secrets list
```

### 方法 4: 使用 Docker 環境變數
在 `docker-compose.yml` 或 Dockerfile 中設定：

```yaml
environment:
  - LetaoSettings__Email=your-email@example.com
  - LetaoSettings__Password=your-password
```

## 參數說明

- **Email**: 樂淘登入信箱（必填）
- **Password**: 樂淘登入密碼（必填）
- **CookieCacheHours**: Cookie 快取時間（小時），預設 23 小時
- **MaxRetries**: 加入購物車失敗時的重試次數，預設 2 次
- **RetryDelayMilliseconds**: 重試間隔（毫秒），預設 1000 毫秒

## 安全性注意事項

⚠️ **重要**：
- 請勿將帳號密碼直接寫在 `appsettings.json` 中並提交到版本控制系統
- 生產環境務必使用環境變數或 Azure Key Vault 等安全存儲方式
- 開發環境建議使用 User Secrets
