//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;

//namespace IdentityServerTest
//{
//    public class AppUserClaimsPrincipalFactory<TUser, TRole>
//        : UserClaimsPrincipalFactory<TUser, TRole>
//        where TUser : AppUser
//        where TRole : IdentityRole
//    {
//        public AppUserClaimsPrincipalFactory(
//            UserManager<TUser> manager,
//            RoleManager<TRole> rolemanager,
//            IOptions<IdentityOptions> options)
//            : base(manager, rolemanager, options)
//        {
//        }

//        public async override Task<ClaimsPrincipal> CreateAsync(TUser user)
//        {
//            var id = await GenerateClaimsAsync(user);
//            if (user != null)
//            {
//                id.AddClaim(new Claim("Zarquon", user.Name));
//            }
//            return new ClaimsPrincipal(id);
//        }
//    }
//}