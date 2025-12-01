using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Surugaya.Repository.Models;

[Table("SurugayaPurchase")]
public class SurugayaPurchase : BaseModel
{
  [Column("url")]
  public string Url { get; set; }

  [Column("date")]
  public DateTime Date { get; set; }

  [Column("note")]
  public string? Note { get; set; }
}
