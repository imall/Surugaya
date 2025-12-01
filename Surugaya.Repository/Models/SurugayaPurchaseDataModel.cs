using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Surugaya.Repository.Models;

[Table("SurugayaPurchase")]
public class SurugayaPurchaseDataModel : BaseModel
{
    [PrimaryKey("id", false)]
    [Column("id")]
    public long Id { get; set; }
    
    [Column("url")]
    public string Url { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("note")]
    public string? Note { get; set; }
}
