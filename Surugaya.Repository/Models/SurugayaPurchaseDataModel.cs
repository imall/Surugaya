using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Surugaya.Repository.Models;

[Table("SurugayaPurchase")]
public class SurugayaPurchaseDataModel : BaseModel
{

  [Column("url")]
  public string Url { get; set; } = string.Empty;

  [Column("date")]
  public DateTime Date { get; set; }

  [Column("note")]
  public string? Note { get; set; }
}
