using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Surugaya.Repository.Models;

[Table("SurugayaPurchase")]
public class SurugayaPurchase : BaseModel
{
  [PrimaryKey("id", true)]
  [Column("id")]
  public long Id { get; set; }

  [Column("url")]
  public string Url { get; set; } = string.Empty;

  [Column("date")]
  public DateTime Date { get; set; }

  [Column("note")]
  public string? Note { get; set; }
}
