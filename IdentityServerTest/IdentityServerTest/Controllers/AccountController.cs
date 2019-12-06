using Microsoft.AspNetCore.Mvc;
using AuthServer.Models;
using System.Threading.Tasks;
using IdentityServer4.Services;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Claims;
using IdentityServer4.Stores;
using IdentityServerTest;
using IdentityServerTest.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using Newtonsoft.Json.Linq;
using IdentityServerTest.Helpers.Interfaces;
using IdentityServerTest.Helpers.Password;
using Newtonsoft.Json;

namespace AuthServer.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILinkedinHelpers _linkedinHelpers;
        public AccountController(ILinkedinHelpers linkedinHelpers, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _linkedinHelpers = linkedinHelpers;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        /// 
        [Authorize]
        [HttpGet]
        public IActionResult Test()
        {
            var test = HttpContext.User.Claims;
            var test2 = HttpContext.User.Identity.Name;
            var user = _userManager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            //User user = await _userManager.FindByIdAsync(HttpContext.);
            var roles = _signInManager.UserManager.GetRolesAsync(user).Result;
            var userRoles = _userManager.GetRolesAsync(user);
            var b1 = User.HasClaim("role", "Staff");
            var test3 = User.IsInRole("Staff");
            // получаем все роли
            // var allRoles = _roleManager.Roles.ToList();
            // получаем список ролей, которые были добавлены
            return new JsonResult(User.Claims.Select(c => new { c.Type, c.Value }));
            //   return Ok("Test");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin")]
        public IActionResult Test1()
        {
            return Ok("Admin");
        }

        [Authorize(Roles = "Staff")]
        [HttpGet]
        [Route("Staff")]
        public IActionResult Test2()
        {
            //var test = HttpContext.User.Claims;
            //var test2 = HttpContext.User.Identity.Name;
            //var user = _userManager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            ////User user = await _userManager.FindByIdAsync(HttpContext.);
            //var roles = _signInManager.UserManager.GetRolesAsync(user).Result;
            //var userRoles = _userManager.GetRolesAsync(user);
            //// получаем все роли
            //var allRoles = _roleManager.Roles.ToList();
            // получаем список ролей, которые были добавлены
            return Ok("Staff");
        }

        [HttpGet]
        [Route("All")]
        [Authorize(Roles = "Admin, Staff")]
        public IActionResult Test3()
        {
            return Ok("All");
        }
        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        ///
        [AllowAnonymous]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody]LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    var userClaims = await this._userManager.GetClaimsAsync(user);
                    
                    await HttpContext.SignInAsync("Cookies", userClaims.ToArray());
                    return Ok(new { user = model });
                }
            }

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser { UserName = model.Email, Name = model.Name, Email = model.Email };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, model.Role);
            await _userManager.AddClaimAsync(user,
                new System.Security.Claims.Claim(ClaimConstants.USERNAME, user.UserName));
            await _userManager.AddClaimAsync(user,
                new System.Security.Claims.Claim(ClaimConstants.NAME, user.Name));
            await _userManager.AddClaimAsync(user,
                new System.Security.Claims.Claim(ClaimConstants.EMAIL, user.Email));
            await _userManager.AddClaimAsync(user,
                new System.Security.Claims.Claim(ClaimConstants.ROLE, RoleConstants.DEFAULT_ROLE));

            return Ok(new RegisterResponseViewModel(user));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Linkedin/Login")]
        public async Task<IActionResult> LinkedinLogin()
        {
            //string uri =
            //    "https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id=777f88gmg9ekka&scope=r_emailaddress%20w_member_social%20r_liteprofile&redirect_uri=https://localhost:44364/api/Account/LinkedinRedirect&state=aRandomString";
            
            return Ok(_linkedinHelpers.LinkedinLogin());
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("LinkedinRedirect")]
        public async Task<IActionResult> LinkedinRedirect([FromQuery] string code, [FromQuery] string state)
        {
            var accessToken = _linkedinHelpers.AccessTokenRequest(code);
            if (accessToken == null) return BadRequest(new
            {
                Error = "User dismissed the authorization"
            });

            var personModel = await _linkedinHelpers.PersonRequest(accessToken);
            var email = await _linkedinHelpers.EmailRequest(accessToken);

            var user = await _userManager.FindByNameAsync(email);

            if (user != null)
            {

                var userClaims = await this._userManager.GetClaimsAsync(user);
                await HttpContext.SignInAsync("Cookies", userClaims.ToArray());
                return Ok(new { user = user });
            }
            else
            {
                var userRegistrationModel = new AppUser
                    {UserName = email, Name = personModel.localizedFirstName, Email = email};
                var password = PasswordGenerating.GeneratePassword();
                var result = await _userManager.CreateAsync(userRegistrationModel, password);

                if (!result.Succeeded) return BadRequest(result.Errors);

                await _userManager.AddToRoleAsync(userRegistrationModel, RoleConstants.DEFAULT_ROLE);
                await _userManager.AddClaimAsync(userRegistrationModel,
                    new System.Security.Claims.Claim(ClaimConstants.USERNAME, userRegistrationModel.UserName));
                await _userManager.AddClaimAsync(userRegistrationModel,
                    new System.Security.Claims.Claim(ClaimConstants.NAME, userRegistrationModel.Name));
                await _userManager.AddClaimAsync(userRegistrationModel,
                    new System.Security.Claims.Claim(ClaimConstants.EMAIL, userRegistrationModel.Email));
                await _userManager.AddClaimAsync(userRegistrationModel,
                    new System.Security.Claims.Claim(ClaimConstants.ROLE, RoleConstants.DEFAULT_ROLE));

                await HttpContext.SignInAsync("Cookies",
                    _userManager.GetClaimsAsync(userRegistrationModel).Result.ToArray());
                return Ok(new {user = user});
            }
        }
    }
}
