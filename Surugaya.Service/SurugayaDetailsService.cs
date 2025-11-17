using Surugaya.Common.Models;
using Surugaya.Repository;

namespace Surugaya.Service;

public class SurugayaDetailsService(SurugayaDetailsRepository detailsRepository , SupabaseRepository repository)
{

    public async Task<IEnumerable<SurugayaDetailModel>> GetAllInUrlAsync()
    {
        var dto = await repository.GetAllSurugayaAsync();

        var urls = dto.Select(x => x.ProductUrl);
        
        var details = await detailsRepository.GetAllInUrlAsync(urls);

        return details.Select(x => new SurugayaDetailModel
        {
            Url = x.Url,
            Title = x.Title,
            ImageUrl = x.ImageUrl,
            CurrentPrice = x.CurrentPrice,
            SalePrice = x.SalePrice,
            Status = x.Status,
            LastUpdated = x.LastUpdated
        });
    }
}
