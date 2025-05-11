using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegiterRequest registerRequest)
        {
            ApplicationUser applicationUser = registerRequest.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(applicationUser, registerRequest.Password);

            if (result.Succeeded)
            {
                // Success Register
                await _signInManager.SignInAsync(applicationUser, false);

                if (_roleManager.Roles is not null)
                {
                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Company"));
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));
                }
                await _userManager.AddToRoleAsync(applicationUser, "Customer");

                return Created();
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var appUser = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (appUser != null)
            {
                var result = await _userManager.CheckPasswordAsync(appUser, loginRequest.Password);

                if (result)
                {
                    // Login
                    //await _signInManager.SignInAsync(appUser, loginRequest.RememberMe);

                    var userRoles = await _userManager.GetRolesAsync(appUser);

                    List<Claim> claims =
                    [
                        new Claim(ClaimTypes.Name, appUser.UserName),
                        new Claim(ClaimTypes.NameIdentifier, appUser.Id),
                    ];

                    foreach (var item in userRoles)
                    {
                        claims.Add(new(ClaimTypes.Role, item));
                    }

                    SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes("EraaSoft511$$EraaSoft511&&EraaSoft511"));
                    SigningCredentials signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

                    JwtSecurityToken token = new(
                        issuer: "https://localhost:7021",
                        audience: "https://localhost:4200",
                        claims: claims,
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: signingCredentials
                        );

                    return Ok(
                        new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token)
                        }
                        );
                }
                else
                {
                    ModelStateDictionary keyValuePairs = new();
                    keyValuePairs.AddModelError("Error", "Invalid Data");
                    return BadRequest(keyValuePairs);
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
