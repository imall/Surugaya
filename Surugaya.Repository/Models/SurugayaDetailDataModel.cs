using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Surugaya.Repository.Models;

[Table("SurugayaDetails")]
public class SurugayaDetailDataModel : BaseModel
{
    [PrimaryKey("url")]
    [Column("url")]
    public string Url { get; set; } 

    [Column("title")]
    public string Title { get; set; }

    [Column("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;

    [Column("currentPrice")]
    public decimal CurrentPrice { get; set; }

    [Column("salePrice")]
    public decimal? SalePrice { get; set; }

    [Column("status")]
    public string Status { get; set; } = string.Empty;

    [Column("lastUpdated")]
    public DateTime LastUpdated { get; set; }
}
