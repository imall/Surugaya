using Microsoft.AspNetCore.Mvc;
using Surugaya.Common.Models;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

/// <summary>
/// 紀錄願望清單 Url 
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SurugayaUrlsController(SurugayaUrlsService urlsService, ILogger<SurugayaUrlsController> logger) : ControllerBase
{
    /// <summary>
    /// 插入新的 SurugayaDataModel 資料
    /// </summary>
    /// <param name="parameter">包含 ProductUrl 的請求物件</param>
    /// <returns>插入的資料</returns>
    [HttpPost]
    public async Task<ActionResult<SurugayaDetailModel>> InsertSurugaya([FromBody] CreateSurugayaParameter parameter)
    {
        try
        {
            var result = await urlsService.InsertSurugayaAsync(parameter);

            return CreatedAtAction(nameof(InsertSurugaya), result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "插入資料時發生內部錯誤", message = ex.Message });
        }
    }

    /// <summary>
    /// 取得所有 SurugayaDataModel 資料
    /// </summary>
    /// <returns>所有 SurugayaDataModel 資料列表</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SurugayaModel>>> GetAllSurugaya()
    {
        try
        {
            var result = await urlsService.GetAllSurugayaAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "取得資料時發生內部錯誤", message = ex.Message });
        }
    }
    


    /// <summary>
    /// 透過駿河屋商品 id 刪除資料
    /// </summary>
    /// <param name="id">商品 id</param>
    [HttpDelete("{id:int}")]
    public async Task DeleteFromId(int id)
    {
        try
        {
            await urlsService.DeleteFromIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    /// <summary>
    /// 透過駿河屋商品 url 刪除資料
    /// </summary>
    /// <param name="url">商品 url</param>
    [HttpDelete("{url}")]
    public async Task DeleteFromUrl(string url)
    {
        try
        {
            var decodedUrl = Uri.UnescapeDataString(url);
            await urlsService.DeleteFromUrlAsync(decodedUrl);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
