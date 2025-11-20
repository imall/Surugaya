using Surugaya.Common.Models;
using Surugaya.Repository;
using Surugaya.Repository.Models;

namespace Surugaya.Service;


/// <summary>
/// 處理網址
/// </summary>
/// <param name="repo"></param>
public class SurugayaService(SurugayaRepository repo,SurugayaDetailsRepository detailsRepository, SurugayaScraperService service)
{
    public async Task<SurugayaDetailModel> InsertSurugayaAsync(CreateSurugayaParameter parameter)
    {
        var existing = await repo.IsUrlExistAsync(parameter.ProductUrl);
        if (existing)
        {
            throw new Exception("該 ProductUrl 已存在，無法重複插入。");
        }

        var surugaya = new Repository.Models.Surugaya
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

        return dto.Select(x =>
        {
            var uri = new Uri(x.ProductUrl);
            return new SurugayaModel()
            {
                Id = int.Parse(uri.Segments.Last()),
                ProductUrl = x.ProductUrl,
                CreateAt = x.CreatedAt
            };
        });
    }
    
    public async Task DeleteFromIdAsync(int id)
    {
        await repo.DeleteFromIdAsync(id);
        await detailsRepository.DeleteFromIdAsync(id);
    }
}
