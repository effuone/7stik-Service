using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Zhetistik.Data.AuthModels;
using Zhetistik.Data.Roles;
using Zhetistik.Data.MailAccess;

namespace Zhetistik.Api.Controllers
{
    [Route("api/account/")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ZhetistikAppContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICandidateRepository _candidateRepository;
        private readonly UserManager<ZhetistikUser> _userManager;
        private readonly SignInManager<ZhetistikUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMailSender _mailSender;
        private readonly ILogger<AccountController> _logger;
        
        private async Task<IEnumerable<string>> GetUsersRoleAsync(ZhetistikUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public AccountController(ZhetistikAppContext dbContext, RoleManager<IdentityRole> roleManager, ICandidateRepository candidateRepository, UserManager<ZhetistikUser> userManager, SignInManager<ZhetistikUser> signInManager, IConfiguration configuration, IMailSender mailSender, ILogger<AccountController> logger)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _candidateRepository = candidateRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mailSender = mailSender;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody]LoginModelType loginModel)
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if(user is null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Wrong password or email" });
            }
            if(user.EmailConfirmed is false)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "Error", Message = "Confirm your email first" });   
            }  
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))  
            {  
                var userRoles = await _userManager.GetRolesAsync(user);  
                var roleClaims = new List<Claim>();
                for (int i = 0; i < userRoles.Count; i++)
                {
                    var claim = new Claim($"Role", userRoles[i]);
                    roleClaims.Add(claim);
                    if(userRoles[i] == "Candidate")
                    {
                        var candidate = await _candidateRepository.GetAsync(user.Id);
                        var candidateClaim = new Claim("CandidateId", candidate.CandidateId.ToString());
                        roleClaims.Add(candidateClaim);
                    }
                }
                var authClaims = new List<Claim>  
                {
                    new Claim("Id", user.Id),
                    new Claim("Username", user.UserName),  
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("Email", user.Email),
                    new Claim("PhoneNumber", user.PhoneNumber)
                };
                    authClaims.AddRange(roleClaims);
                    authClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
  
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
        public async Task<ActionResult<string>> RegisterAsync([FromBody] RegisterModelType registerModel)
        {
            //Check if user exists
            var userExists = await _userManager.FindByEmailAsync(registerModel.Email);  
            if (userExists != null)  
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" }); 
            }
            //Create new user
            ZhetistikUser user = new ZhetistikUser()  
            {  
                Email = registerModel.Email,  
                SecurityStamp = Guid.NewGuid().ToString(),  
                UserName = registerModel.UserName,
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                PhoneNumber = registerModel.PhoneNumber  
            };
            //Check role validity        
            if(await _roleManager.RoleExistsAsync(registerModel.Role) is false)
            {
                return NotFound("Role not found");
            }
            //Update new user in database
            var result = await _userManager.CreateAsync(user, registerModel.Password);  
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, registerModel.Role);
                //Email Confirmation
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var request = new MailRequest();
                var callbackUrl = Url.Action("ConfirmEmailAsync", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                request.Subject = "7stik email confirmation!";
                request.Body = 
                    @$"<form action='{callbackUrl}' method='POST'> 
                        <div>
                            <button><h1>Confirm email</h1></button>
                        </div>
                      </form>";
                request.ToEmail = registerModel.Email;
                await _mailSender.SendEmailAsync(request);
                return user.Id;
            }  
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
            }
        }
        [HttpPost("confirmEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmailAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded)
            {
                return Ok(new Response { Status = "Success", Message = "Email confirmed successfully" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." }); 
            }
        }
        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPasswordAsync(ForgotPasswordModelType forgotPasswordModelType)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordModelType.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return StatusCode(StatusCodes.Status200OK, new Response {Status = "Sent", Message=$"the request to restore the account was sent to the mail {forgotPasswordModelType.Email}"});
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var request = new MailRequest();
            var form = new ResetPasswordModelType();
            form.Email = forgotPasswordModelType.Email;
            var callbackUrl = Url.Action("ResetPasswordAsync","Account",new { userId = user.Id, code = code },protocol: HttpContext.Request.Scheme);
            request.Subject = "7stik email confirmation!";
            request.Body = 
                        @"<form action='/action_page.php'>
            <label for='pword'>Password:</label><br>
            <input type='password' id='fname' name='pword' value='John'><br>
            <label for='confirmP'>Confirm Password:</label><br>
            <input type='password' id='lname' name='lname' value='Doe'><br><br>
            <input type='submit' value='Submit'>
            </form>";
            request.ToEmail = forgotPasswordModelType.Email;
            await _mailSender.SendEmailAsync(request);
            return StatusCode(StatusCodes.Status200OK, new Response {Status = "Sent", Message=$"the request to restore the account was sent to the mail {forgotPasswordModelType.Email}"});
        }
        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassworAsync(ResetPasswordModelType model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response {Status = "Error", Message=$"User not found"});
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded!)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
            }
            return StatusCode(StatusCodes.Status200OK, new Response {Status = "Sent", Message=$"the request to restore the account was sent to the mail {model.Email}"});
        }
        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
        // [HttpGet("getRoles")]
        // public async Task<bool> GetAllRolesAsync(string role)
        // {
        //     return await _roleManager.RoleExistsAsync(role);
        // }
    }
}