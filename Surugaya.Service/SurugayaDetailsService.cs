using Surugaya.Common.Models;
using Surugaya.Repository;
using Surugaya.Repository.Models;

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
                LastUpdated = x.LastUpdated,
                PurposeCategory = x.PurposeCategory.ToString()
            };
        });
    }

    public async Task<SurugayaDetailModel> UpdatePurposeCategoryAsync(int url, PurposeCategoryEnum purposeCategory)
    {
        var dto = await detailsRepository.UpdatePurposeCategoryAsync(url, purposeCategory);
        var uri = new Uri(dto.Url);
        return new SurugayaDetailModel
        {
            Id = int.Parse(uri.Segments.Last()),
            Url = dto.Url,
            Title = dto.Title,
            ImageUrl = dto.ImageUrl,
            CurrentPrice = dto.CurrentPrice,
            SalePrice = dto.SalePrice,
            Status = dto.Status,
            LastUpdated = dto.LastUpdated,
            PurposeCategory = dto.PurposeCategory.ToString(),
            SeriesName = dto.SeriesName
        };
    }
    
    public async Task<SurugayaDetailModel> UpdateSeriesNameAsync(int id, string seriesName)
    {
        var dto = await detailsRepository.UpdateSeriesNameAsync(id, seriesName);
        var uri = new Uri(dto.Url);
        return new SurugayaDetailModel
        {
            Id = int.Parse(uri.Segments.Last()),
            Url = dto.Url,
            Title = dto.Title,
            ImageUrl = dto.ImageUrl,
            CurrentPrice = dto.CurrentPrice,
            SalePrice = dto.SalePrice,
            Status = dto.Status,
            LastUpdated = dto.LastUpdated,
            PurposeCategory = dto.PurposeCategory.ToString(),
            SeriesName = dto.SeriesName
        };
    }
}
