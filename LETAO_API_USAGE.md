# 樂淘購物車 API 使用說明

## API 端點

### 1. 加入商品到購物車

**端點**: `POST /api/LetaoCart/add`

**請求範例 1（使用 ProductId，推薦）**:
```json
{
  "url": "https://www.suruga-ya.jp/product/detail/873910587",
  "productId": "873910587",
  "title": "タペストリー　[単品] マリヤ・ミハイロヴナ・九条(マーシャ) B2タペストリー",
  "spec": "",
  "unitPrice": "3,970",
  "quantity": "1",
  "comment": ""
}
```

**請求範例 2（手動提供完整 ImageUrl）**:
```json
{
  "url": "https://www.suruga-ya.jp/product/detail/873910587",
  "imageUrl": "https://cdn.suruga-ya.jp/database/pics_light/game/873910587.jpg",
  "title": "タペストリー　[単品] マリヤ・ミハイロヴナ・九条(マーシャ) B2タペストリー",
  "spec": "",
  "unitPrice": "3,970",
  "quantity": "1",
  "comment": ""
}
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

**成功回應範例**:
```json
{
  "success": true,
  "message": "加入購物車成功",
  "statusCode": 200,
  "rawResponse": "{\"code\":\"200\",\"message\":\"成功\"}"
}
```

**失敗回應範例**:
```json
{
  "success": false,
  "message": "Session 已失效，已達最大重試次數",
  "statusCode": 401,
  "rawResponse": "{\"code\":\"401\",\"message\":\"未登入\"}"
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

### cURL
```bash
# 使用 ProductId（推薦）
curl -X POST "https://localhost:5001/api/LetaoCart/add" \
  -H "Content-Type: application/json" \
  -d '{
    "url": "https://www.suruga-ya.jp/product/detail/873910587",
    "productId": "873910587",
    "title": "タペストリー　[単品] マリヤ・ミハイロヴナ・九条(マーシャ) B2タペストリー",
    "unitPrice": "3,970",
    "quantity": "1"
  }'
```

### JavaScript (Fetch)
```javascript
const response = await fetch('https://localhost:5001/api/LetaoCart/add', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    url: 'https://www.suruga-ya.jp/product/detail/873910587',
    productId: '873910587',  // 只需要提供 ID
    title: 'タペストリー　[単品] マリヤ・ミハイロヴナ・九条(マーシャ) B2タペストリー',
    unitPrice: '3,970',
    quantity: '1'
  })
});

const result = await response.json();
console.log(result);
```

### C# (HttpClient)
```csharp
var client = new HttpClient();
var request = new LetaoAddToCartRequest
{
    Url = "https://www.suruga-ya.jp/product/detail/873910587",
    ProductId = "873910587",  // 只需要提供 ID，會自動生成 ImageUrl
    Title = "タペストリー　[単品] マリヤ・ミハイロヴナ・九条(マーシャ) B2タペストリー",
    UnitPrice = "3,970",
    Quantity = "1"
};

var response = await client.PostAsJsonAsync(
    "https://localhost:5001/api/LetaoCart/add", 
    request
);

var result = await response.Content.ReadFromJsonAsync<LetaoAddToCartResponse>();
```

## 工作流程

1. **首次呼叫**: API 會自動登入樂淘並取得 Cookie
2. **後續呼叫**: 使用快取的 Cookie（快取 23 小時）
3. **Cookie 失效**: 自動重新登入並重試（最多 2 次）
4. **成功**: 回傳成功訊息
5. **失敗**: 回傳失敗原因和狀態碼

## 注意事項

- Cookie 快取時間預設為 23 小時
- 加入購物車失敗時會自動重試最多 2 次
- 每次重試間隔 1 秒
- 價格中的逗號會自動被移除
- 數量預設為 1
- **推薦使用 `productId` 而非手動提供 `imageUrl`**，系統會自動生成正確的圖片 URL
- 圖片 URL 格式：`https://cdn.suruga-ya.jp/database/pics_light/game/{productId}.jpg`
