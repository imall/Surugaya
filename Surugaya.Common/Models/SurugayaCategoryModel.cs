using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Surugaya.Repository.Models;

namespace Surugaya.Common.Models;

public class SurugayaCategoryModel
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string PurposeCategory { get; set; }
    public string? SeriesName { get; set; }
}
