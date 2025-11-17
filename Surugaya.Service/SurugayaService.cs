using Surugaya.Common.Models;
using Surugaya.Repository;
using Surugaya.Repository.Models;

namespace Surugaya.Service;


/// <summary>
/// 處理網址
/// </summary>
/// <param name="repo"></param>
public class SurugayaService(SupabaseRepository repo, SurugayaScraperService service)
{
    public async Task<SurugayaDetailModel> InsertSurugayaAsync(CreateSurugayaParameter parameter)
    {
        var existing = await repo.IsUrlExistAsync(parameter.ProductUrl);
        if (existing)
        {
            throw new Exception("該 ProductUrl 已存在，無法重複插入。");
        }

        var surugaya = new SurugayaDataModel
        {
            ProductUrl = parameter.ProductUrl,
            CreatedAt = DateTime.Now
        };

        var dto = await repo.InsertSurugayaAsync(surugaya);
        
        var result = await service.ScrapeProductInfoByUrl(dto.ProductUrl);

        return result;
    }

    public async Task<IEnumerable<SurugayaModel>> GetAllSurugayaAsync()
    {
        var dto = await repo.GetAllSurugayaAsync();

        return dto.Select(x => new SurugayaModel()
        {
            ProductUrl = x.ProductUrl,
            CreateAt = x.CreatedAt
        });
    }

    public async Task<SurugayaModel> GetSurugayaByUrlAsync(string url)
    {
        var dto = await repo.GetSurugayaByUrlAsync(url);

        if (dto == null)
        {
            throw new Exception("找不到指定 ID 的資料。");
        }

        return new SurugayaModel()
        {
            ProductUrl = dto.ProductUrl,
            CreateAt = dto.CreatedAt
        };
    }

    public async Task<bool> IsUrlExistAsync(string url)
    {
        return await repo.IsUrlExistAsync(url);
    }
    
    public async Task DeleteFromUrlAsync(string url)
    {
        await repo.DeleteFromUrlAsync(url);
    }
}
