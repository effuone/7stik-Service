namespace Zhetistik.Core.Interfaces
{
    public interface IAsyncRepository<T> where T:class
    {
        public Task<T> GetAsync(int id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<int> CreateAsync(T model);
        public Task<bool> UpdateAsync(int id, T model);
        public Task<bool> DeleteAsync(int id);
    }
}