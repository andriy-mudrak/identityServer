using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthServer.Models;
using IdentityServerTest.Constants;
using IdentityServerTest.Helpers.Interfaces;
using IdentityServerTest.Helpers.Password;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IdentityServerTest.Helpers
{
    public class LinkedinHelpers : ILinkedinHelpers
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string grantType;
        private readonly string authorization_uri;
        private readonly string accessToken_uri;
        private readonly string redirect_uri;
        private readonly string person_uri;
        private readonly string email_uri;

        public LinkedinHelpers(IConfiguration iconfiguration, UserManager<AppUser> userManager)
        {
            clientId = iconfiguration.GetValue<string>("LinkedIn:ClientId");
            clientSecret = iconfiguration.GetValue<string>("LinkedIn:ClientSecret");
            grantType = iconfiguration.GetValue<string>("LinkedIn:GrantType");
            authorization_uri = iconfiguration.GetValue<string>("LinkedIn:Authorization_uri");
            accessToken_uri = iconfiguration.GetValue<string>("LinkedIn:AccessToken_uri");
            redirect_uri = iconfiguration.GetValue<string>("LinkedIn:Redirect_uri");
            person_uri = iconfiguration.GetValue<string>("LinkedIn:Person_uri");
            email_uri = iconfiguration.GetValue<string>("LinkedIn:Email_uri");
            _userManager = userManager;
        }

        public string LinkedinLogin()
        {
            string uri = authorization_uri;
            return uri; // передаю url щоб користувач через браузер зайшов по ній
        }

        public Task<PersonLinkedinModel> PersonRequest(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, person_uri))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = httpClient.SendAsync(requestMessage);

                    var personObject = JsonConvert.DeserializeObject<PersonLinkedinModel>(response.Result.Content.ReadAsStringAsync().Result);

                    return Task.FromResult(personObject);
                }
            }
        }

        public Task<string> EmailRequest(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, email_uri))
                {
                    requestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = httpClient.SendAsync(requestMessage);


                    var emailObject = JsonConvert.DeserializeObject<EmailModel>(response.Result.Content.ReadAsStringAsync().Result);
                    var email = emailObject.elements[0].handleTilde.emailAddress;
                    return Task.FromResult(email);
                }
            }
        }

        public string AccessTokenRequest(string code)
        {
            var requestBody = new Dictionary<string, string>();
            requestBody.Add("client_id", clientId);
            requestBody.Add("client_secret", clientSecret);
            requestBody.Add("grant_type", grantType);
            requestBody.Add("redirect_uri", redirect_uri);
            requestBody.Add("code", code);
            AccessTokenModel AccessToken;

            using (var httpClient = new HttpClient())
            {
                StringBuilder contents = new StringBuilder();
                using (var content = new FormUrlEncodedContent(requestBody))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    var response = httpClient.PostAsync(accessToken_uri, content);
                    contents.Append(response.Result.Content.ReadAsStringAsync().Result.ToCharArray());
                }
                AccessToken = JsonConvert.DeserializeObject<AccessTokenModel>(contents.ToString());
            }
            return AccessToken.access_token;
        }
    }
}