namespace Zhetistik.Core.Interfaces
{
    public interface ICityRepository : IAsyncRepository<City>
    {
        public Task<City> GetAsync(string cityName);
        public Task<IEnumerable<City>> GetAllAsync(string countryName);
    }
}