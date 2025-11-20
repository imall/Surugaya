using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Surugaya.Repository.Models;

[Table("SurugayaCategory")]
public class SurugayaCategory : BaseModel
{
    [PrimaryKey("url")]
    [Column("url")]
    public string Url { get; set; }
    
    [Column("purposeCategory")]
    public PurposeCategoryEnum PurposeCategory { get; set; }

    [Column("seriesName")]
    public string? SeriesName { get; set; }
}