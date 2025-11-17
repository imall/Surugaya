using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Surugaya.Common.Models;

public class SurugayaDetailModel
{
    public string Url { get; set; } 
    public string Title { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}
