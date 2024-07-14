﻿using Microsoft.AspNetCore.Mvc;
using CustomCADs.API.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using CustomCADs.API.Helpers;
using static CustomCADs.API.Helpers.Utilities;
using CustomCADs.Infrastructure.Data.Models.Identity;

namespace CustomCADs.API.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    public class IdentityController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : ControllerBase
    {
        [HttpPost("Register/{role}")]
        [Consumes("application/json")]
        public async Task<ActionResult> Register([FromRoute] string role, [FromBody] UserRegisterModel model)
        {
            AppUser user = new()
            {
                UserName = model.Username,
                Email = model.Email
            };
            IdentityResult result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            if (!(role == "Client" || role == "Contributor"))
            {
                return BadRequest();
            }
            await userManager.AddToRoleAsync(user, role);
            await user.SignInAsync(signInManager, GetAuthProps(false));

            return Ok("Welcome!");
        }

        [HttpPost("Login")]
        [Consumes("application/json")]
        public async Task<ActionResult> Login([FromBody] UserLoginModel model)
        {
            AppUser? user = await userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                return BadRequest("Invalid Username.");
            }

            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized("Invalid Password.");
            }

            await user.SignInAsync(signInManager, GetAuthProps(model.RememberMe));

            return Ok("Welcome back!");
        }

        [HttpPost("Logout")]
        public async Task<ActionResult> Logout()
        {
            try
            {
                await signInManager.Context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok("Bye-bye.");
            }
            catch
            {
                return BadRequest("You're still here?");
            }
        }

        [HttpGet("IsAuthenticated")]
        public ActionResult IsAuthenticated() => Ok(User.Identity?.IsAuthenticated ?? false);

        [HttpGet("IsAuthorized")]
        public ActionResult IsAuthorized(string role) => Ok(User.IsInRole(role));

        [HttpGet("UserExists")]
        public ActionResult Exists() => Ok(userManager.Users.Any(u => User.Identity!.Name == u.UserName));

        [HttpGet("GetUserRole")]
        public async Task<ActionResult> GetUserRole()
        {
            AppUser? user = await userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            IEnumerable<string> roles = await userManager.GetRolesAsync(user);
            try
            {
                string role = roles.Single();

                return Ok(role);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}