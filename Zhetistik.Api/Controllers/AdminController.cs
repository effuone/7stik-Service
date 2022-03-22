using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Zhetistik.Data.AuthModels;
using Zhetistik.Data.Roles;

namespace Zhetistik.Api.Controllers
{
    [ApiController]
    [Route("api/admins")]
    public class AdminController : ControllerBase
    {
        private readonly ZhetistikAppContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICandidateRepository _candidateRepository;
        private readonly UserManager<ZhetistikUser> _userManager;
        private readonly SignInManager<ZhetistikUser> _signInManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ZhetistikAppContext dbContext, RoleManager<IdentityRole> roleManager, ICandidateRepository candidateRepository, UserManager<ZhetistikUser> userManager, SignInManager<ZhetistikUser> signInManager, ILogger<AdminController> logger)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _candidateRepository = candidateRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]  
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
            await _userManager.AddToRoleAsync(user, "Admin");
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
        [HttpPost("roles")]
        // [Authorize(Roles = "Admin")]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddRoleAsync(string role)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                return StatusCode(StatusCodes.Status406NotAcceptable, new Response{Status = "Error", Message=$"Role of {role} already exists"});
            }
            await _roleManager.CreateAsync(new IdentityRole(role)); 
            return StatusCode(StatusCodes.Status202Accepted, new Response{Status = "Success", Message=$"Successfully created role of {role}"});
        }
    }
}