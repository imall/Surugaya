using Supabase.Postgrest;
using Supabase.Postgrest.Responses;
using Surugaya.Common.Models;
using Surugaya.Repository.Models;
using Client = Supabase.Client;

namespace Surugaya.Repository;

public class SurugayaCategoryRepository(Client supabaseClient)
{
    
    /// <summary>
    /// 取得 urls 清單的所有細節資料
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IEnumerable<SurugayaCategory>> GetAllCategoryAsync()
    {
        try
        {
            var response = await supabaseClient
                .From<SurugayaCategory>()
                .Get();

            return response.Models;
        }
        catch (Exception ex)
        {
            throw new Exception($"取得 SurugayaDetail 資料失敗: {ex.Message}", ex);
        }
    }
    
    
    /// <summary>
    /// 更新或插入商品的用途分類
    /// </summary>
    /// <param name="id">商品 id</param>
    /// <param name="purposeCategory">新的分類</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<SurugayaCategory> UpsertPurposeCategoryAsync(int id, PurposeCategoryEnum purposeCategory)
    {
        try
        {
            var url = $"{ProjectConst.BaseUrl}/{id}";
            
            // 先查詢是否存在
            var existingResponse = await IsUrlExist(url);

            if (existingResponse.Models.Count != 0)
            {
                // 資料存在，只更新 PurposeCategory
                var updateResponse = await supabaseClient
                    .From<SurugayaCategory>()
                    .Filter("url", Constants.Operator.Equals, url)
                    .Set(x => x.PurposeCategory, purposeCategory)
                    .Update();

                return updateResponse.Models.First();
            }

            // 資料不存在，插入新資料
            var categoryData = new SurugayaCategory
            {
                Url = url,
                PurposeCategory = purposeCategory,
                SeriesName = null
            };

            var insertResponse = await supabaseClient
                .From<SurugayaCategory>()
                .Insert(categoryData);

            return insertResponse.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新或插入 SurugayaCategory 分類失敗: {ex.Message}", ex);
        }
    }
    
    
    /// <summary>
    /// 更新或插入商品的用途分類
    /// </summary>
    /// <param name="url">商品 id</param>
    /// <param name="purposeCategory">新的分類</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<SurugayaCategory> UpsertPurposeCategoryAsync(string url, PurposeCategoryEnum purposeCategory)
    {
        try
        {
            // 先查詢是否存在
            var existingResponse = await IsUrlExist(url);

            if (existingResponse.Models.Count != 0)
            {
                // 資料存在，只更新 PurposeCategory
                var updateResponse = await supabaseClient
                    .From<SurugayaCategory>()
                    .Filter("url", Constants.Operator.Equals, url)
                    .Set(x => x.PurposeCategory, purposeCategory)
                    .Update();

                return updateResponse.Models.First();
            }

            // 資料不存在，插入新資料
            var categoryData = new SurugayaCategory
            {
                Url = url,
                PurposeCategory = purposeCategory,
                SeriesName = null
            };

            var insertResponse = await supabaseClient
                .From<SurugayaCategory>()
                .Insert(categoryData);

            return insertResponse.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新或插入 SurugayaCategory 分類失敗: {ex.Message}", ex);
        }
    }


