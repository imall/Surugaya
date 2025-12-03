using Microsoft.AspNetCore.Mvc;
using Surugaya.Common.Models;
using Surugaya.Repository.Models;
using Surugaya.Service;

namespace Surugaya.API.Controllers;

/// <summary>
/// 系列名稱對應管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SeriesNameMappingController(SeriesNameMappingService service) : ControllerBase
{
    /// <summary>
    /// 取得所有系列名稱對應
    /// </summary>
    [HttpGet]
    [ProducesResponseType<Dictionary<string, string>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<Dictionary<string, string>>> GetAllMappings()
    {
        try
        {
            var mappings = await service.GetAllMappingsAsync();
            return Ok(mappings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"取得系列名稱對應失敗: {ex.Message}" });
        }
    }

    /// <summary>
    /// 新增系列名稱對應
    /// </summary>
    [HttpPost]
    [ProducesResponseType<SeriesNameMappingModel>(StatusCodes.Status201Created)]
    public async Task<ActionResult<SeriesNameMappingModel>> AddMapping([FromBody] AddMappingRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.JapaneseKey))
                return BadRequest(new { message = "日文關鍵字不可為空" });

            if (string.IsNullOrWhiteSpace(request.ChineseName))
                return BadRequest(new { message = "中文名稱不可為空" });

            var result = await service.AddMappingAsync(request.JapaneseKey, request.ChineseName);
            return CreatedAtAction(nameof(GetAllMappings), result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"新增系列名稱對應失敗: {ex.Message}" });
        }
    }
    
    /// <summary>
    /// 刪除系列名稱對應，依照中文作品名稱
    /// </summary>
    [HttpDelete("{chineseName}")]
    public async Task<ActionResult> DeleteMapping(string chineseName)
    {
        try
        {
            await service.DeleteMappingAsync(chineseName);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"刪除系列名稱對應失敗: {ex.Message}" });
        }
    }
}

/// <summary>
/// 新增系列名稱對應請求
/// </summary>
/// <param name="JapaneseKey">日文關鍵字</param>
/// <param name="ChineseName">中文名稱</param>
public record AddMappingRequest(string JapaneseKey, string ChineseName);

/// <summary>
/// 更新系列名稱對應請求
/// </summary>
/// <param name="ChineseName">中文名稱</param>
public record UpdateMappingRequest(string ChineseName);
