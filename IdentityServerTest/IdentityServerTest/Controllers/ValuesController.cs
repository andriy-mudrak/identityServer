using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServerTest;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly SignInManager<AppUser> _signInManager;

        public ValuesController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var loginProviders = _signInManager.GetExternalAuthenticationSchemesAsync().Result.ToList();

            var callbackUrl = Url.Action("ExternalLoginCallback");
            var authenticationProperties = new AuthenticationProperties { RedirectUri = callbackUrl };
            return this.Challenge(authenticationProperties, loginProviders[0].Name);
            //loginProviders[0].HandlerType
            //HttpContext.SignInAsync(loginProviders[0].HandlerType.)//(SignInManager<AppUser>.GetExternalAuthenticationSchemesAsync()).Result.ToList();
           // return loginProviders; //return new string[] { "value1", "value2" };
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return this.Ok(new
            {
                NameIdentifier = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = result.Principal.FindFirstValue(ClaimTypes.Email),
                Picture = result.Principal.FindFirstValue("image")
            });
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}