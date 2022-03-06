using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Zhetistik.Data.AuthModels;
using Zhetistik.Data.Roles;

namespace Zhetistik.Api.Controllers
{
    [Route("api/account/")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ZhetistikAppContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ZhetistikUser> _userManager;
        private readonly SignInManager<ZhetistikUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ZhetistikAppContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<ZhetistikUser> userManager, SignInManager<ZhetistikUser> signInManager, IConfiguration configuration, ILogger<AccountController> logger)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] LoginModelType loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);  
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))  
            {  
                var userRoles = await _userManager.GetRolesAsync(user);  
  
                var authClaims = new List<Claim>  
                {  
                    new Claim(ClaimTypes.Name, user.UserName),  
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  
                };  
  
                foreach (var userRole in userRoles)  
                {  
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));  
                }  
  
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]));  
  
                var token = new JwtSecurityToken(  
                    issuer: _configuration["JwtConfig:ValidIssuer"],  
                    audience: _configuration["JwtConfig:ValidAudience"],  
                    expires: DateTime.Now.AddHours(3),  
                    claims: authClaims,  
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)  
                    );  
  
                return Ok(new  
                {  
                    token = new JwtSecurityTokenHandler().WriteToken(token),  
                    expiration = token.ValidTo  
                });  
            }  
            return Unauthorized();  
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync([FromBody] RegisterModelType registerModel)
        {
            var userExists = await _userManager.FindByEmailAsync(registerModel.Email);  
            if (userExists != null)  
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });  
  
            ZhetistikUser user = new ZhetistikUser()  
            {  
                Email = registerModel.Email,  
                SecurityStamp = Guid.NewGuid().ToString(),  
                UserName = registerModel.UserName,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                PhoneNumber = registerModel.PhoneNumber  
            };  
            var result = await _userManager.CreateAsync(user, registerModel.Password);  
            if (!result.Succeeded)  
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
  
            return Ok(new Response { Status = "Success", Message = "User created successfully!" }); 
        }
        [HttpPost]  
        [Route("register-admin")]  
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModelType model)  
        {  
            var userExists = await _userManager.FindByNameAsync(model.UserName);  
            if (userExists != null)  
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });  
  
            ZhetistikUser user = new ZhetistikUser()  
            {  
                Email = model.Email,  
                SecurityStamp = Guid.NewGuid().ToString(),  
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber  
            };  
            var result = await _userManager.CreateAsync(user, model.Password);  
            if (!result.Succeeded)  
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
  
            if (!await _roleManager.RoleExistsAsync(UserRole.Admin))  
                await _roleManager.CreateAsync(new IdentityRole(UserRole.Admin)); 
            if (!await _roleManager.RoleExistsAsync(UserRole.User))  
                await _roleManager.CreateAsync(new IdentityRole(UserRole.User));  
  
            if (await _roleManager.RoleExistsAsync(UserRole.Admin))  
            {  
                await _userManager.AddToRoleAsync(user, UserRole.Admin);  
            }  
  
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });  
        }  
    }
}