namespace Zhetistik.Core.Interfaces
{
    public interface ICountryRepository : IAsyncRepository<Country>
    {
        public Task<Country> GetAsync(string countryName);
    }
}