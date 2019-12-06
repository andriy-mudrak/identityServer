using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Services;
using IdentityServerTest.Helpers;
using IdentityServerTest.Helpers.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServerTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")));



            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IProfileService, IdentityClaimsProfileService>();
            services.AddTransient<ILinkedinHelpers, LinkedinHelpers>();

            //services.AddAuthentication().AddLinkedIn(options =>
            //{
            //    options.ClientId = Configuration["Authentication:LinkedIn:ClientId"];
            //    options.ClientSecret = Configuration["Authentication:LinkedIn:ClientSecret"];
            //    options.Scope.Add("r_liteprofile");
            //});

            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                    googleOptions.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                    googleOptions.Events.OnCreatingTicket = (context) =>
                    {
                        context.Identity.AddClaim(new Claim("image", context.User.GetValue("image").SelectToken("url").ToString()));

                        return Task.CompletedTask;
                    };
                    //googleOptions.ClaimActions.Clear();
                    //googleOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    //googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    //googleOptions.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                    //googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                    //googleOptions.ClaimActions.MapJsonKey("urn:google:profile", "link");
                    //googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    //googleOptions.ClaimActions.MapJsonKey("urn:google:image", "picture");
                })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = Configuration["Authentication:Facebook:ClientId"];
                    facebookOptions.AppSecret = Configuration["Authentication:Facebook:ClientSecret"];
                })
                .AddLinkedIn(linkedinOptions =>
                {
                    linkedinOptions.ClientId = Configuration["Authentication:Linkedin:ClientId"];
                    linkedinOptions.ClientSecret = Configuration["Authentication:Linkedin:ClientSecret"];
                });

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                            builder.UseSqlServer(Configuration.GetConnectionString("Default"));
                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                        options.TokenCleanupInterval = 30; // interval in seconds
                    })
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddAspNetIdentity<AppUser>()
                .AddProfileService<IdentityClaimsProfileService>();


            //services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader()));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors("AllowAll");
            
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc();
        }

    }
}