    /// <summary>
    /// 更新或插入商品的作品名稱
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="seriesName">作品名稱</param>
    /// <returns></returns>
    public async Task<SurugayaCategory> UpsertSeriesNameAsync(int id, string? seriesName)
    {
        try
        {
            var url = $"{ProjectConst.BaseUrl}/{id}";
            
            // 先查詢是否存在
            var existingResponse = await IsUrlExist(url);

            if (existingResponse.Models.Count != 0)
            {
                // 資料存在，只更新 SeriesName
                var updateResponse = await supabaseClient
                    .From<SurugayaCategory>()
                    .Filter("url", Constants.Operator.Equals, url)
                    .Set(x => x.SeriesName!, seriesName)
                    .Update();

                return updateResponse.Models.First();
            }

            // 資料不存在，插入新資料
            var categoryData = new SurugayaCategory
            {
                Url = url,
                PurposeCategory = default, // 使用預設值
                SeriesName = seriesName
            };

            var insertResponse = await supabaseClient
                .From<SurugayaCategory>()
                .Insert(categoryData);

            return insertResponse.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新或插入作品名稱失敗: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// 更新或插入商品的作品名稱
    /// </summary>
    /// <param name="url">url</param>
    /// <param name="seriesName">作品名稱</param>
    /// <returns></returns>
    public async Task<SurugayaCategory> UpsertSeriesNameAsync(string url, string seriesName)
    {
        try
        {
            // 先查詢是否存在
            var existingResponse = await IsUrlExist(url);

            if (existingResponse.Models.Count != 0)
            {
                // 資料存在，只更新 SeriesName
                var updateResponse = await supabaseClient
                    .From<SurugayaCategory>()
                    .Filter("url", Constants.Operator.Equals, url)
                    .Set(x => x.SeriesName!, seriesName)
                    .Update();

                return updateResponse.Models.First();
            }

            // 資料不存在，插入新資料
            var categoryData = new SurugayaCategory
            {
                Url = url,
                PurposeCategory = default, // 使用預設值
                SeriesName = seriesName
            };

            var insertResponse = await supabaseClient
                .From<SurugayaCategory>()
                .Insert(categoryData);

            return insertResponse.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新或插入作品名稱失敗: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 更新或插入商品的分類和作品名稱
    /// </summary>
    /// <param name="id">商品 id</param>
    /// <param name="purposeCategory">用途分類</param>
    /// <param name="seriesName">作品名稱</param>
    /// <returns></returns>
    public async Task<SurugayaCategory> UpsertCategoryAndSeriesAsync(int id, PurposeCategoryEnum purposeCategory, string? seriesName)
    {
        try
        {
            var url = $"{ProjectConst.BaseUrl}/{id}";
            
            var existingResponse = await IsUrlExist(url);

            if ( existingResponse.Models.Count != 0)
            {
                // 資料存在，更新兩個欄位
                var updateResponse = await supabaseClient
                    .From<SurugayaCategory>()
                    .Filter("url", Constants.Operator.Equals, url)
                    .Set(x => x.PurposeCategory, purposeCategory)
                    .Set(x => x.SeriesName!, seriesName)
                    .Update();

                return updateResponse.Models.First();
            }

            // 資料不存在，插入新資料
            var categoryData = new SurugayaCategory
            {
                Url = url,
                PurposeCategory = purposeCategory,
                SeriesName = seriesName
            };

            var insertResponse = await supabaseClient
                .From<SurugayaCategory>()
                .Insert(categoryData);

            return insertResponse.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新或插入分類和作品名稱失敗: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// 更新或插入商品的分類和作品名稱
    /// </summary>
    /// <param name="url">商品 url</param>
    /// <param name="purposeCategory">用途分類</param>
    /// <param name="seriesName">作品名稱</param>
    /// <returns></returns>
    public async Task<SurugayaCategory> UpsertCategoryAndSeriesAsync(string url, PurposeCategoryEnum purposeCategory, string? seriesName)
    {
        try
        {
            var existingResponse = await IsUrlExist(url);

            if ( existingResponse.Models.Count != 0)
            {
                // 資料存在，更新兩個欄位
                var updateResponse = await supabaseClient
                    .From<SurugayaCategory>()
                    .Filter("url", Constants.Operator.Equals, url)
                    .Set(x => x.PurposeCategory, purposeCategory)
                    .Set(x => x.SeriesName!, seriesName)
                    .Update();

                return updateResponse.Models.First();
            }

            // 資料不存在，插入新資料
            var categoryData = new SurugayaCategory
            {
                Url = url,
                PurposeCategory = purposeCategory,
                SeriesName = seriesName
            };

            var insertResponse = await supabaseClient
                .From<SurugayaCategory>()
                .Insert(categoryData);

            return insertResponse.Models.First();
        }
        catch (Exception ex)
        {
            throw new Exception($"更新或插入分類和作品名稱失敗: {ex.Message}", ex);
        }
    }

    private async Task<ModeledResponse<SurugayaCategory>> IsUrlExist(string url)
    {
        // 先查詢是否存在
        var existingResponse = await supabaseClient
            .From<SurugayaCategory>()
            .Filter("url", Constants.Operator.Equals, url)
            .Get();
        return existingResponse;
    }
}
