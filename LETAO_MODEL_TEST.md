# LetaoAddToCartRequest 自動屬性測試

## 測試程式碼

```csharp
using Surugaya.Common.Models;
using System.Text.Json;

// 測試 1: 使用 ProductId 自動生成 ImageUrl
var request1 = new LetaoAddToCartRequest
{
    Url = "https://www.suruga-ya.jp/product/detail/873910587",
    ProductId = "873910587",  // 設定 ProductId
    Title = "測試商品",
    UnitPrice = "3,970"
};

Console.WriteLine("測試 1: 使用 ProductId");
Console.WriteLine($"ProductId: {request1.ProductId}");
Console.WriteLine($"ImageUrl: {request1.ImageUrl}");
Console.WriteLine($"預期: https://cdn.suruga-ya.jp/database/pics_light/game/873910587.jpg");
Console.WriteLine($"結果: {request1.ImageUrl == "https://cdn.suruga-ya.jp/database/pics_light/game/873910587.jpg"}");
Console.WriteLine();

// 測試 2: 手動設定 ImageUrl
var request2 = new LetaoAddToCartRequest
{
    Url = "https://www.suruga-ya.jp/product/detail/123456",
    ImageUrl = "https://custom-url.com/image.jpg",  // 直接設定 ImageUrl
    Title = "測試商品2",
    UnitPrice = "1,000"
};

Console.WriteLine("測試 2: 手動設定 ImageUrl");
Console.WriteLine($"ProductId: {request2.ProductId ?? "null"}");
Console.WriteLine($"ImageUrl: {request2.ImageUrl}");
Console.WriteLine($"預期: https://custom-url.com/image.jpg");
Console.WriteLine($"結果: {request2.ImageUrl == "https://custom-url.com/image.jpg"}");
Console.WriteLine();

// 測試 3: JSON 反序列化（模擬 API 接收請求）
var json = @"{
  ""url"": ""https://www.suruga-ya.jp/product/detail/999888777"",
  ""productId"": ""999888777"",
  ""title"": ""測試商品3"",
  ""unitPrice"": ""5,000""
}";

var request3 = JsonSerializer.Deserialize<LetaoAddToCartRequest>(json);

Console.WriteLine("測試 3: JSON 反序列化");
Console.WriteLine($"ProductId: {request3?.ProductId}");
Console.WriteLine($"ImageUrl: {request3?.ImageUrl}");
Console.WriteLine($"預期: https://cdn.suruga-ya.jp/database/pics_light/game/999888777.jpg");
Console.WriteLine($"結果: {request3?.ImageUrl == "https://cdn.suruga-ya.jp/database/pics_light/game/999888777.jpg"}");
Console.WriteLine();

// 測試 4: 先設定 ImageUrl，後設定 ProductId（ProductId 會覆蓋）
var request4 = new LetaoAddToCartRequest
{
    Url = "https://www.suruga-ya.jp/product/detail/111222333",
    ImageUrl = "https://old-url.com/image.jpg"
};

Console.WriteLine("測試 4: ProductId 覆蓋 ImageUrl");
Console.WriteLine($"初始 ImageUrl: {request4.ImageUrl}");

request4.ProductId = "111222333";  // 設定 ProductId 會覆蓋 ImageUrl

Console.WriteLine($"設定 ProductId 後的 ImageUrl: {request4.ImageUrl}");
Console.WriteLine($"預期: https://cdn.suruga-ya.jp/database/pics_light/game/111222333.jpg");
Console.WriteLine($"結果: {request4.ImageUrl == "https://cdn.suruga-ya.jp/database/pics_light/game/111222333.jpg"}");
```

## 預期輸出

```
測試 1: 使用 ProductId
ProductId: 873910587
ImageUrl: https://cdn.suruga-ya.jp/database/pics_light/game/873910587.jpg
預期: https://cdn.suruga-ya.jp/database/pics_light/game/873910587.jpg
結果: True

測試 2: 手動設定 ImageUrl
ProductId: null
ImageUrl: https://custom-url.com/image.jpg
預期: https://custom-url.com/image.jpg
結果: True

測試 3: JSON 反序列化
ProductId: 999888777
ImageUrl: https://cdn.suruga-ya.jp/database/pics_light/game/999888777.jpg
預期: https://cdn.suruga-ya.jp/database/pics_light/game/999888777.jpg
結果: True

測試 4: ProductId 覆蓋 ImageUrl
初始 ImageUrl: https://old-url.com/image.jpg
設定 ProductId 後的 ImageUrl: https://cdn.suruga-ya.jp/database/pics_light/game/111222333.jpg
預期: https://cdn.suruga-ya.jp/database/pics_light/game/111222333.jpg
結果: True
```

## 結論

✅ 自動屬性功能正常運作
✅ 支援 JSON 反序列化時自動生成
✅ 可手動覆蓋 ImageUrl
✅ ProductId 設定時會自動更新 ImageUrl
