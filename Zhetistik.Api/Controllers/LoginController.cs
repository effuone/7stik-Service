// using Microsoft.AspNetCore.Authorization;
// using Zhetistik.Data.AuthModels;
// using Zhetistik.Data.ViewModels;

// namespace Zhetistik.Api.Controllers
// {
//     [ApiController]
//     [Route("api/auth/")]
//     public class LoginController : ControllerBase
//     {
//         private IConfiguration _config;

//         public LoginController(IConfiguration config)
//         {
//             _config = config;
//         }
//         [AllowAnonymous]
//         [HttpPost]
//         public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
//         {
//             var user = Authenticate(userLogin);
//             if(user is not null)
//             {
//                 var token = Generate(user);
//                 return Ok();
//             }
//             return NotFound();
//         }

//         private Task<object> Generate(UserModel user)
//         {
//             throw new NotImplementedException();
//         }

//         private Task<UserModel> Authenticate(UserLogin userLogin)
//         {
//             var currentUser = UserConstants.Users.FirstOrDefault(o=>o.Username.ToLower()==
//             userLogin.Username.ToLower() && o.Password == userLogin.Password);

//         }
//     }
// }