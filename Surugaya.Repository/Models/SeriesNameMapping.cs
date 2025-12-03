using System.Text.Json.Serialization;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Surugaya.Repository.Models;

[Table("SeriesNameMapping")]
public class SeriesNameMapping : BaseModel
{
    
    [Column("japanese_key")]
    public string JapaneseKey { get; set; } = string.Empty;

    [Column("chinese_name")]
    public string ChineseName { get; set; } = string.Empty;
}
