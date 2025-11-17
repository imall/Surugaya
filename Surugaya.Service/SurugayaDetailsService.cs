using Surugaya.Common.Models;
using Surugaya.Repository;

namespace Surugaya.Service;

public class SurugayaDetailsService(SurugayaDetailsRepository detailsRepository, SurugayaRepository repository)
{
    public async Task<IEnumerable<SurugayaDetailModel>> GetAllInUrlAsync()
    {
        var dto = await repository.GetAllSurugayaAsync();

        var urls = dto.Select(x => x.ProductUrl);

        var details = await detailsRepository.GetAllInUrlAsync(urls);

        return details.Select(x =>
        {
            var uri = new Uri(x.Url);
            return new SurugayaDetailModel
            {
                Id = int.Parse(uri.Segments.Last()),
                Url = x.Url,
                Title = x.Title,
                ImageUrl = x.ImageUrl,
                CurrentPrice = x.CurrentPrice,
                SalePrice = x.SalePrice,
                Status = x.Status,
                LastUpdated = x.LastUpdated
            };
        });
    }
}
