using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Surugaya.Repository.Models;

[Table("Surugaya")]
public class SurugayaDataModel : BaseModel
{
    [PrimaryKey("productUrl", false)]
    [Column("productUrl")]
    public string ProductUrl { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
