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
    /// <returns></returns>
    [HttpPatch("purposeCategory/{purposeCategoryId}")]
    public async Task<SurugayaCategoryModel> UpdatePurposeCategory([FromBody]string url, PurposeCategoryEnum purposeCategoryId)
    {
        return await service.UpdatePurposeCategoryAsync(url, purposeCategoryId);
    }
    
    /// <summary>
    /// 編輯作品名稱
    /// </summary>
    [HttpPatch("seriesName/{seriesName}")]
    public async Task<SurugayaCategoryModel> UpdateSeriesName(string url, string seriesName)
    {
        return await service.UpdateSeriesNameAsync(url, seriesName);
    }
    
    /// <summary>
    /// 編輯目的與作品名稱
    /// </summary>
    /// <returns></returns>
    [HttpPatch("purposeAndSeries/{purposeCategory}/{seriesName}")]
    public async Task<SurugayaCategoryModel> UpsertPurposeAndSeries([FromBody]string url, PurposeCategoryEnum purposeCategory, string seriesName)
    {
        return await service.UpsertPurposeAndSeriesAsync(url, purposeCategory, seriesName);
    }
}
