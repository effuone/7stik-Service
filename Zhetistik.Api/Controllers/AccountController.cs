using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Zhetistik.Data.AuthModels;
using Zhetistik.Data.MailAccess;
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
        private readonly IMailSender _mailSender;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ZhetistikAppContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<ZhetistikUser> userManager, SignInManager<ZhetistikUser> signInManager, IConfiguration configuration, IMailSender mailSender, ILogger<AccountController> logger)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mailSender = mailSender;
            _logger = logger;
        }
        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<ActionResult<UserViewModel>> GetUserClaimsByToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            SecurityToken jsonToken;
            try
            {
                jsonToken = handler.ReadToken(token);
            }
            catch (System.Exception)
            {
                return NotFound("Unexisting token");
            }
            var tokens = jsonToken as JwtSecurityToken;
            var userName = tokens.Claims.First(x=>x.Type=="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").Value;
            var user =  await _userManager.FindByNameAsync(userName);
            if(user is null)
            {
                return NotFound();
            }
            var userVm = new UserViewModel();
            userVm.Username = user.UserName;
            userVm.FirstName = user.FirstName;
            userVm.LastName = user.LastName;
            userVm.Email = user.Email;
            userVm.PhoneNumber = user.PhoneNumber;
            return userVm;
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
                    var claim = new Claim($"Role {i+1}", userRoles[i]);
                    roleClaims.Add(claim);
                }
                var authClaims = new List<Claim>  
                {
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
            if(await _roleManager.RoleExistsAsync(registerModel.Role) is false)
            {
                return NotFound("Role not found");
            }
            var result = await _userManager.CreateAsync(user, registerModel.Password);  
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, registerModel.Role);
                    //Email Confirmation
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var request = new MailRequest();
                    var callbackUrl = Url.Action(
                        "ConfirmEmailAsync",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: HttpContext.Request.Scheme);
                    request.Subject = "7stik email confirmation!";
                    request.Body = 
                    @$"<form action='{callbackUrl}' method='POST'> 
                        <div>
                            <button><h1>Confirm email</h1></button>
                        </div>
                      </form>";
                    request.ToEmail = registerModel.Email;
                    await _mailSender.SendEmailAsync(request);
                }  
                
                if (!result.Succeeded)  
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
    
                return Ok(new Response { Status = "Success", Message = "Now confirm your email" });
        }
        [HttpPost("confirmEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmailAsync(string userId, string code)
        {
            if (userId == null || code == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded)
            {
                return Ok(new Response { Status = "Success", Message = "Email confirmed successfully!" });
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });  
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
        // [HttpGet("getRoles")]
        // public async Task<bool> GetAllRolesAsync(string role)
        // {
        //     return await _roleManager.RoleExistsAsync(role);
        // }
    }
}