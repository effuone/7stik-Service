using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Zhetistik.Api.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ZhetistikUser> _userManager;
        private readonly ILogger<RoleController> _logger;
        private readonly ICandidateRepository _candidateRepository;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ZhetistikUser> userManager, ILogger<RoleController> logger, ICandidateRepository candidateRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
            _candidateRepository = candidateRepository;
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
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