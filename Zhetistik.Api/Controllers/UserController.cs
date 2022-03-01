// using Zhetistik.Core.Helpers;
// using Zhetistik.Data.AuthModels;

// namespace Zhetistik.Api.Controllers
// {
//     [ApiController]
//     [Route("api/users/")]
//     public class UserController : ControllerBase
//     {
//         private readonly IUserService _userService;
//         private readonly ILogger<UserController> _logger;

//         public UserController(IUserService userService, ILogger<UserController> logger)
//         {
//             _userService = userService;
//             _logger = logger;
//         }

//         [HttpPost("authenticate")]
//         public async Task<IActionResult> Authenticate(AuthenticateRequest model)
//         {
//             var response = await _userService.AuthenticateAsync(model);

//             if (response == null)
//                 return BadRequest(new { message = "Username or password is incorrect" });

//             return Ok(response);
//         }

//         [HttpPost("register")]
//         public async Task<IActionResult> Register(CreateUserModel userModel)
//         {
//             var newUser = new UserModel();
//             newUser.Email = userModel.Email;
//             newUser.FirstName = userModel.FirstName;
//             newUser.LastName = userModel.LastName;
//             if(userModel.Password != userModel.ConfirmPassword)
//             {
//                 return BadRequest("Passwords aren't matching.");
//             }
//             newUser.Password = userModel.Password;
//             var response = await _userService.RegisterAsync(newUser);

//             if (response == null)
//             {
//                 return BadRequest(new {message = "Didn't register!"});
//             }

//             return Ok(response);
//         }

//         [Authorize]
//         [HttpGet]
//         public async Task<IActionResult> GetAll()
//         {
//             var users = await _userService.GetAllAsync();
//             return Ok(users);
//         }
//     }
// }