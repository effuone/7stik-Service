// using AutoMapper;
// using Microsoft.Extensions.Configuration;
// using Zhetistik.Data.AuthModels;
// using System.Linq;
// using Zhetistik.Core.Helpers;
// using System.Text;

// namespace Zhetistik.Core.Repositories
// {
//     public class UserService : IUserService
//     {
//         private readonly IUserRepository _userRepository;
//         private readonly IConfiguration _configuration;
//         private readonly IMapper _mapper;

//         public UserService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
//         {
//             _userRepository = userRepository;
//             _configuration = configuration;
//             _mapper = mapper;
//         }
//         public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model)
//         {
//             var user = (await _userRepository
//                 .GetAllAsync())
//                 .FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);

//             if (user == null)
//             {
//                 return null;
//             }

//             var token = _configuration.GenerateJwtToken(user);

//             return new AuthenticateResponse(user, token);
//         }

//         public async Task<IEnumerable<User>> GetAllAsync()
//         {
//             return await _userRepository.GetAllAsync();
//         }

//         public async Task<User> GetByIdAsync(int id)
//         {
//             return await _userRepository.GetAsync(id);
//         }

//         public async Task<AuthenticateResponse> RegisterAsync(UserModel userModel)
//         {
//             var user = _mapper.Map<User>(userModel);

//             var addedUser = await _userRepository.CreateAsync(user);

//             var response = await AuthenticateAsync(new AuthenticateRequest
//             {
//                 Email = user.Email,
//                 Password = user.Password
//             });
            
//             return response;
//         }
//     }
// }