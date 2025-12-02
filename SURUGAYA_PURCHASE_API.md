# Surugaya Purchase API 使用說明

## 概述
此 API 用於記錄 Surugaya 商品的購買紀錄，支援新增、查詢和刪除購買紀錄。

## 資料表結構
在使用此 API 之前，請先在 Supabase 資料庫中執行以下 SQL 腳本建立資料表：

```sql
-- 參考 database/create_purchase_table.sql
```

## API 端點

### 1. 新增購買紀錄
**端點：** `POST /api/SurugayaPurchase`

**Request Body：**
```json
{
  "url": "https://www.suruga-ya.jp/product/detail/123456",
  "date": "2024-12-01T00:00:00Z",  // 選填，預設為當下時間
  "note": "首次購入 - 初回限定版"      // 選填
}
```

**Response (成功)：**
```json
{
  "success": true,
  "message": "購買紀錄已成功新增",
  "purchaseHistory": [
    {
      "url": "https://www.suruga-ya.jp/product/detail/123456",
      "date": "2024-12-01T00:00:00Z",
      "note": "首次購入 - 初回限定版"
    }
  ]
}
```

**Response (失敗)：**
```json
{
  "success": false,
  "message": "商品 URL 為必填欄位"
}
```

### 2. 取得所有購買紀錄
**端點：** `GET /api/SurugayaPurchase`

**Response：**
```json
{
  "success": true,
  "purchaseHistory": [
    {
      "url": "https://www.suruga-ya.jp/product/detail/789012",
      "date": "2024-12-02T00:00:00Z",
      "note": "第二次購買",
      "createdAt": "2024-12-02T10:30:00Z"
    },
    {
      "url": "https://www.suruga-ya.jp/product/detail/123456",
      "date": "2024-12-01T00:00:00Z",
      "note": "首次購入 - 初回限定版",
      "createdAt": "2024-12-01T10:30:00Z"
    }
  ]
}
```

### 3. 根據 URL 查詢購買紀錄
**端點：** `GET /api/SurugayaPurchase/by-url?url={商品URL}`

**Query Parameters：**
- `url` (必填)：商品的完整 URL

**範例：**
```
GET /api/SurugayaPurchase/by-url?url=https://www.suruga-ya.jp/product/detail/123456
```

**Response：**
```json
{
  "success": true,
  "purchaseHistory": [
    {
      "url": "https://www.suruga-ya.jp/product/detail/123456",
      "date": "2024-12-01T00:00:00Z",
      "note": "首次購入 - 初回限定版",
      "createdAt": "2024-12-01T10:30:00Z"
    }
  ]
}
```

### 4. 刪除購買紀錄
**端點：** `DELETE /api/SurugayaPurchase?url={商品URL}`

**Query Parameters：**
- `url` (必填)：商品的完整 URL

**範例：**
```
DELETE /api/SurugayaPurchase?url=https://www.suruga-ya.jp/product/detail/123456
```

**Response (成功)：**
```json
{
  "success": true,
  "message": "購買紀錄已成功刪除"
}
```

**Response (失敗)：**
```json
{
  "success": false,
  "message": "找不到指定的購買紀錄"
}
```

## 使用範例

### 使用 curl 新增購買紀錄
```bash
# 使用當前時間
curl -X POST "http://localhost:5000/api/SurugayaPurchase" \
  -H "Content-Type: application/json" \
  -d '{
    "url": "https://www.suruga-ya.jp/product/detail/123456",
    "note": "首次購入"
  }'

# 指定購買日期
curl -X POST "http://localhost:5000/api/SurugayaPurchase" \
  -H "Content-Type: application/json" \
  -d '{
    "url": "https://www.suruga-ya.jp/product/detail/123456",
    "date": "2024-12-01T00:00:00Z",
    "note": "首次購入 - 初回限定版"
  }'
```

### 使用 curl 查詢購買紀錄
```bash
# 取得所有購買紀錄
curl -X GET "http://localhost:5000/api/SurugayaPurchase"

# 根據 URL 查詢
curl -X GET "http://localhost:5000/api/SurugayaPurchase/by-url?url=https://www.suruga-ya.jp/product/detail/123456"
```

### 使用 curl 刪除購買紀錄
```bash
curl -X DELETE "http://localhost:5000/api/SurugayaPurchase?url=https://www.suruga-ya.jp/product/detail/123456"
```

### 使用 JavaScript fetch
```javascript
// 新增購買紀錄
const response = await fetch('http://localhost:5000/api/SurugayaPurchase', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    url: 'https://www.suruga-ya.jp/product/detail/123456',
    note: '首次購入 - 初回限定版'
  })
});

const data = await response.json();
console.log(data);
```

## 注意事項

1. **主鍵設計**：URL 作為主鍵，每個商品 URL 只能有一筆購買紀錄
2. **日期格式**：日期欄位使用 ISO 8601 格式 (YYYY-MM-DDTHH:mm:ssZ)
3. **時區**：所有時間均以 UTC 時區儲存
4. **URL 驗證**：URL 必須為有效的 Surugaya 商品連結
5. **排序**：購買紀錄預設按日期降序排列（最新的在前）
6. **資料庫**：確保已在 Supabase 中建立 `SurugayaPurchase` 資料表
7. **重複處理**：由於 URL 是主鍵，重複新增相同 URL 的購買紀錄會產生錯誤

## 架構說明

此功能包含以下層級：

1. **Controller** (`SurugayaPurchaseController.cs`)
   - 處理 HTTP 請求
   - 驗證輸入參數
   - 回傳適當的 HTTP 狀態碼

2. **Service** (`SurugayaPurchaseService.cs`)
   - 商業邏輯處理
   - 資料轉換
   - 錯誤處理和日誌記錄

3. **Repository** (`SurugayaPurchaseRepository.cs`)
   - 資料庫操作
   - CRUD 功能實作

4. **Models** (`SurugayaPurchaseModel.cs`)
   - Request/Response 資料模型定義
