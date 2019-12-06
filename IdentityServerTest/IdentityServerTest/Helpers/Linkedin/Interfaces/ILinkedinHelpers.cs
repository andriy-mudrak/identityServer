using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthServer.Models;

namespace IdentityServerTest.Helpers.Interfaces
{
    public interface ILinkedinHelpers
    {
        Task<PersonLinkedinModel> PersonRequest(string accessToken);
        Task<string> EmailRequest(string accessToken);
        string AccessTokenRequest(string code);
        string LinkedinLogin();
    }
}