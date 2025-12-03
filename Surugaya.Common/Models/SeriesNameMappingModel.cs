using System.Text.Json.Serialization;

namespace Surugaya.Common.Models;

/// <summary>
/// 系列名稱對應 DTO
/// </summary>
public class SeriesNameMappingModel
{
    /// <summary>
    /// 對應 ID
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
    public long? Id { get; set; }
    
    /// <summary>
    /// 日文關鍵字
    /// </summary>
    public string JapaneseKey { get; set; } = string.Empty;
    
    /// <summary>
    /// 中文作品名稱
    /// </summary>
    public string ChineseName { get; set; } = string.Empty;
}
