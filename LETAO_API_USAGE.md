# 樂淘購物車 API 使用說明

## API 端點

### 1. 加入商品到購物車（支援單筆或多筆）

**端點**: `POST /api/LetaoCart/add`

**請求範例 1（單筆商品 - 使用陣列格式）**:
```json
[
  {
    "url": "https://www.suruga-ya.jp/product/detail/873910587",
    "productId": "873910587",
    "title": "タペストリー　[単品] マリヤ・ミハイロヴナ・九条(マーシャ) B2タペストリー",
    "spec": "",
    "unitPrice": "3,970",
    "quantity": "1",
    "comment": ""
  }
]
```

**請求範例 2（多筆商品）**:
```json
[
  {
    "url": "https://www.suruga-ya.jp/product/detail/873910587",
    "productId": "873910587",
    "title": "タペストリー　[単品] マリヤ・ミハイロヴナ・九条(マーシャ) B2タペストリー",
    "unitPrice": "3,970",
    "quantity": "1"
  },
  {
    "url": "https://www.suruga-ya.jp/product/detail/123456789",
    "productId": "123456789",
    "title": "商品2",
    "unitPrice": "2,500",
    "quantity": "2"
  },
  {
    "url": "https://www.suruga-ya.jp/product/detail/987654321",
    "productId": "987654321",
    "title": "商品3",
    "unitPrice": "1,800",
    "quantity": "1"
  }
]
```

**請求參數說明**:
- `url` (必填): 商品的 URL
- `productId` (選填): 商品 ID，提供後會自動生成 `imageUrl`（推薦使用）
- `imageUrl` (選填): 商品圖片的 URL，若提供 `productId` 則會自動生成
  - 格式：`https://cdn.suruga-ya.jp/database/pics_light/game/{productId}.jpg`
  - 若 `productId` 和 `imageUrl` 都未提供，將回傳錯誤
- `title` (必填): 商品標題
- `spec` (選填): 商品規格
- `unitPrice` (必填): 商品單價（可包含逗號）
- `quantity` (選填): 數量，預設為 "1"
- `comment` (選填): 備註

**成功回應範例（批次）**:
```json
{
  "totalCount": 3,
  "successCount": 2,
  "failedCount": 1,
  "allSuccess": false,
  "results": [
    {
      "title": "タペストリー　[単品] マリヤ・ミハイロヴナ・九条(マーシャ) B2タペストリー",
      "url": "https://www.suruga-ya.jp/product/detail/873910587",
      "success": true,
      "message": "加入購物車成功",
      "statusCode": 200
    },
    {
      "title": "商品2",
      "url": "https://www.suruga-ya.jp/product/detail/123456789",
      "success": true,
      "message": "加入購物車成功",
      "statusCode": 200
    },
    {
      "title": "商品3",
      "url": "https://www.suruga-ya.jp/product/detail/987654321",
      "success": false,
      "message": "Session 已失效，已達最大重試次數",
      "statusCode": 401
    }
  ]
}
```

### 2. 清除登入快取

**端點**: `POST /api/LetaoCart/clear-cache`

用於測試或強制重新登入。

**回應範例**:
```json
{
  "message": "已清除登入快取"
}
```

## 使用範例

### cURL（單筆商品）
```bash
curl -X POST "https://localhost:5001/api/LetaoCart/add" \
  -H "Content-Type: application/json" \
  -d '[
    {
      "url": "https://www.suruga-ya.jp/product/detail/873910587",
      "productId": "873910587",
      "title": "タペストリー　[単品] マリヤ・ミハイロヴナ・九条(マーシャ) B2タペストリー",
      "unitPrice": "3,970",
      "quantity": "1"
    }
  ]'
```

