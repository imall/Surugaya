using Microsoft.AspNetCore.Mvc;
using Surugaya.Common.Models;
using Surugaya.Repository.Models;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

/// <summary>
/// 處理完成的願望清單資料 
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SurugayaCategoryController(SurugayaCategoryService service) : ControllerBase
{
    /// <summary>
    /// 編輯用途分類
    /// </summary>
    /// <param name="id"></param>
    /// <param name="purposeCategory"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}/purposeCategory")]
    public async Task<SurugayaCategoryModel> UpdatePurposeCategory(int id, PurposeCategoryEnum purposeCategory)
    {
        return await service.UpdatePurposeCategoryAsync(id, purposeCategory);
    }
    
    /// <summary>
    /// 編輯作品名稱
    /// </summary>
    /// <param name="id"></param>
    /// <param name="seriesName"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}/seriesName")]
    public async Task<SurugayaCategoryModel> UpdateSeriesName(int id, string seriesName)
    {
        return await service.UpdateSeriesNameAsync(id, seriesName);
    }
    
    /// <summary>
    /// 編輯作品名稱
    /// </summary>
    /// <param name="id"></param>
    /// <param name="category"></param>
    /// <param name="seriesName"></param>
    /// <returns></returns>
    [HttpPatch("{id:int}/purposeAndSeries")]
    public async Task<SurugayaCategoryModel> UpsertPurposeAndSeries(int id, PurposeCategoryEnum category,string seriesName)
    {
        return await service.UpsertPurposeAndSeriesAsync(id, category,seriesName);
    }
}
