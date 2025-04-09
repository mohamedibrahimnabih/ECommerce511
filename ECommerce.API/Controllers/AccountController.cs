using ECommerce.API.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ECommerce.API.DTOs.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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

                if (_roleManager.Roles.IsNullOrEmpty())
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
    }
}
