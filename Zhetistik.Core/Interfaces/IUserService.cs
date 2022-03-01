using Zhetistik.Data.AuthModels;

namespace Zhetistik.Core.Interfaces
{
    public interface IUserService
    {
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model);
        Task<AuthenticateResponse> RegisterAsync(UserModel userModel);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
    }
}