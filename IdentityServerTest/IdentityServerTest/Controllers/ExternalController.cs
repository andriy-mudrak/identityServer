//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using IdentityModel;
//using IdentityServer4.Services;
//using IdentityServer4.Stores;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//namespace IdentityServerTest.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    public class LinkedinController : ControllerBase
//    {
//        private readonly UserManager<AppUser> _userManager;
//        private readonly SignInManager<AppUser> _signInManager;
//        private readonly IIdentityServerInteractionService _interaction;
//        private readonly IClientStore _clientStore;
//        private readonly IEventService _events;

//        public LinkedinController(
//            IIdentityServerInteractionService interaction,
//            IClientStore clientStore,
//            IEventService events,
//            UserManager<AppUser> userManager,
//            SignInManager<AppUser> signInManager
//        )
//        {
//            _interaction = interaction;
//            _clientStore = clientStore;
//            _events = events;

//            _userManager = userManager;
//            _signInManager = signInManager;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Callback()
//        {
//            // read external identity from the temporary cookie
//            var result =
//                await HttpContext.AuthenticateAsync(IdentityServer4.IdentityServerConstants
//                    .ExternalCookieAuthenticationScheme);
//            if (result?.Succeeded != true)
//            {
//                throw new Exception("External authentication error");
//            }

//            var extPrincipal = result.Principal;
//            var expProperties = result.Properties;
//            var claims = extPrincipal.Claims.ToList();

//            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
//            if (userIdClaim == null)
//            {
//                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
//            }

//            if (userIdClaim == null)
//            {
//                throw new Exception("Unknown userid");
//            }

//            claims.Remove(userIdClaim);
//            var provider = expProperties.Items["scheme"];
//            var userId = userIdClaim.Value;

//            var user = await _userManager.FindByLoginAsync(provider, userId);
//            if (user == null)
//            {
//                // Register if user doesn't exist
//                var candidateId = Guid.NewGuid();
//                user = new AppUser();
//                var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);
//                if (emailClaim != null)
//                {
//                    user.UserName = emailClaim.Value;
//                    user.Email = emailClaim.Value;
//                    user.EmailConfirmed = true;
//                }

//                var firstNameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
//                if (firstNameClaim != null)
//                {
//                    user.Name = firstNameClaim.Value;
//                }

//                var lastNameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
//                //if (lastNameClaim != null)
//                //{
//                //    user.LastName = lastNameClaim.Value;
//                //}
//                //user. = EnumUserType.Audience;
//                var createResult = await _userManager.CreateAsync(user);
//                if (createResult.Succeeded)
//                {
//                    var loginResult =
//                        await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userId, provider));
//                    if (loginResult.Succeeded)
//                    {
//                        // Update Tokens to AspNet Identity
//                        var externalLoginInfo = new ExternalLoginInfo(extPrincipal, provider, userId, provider)
//                        {
//                            AuthenticationTokens = expProperties.GetTokens()
//                        };
//                        await _signInManager.UpdateExternalAuthenticationTokensAsync(externalLoginInfo);
//                    }
//                }
//            }

//            return Ok("Not Empty");
//        }
//    }
//}