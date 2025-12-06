using HtmlAgilityPack;
using System.Text.Json;
using Surugaya.Repository.Models;

namespace Surugaya.Service.Utils;

public class ScraperUtil(string flareSolverrUrl = "https://flaresolverr-production-b4e6.up.railway.app/v1")
{
    private readonly HttpClient _httpClient = new();

    public async Task<SurugayaDetail> ScrapeProductAsync(string url)
    {
        try
        {
            Console.WriteLine($"開始使用 FlareSolverr 爬取: {url}");

            // 呼叫 FlareSolverr API
            var request = new
            {
                cmd = "request.get",
                url = url,
                maxTimeout = 60000
            };

            var response = await _httpClient.PostAsync(
                flareSolverrUrl,
                new StringContent(
                    JsonSerializer.Serialize(request),
                    System.Text.Encoding.UTF8,
                    "application/json"
                )
            );

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"FlareSolverr 請求失敗: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            // 檢查狀態
            if (root.GetProperty("status").GetString() != "ok")
            {
                var message = root.GetProperty("message").GetString();
                throw new Exception($"FlareSolverr 錯誤: {message}");
            }

            // 取得 HTML
            var html = root.GetProperty("solution").GetProperty("response").GetString();

            // 使用 HtmlAgilityPack 解析 HTML
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var product = new SurugayaDetail
            {
                Url = url,
                LastUpdated = DateTime.Now
            };

            // 抓取商品標題
            var titleNode = doc.DocumentNode.SelectSingleNode("//h1[@class='h1_title_product']")
                ?? doc.DocumentNode.SelectSingleNode("//h1[@id='item_title']")
                ?? doc.DocumentNode.SelectSingleNode("//h1");
            product.Title = titleNode?.InnerText.Trim() ?? "找不到標題";
            Console.WriteLine($"商品標題: {product.Title}");

            // 抓取商品圖片
            var imageNode = doc.DocumentNode.SelectSingleNode("//div[@class='item_img']//img")
                ?? doc.DocumentNode.SelectSingleNode("//img[contains(@class,'img-fluid')]");
            if (imageNode != null)
            {
                var imgSrc = imageNode.GetAttributeValue("src", "");
                if (!string.IsNullOrEmpty(imgSrc))
                {
                    if (!imgSrc.StartsWith("http"))
                    {
                        product.ImageUrl = "https://www.suruga-ya.jp" + imgSrc;
                    }
                    else
                    {
                        product.ImageUrl = imgSrc;
                    }
                }
            }

            // 先檢查是否售完
            var outOfStockNode = doc.DocumentNode.SelectSingleNode("//div[@class='mgnB5 out-of-stock-text']");
            bool isOutOfStock = outOfStockNode != null;

            if (isOutOfStock)
            {
                // 商品已售完，設定狀態
                product.Status = outOfStockNode?.InnerText.Trim() ?? "品切れ中";
                product.CurrentPrice = 0;
                product.SalePrice = null;
            }
            else
            {
                // 商品有庫存，抓取價格資訊

                // 抓取原價 (price-old) - 劃掉的價格
                var oldPriceNode = doc.DocumentNode.SelectSingleNode("//span[@class='text-price-detail price-old']");
                if (oldPriceNode != null)
                {
                    var oldPriceText = oldPriceNode.InnerText
                        .Replace("¥", "")
                        .Replace(",", "")
                        .Replace("円", "")
                        .Replace("(税込)", "")
                        .Trim();
                    if (decimal.TryParse(oldPriceText, out var oldPrice))
                    {
                        product.CurrentPrice = oldPrice;
                    }
                }

                // 抓取優惠價格 (price-buy) - 實際購買價格
                var buyPriceNode = doc.DocumentNode.SelectSingleNode("//span[@class='text-price-detail price-buy']");
                if (buyPriceNode != null)
                {
                    var buyPriceText = buyPriceNode.InnerText
                        .Replace("¥", "")
                        .Replace(",", "")
                        .Replace("円", "")
                        .Replace("(税込)", "")
                        .Trim();
                    if (decimal.TryParse(buyPriceText, out var buyPrice))
                    {
                        // 如果有原價，則 buyPrice 是優惠價
                        if (product.CurrentPrice > 0)
                        {
                            product.SalePrice = buyPrice;
                        }
                        else
                        {
                            // 如果沒有原價，則 buyPrice 就是當前價格
                            product.CurrentPrice = buyPrice;
                        }
                    }
                }

                // 抓取庫存狀態
                var stockNode = doc.DocumentNode.SelectSingleNode("//span[@class='tag_product blue-light']/span");
                if (stockNode != null)
                {
                    product.Status = stockNode.InnerText.Trim();
                }
                else
                {
                    // 如果找不到庫存標籤，嘗試其他可能的狀態
                    var statusNode = doc.DocumentNode.SelectSingleNode("//span[@class='tag_product tag_popular']");
                    product.Status = statusNode?.InnerText.Trim() ?? "未知";
                }
            }

            // 檢查是否有 Flash Sale (タイムセール)
            var flashSaleNode = doc.DocumentNode.SelectSingleNode("//div[@class='flash_sale d-flex justify-content-between border']");
            if (flashSaleNode != null)
            {
                product.Status += " (タイムセール中)";
            }

            Console.WriteLine("爬取成功！");
            return product;
        }
        catch (Exception ex)
        {
            throw new Exception($"爬取失敗: {ex.Message}", ex);
        }
    }
}