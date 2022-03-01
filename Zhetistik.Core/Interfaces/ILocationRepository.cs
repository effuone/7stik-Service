global using Zhetistik.Data.Models;
using System.Linq.Expressions;
using Zhetistik.Data.ViewModels;

namespace Zhetistik.Core.Interfaces
{
    public interface ILocationRepository : IAsyncRepository<Location>
    {
         public Task<LocationViewModel> GetViewModelAsync(string countryName, string cityName);
         public Task<Location> GetAsync(int countryId, int cityId);
         public Task<IEnumerable<LocationViewModel>> GetAllViewModelsAsync();
         public Task<LocationViewModel> GetViewModelAsync(int id);
    }
}