### cURL（多筆商品）
```bash
curl -X POST "https://localhost:5001/api/LetaoCart/add" \
  -H "Content-Type: application/json" \
  -d '[
    {
      "url": "https://www.suruga-ya.jp/product/detail/873910587",
      "productId": "873910587",
      "title": "商品1",
      "unitPrice": "3,970"
    },
    {
      "url": "https://www.suruga-ya.jp/product/detail/123456789",
      "productId": "123456789",
      "title": "商品2",
      "unitPrice": "2,500"
    }
  ]'
```

### JavaScript (Fetch)
```javascript
// 單筆商品
const response = await fetch('https://localhost:5001/api/LetaoCart/add', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify([
    {
      url: 'https://www.suruga-ya.jp/product/detail/873910587',
      productId: '873910587',
      title: 'タペストリー　[単品] マリヤ・ミハイロヴナ・九条(マーシャ) B2タペストリー',
      unitPrice: '3,970',
      quantity: '1'
    }
  ])
});

// 多筆商品
const response2 = await fetch('https://localhost:5001/api/LetaoCart/add', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify([
    {
      url: 'https://www.suruga-ya.jp/product/detail/873910587',
      productId: '873910587',
      title: '商品1',
      unitPrice: '3,970'
    },
    {
      url: 'https://www.suruga-ya.jp/product/detail/123456789',
      productId: '123456789',
      title: '商品2',
      unitPrice: '2,500'
    }
  ])
});

const result = await response.json();
console.log(`成功: ${result.successCount}/${result.totalCount}`);
```

const result = await response.json();
console.log(result);
```

### C# (HttpClient)
```csharp
var client = new HttpClient();
var requests = new List<LetaoAddToCartRequest>
{
    new LetaoAddToCartRequest
    {
        Url = "https://www.suruga-ya.jp/product/detail/873910587",
        ProductId = "873910587",
        Title = "商品1",
        UnitPrice = "3,970",
        Quantity = "1"
    },
    new LetaoAddToCartRequest
    {
        Url = "https://www.suruga-ya.jp/product/detail/123456789",
        ProductId = "123456789",
        Title = "商品2",
        UnitPrice = "2,500",
        Quantity = "2"
    }
};

var response = await client.PostAsJsonAsync(
    "https://localhost:5001/api/LetaoCart/add", 
    requests
);

var result = await response.Content.ReadFromJsonAsync<LetaoBatchAddToCartResponse>();
Console.WriteLine($"成功: {result.SuccessCount}/{result.TotalCount}");
```

## 工作流程

1. **首次呼叫**: API 會自動登入樂淘並取得 Cookie
2. **後續呼叫**: 使用快取的 Cookie（快取 23 小時）
3. **Cookie 失效**: 自動重新登入並重試（最多 2 次）
4. **批次處理**: 逐筆處理每個商品，一筆失敗不影響其他商品
5. **回傳結果**: 包含總數、成功數、失敗數及每筆商品的詳細結果

## 注意事項

- **陣列格式**: 現在必須使用陣列格式，即使只加入一筆商品也要用 `[...]` 包裹
- **批次處理**: 多筆商品會依序處理，一筆失敗不會中斷其他商品
- Cookie 快取時間預設為 23 小時
- 每筆商品失敗時會自動重試最多 2 次
- 每次重試間隔 1 秒
- 價格中的逗號會自動被移除
- 數量預設為 1
- **推薦使用 `productId` 而非手動提供 `imageUrl`**，系統會自動生成正確的圖片 URL
- 圖片 URL 格式：`https://cdn.suruga-ya.jp/database/pics_light/game/{productId}.jpg`

## 回應欄位說明

- `totalCount`: 總共嘗試加入的商品數量
- `successCount`: 成功加入的商品數量
- `failedCount`: 失敗的商品數量
- `allSuccess`: 是否全部成功（boolean）
- `results[]`: 每筆商品的詳細結果
  - `title`: 商品標題
  - `url`: 商品 URL
  - `success`: 該筆是否成功
  - `message`: 結果訊息
  - `statusCode`: HTTP 狀態碼
