using Microsoft.AspNetCore.Mvc;
using Surugaya.Common.Models;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

/// <summary>
/// 管理 url 的 controller
/// </summary>
/// <param name="service"></param>
/// <param name="logger"></param>
[ApiController]
[Route("api/[controller]")]
public class SurugayaController(SurugayaService service, ILogger<SurugayaController> logger) : ControllerBase
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
            var result = await service.InsertSurugayaAsync(parameter);

            return CreatedAtAction(nameof(InsertSurugaya), result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "插入 SurugayaDataModel 資料時發生錯誤");
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
            var result = await service.GetAllSurugayaAsync();
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
    public async Task DeleteFromUrl(int id)
    {
        try
        {
            await service.DeleteFromIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
