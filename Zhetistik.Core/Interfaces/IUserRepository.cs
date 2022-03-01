using Zhetistik.Data.AuthModels;

namespace Zhetistik.Core.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> GetAsync(int id);
        public Task<IEnumerable<User>> GetAllAsync();
        public Task<int> CreateAsync(User model);
        public Task<bool> UpdateAsync(long id, User model);
        public Task<bool> DeleteAsync(long id);
    }
}