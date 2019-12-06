using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerTest
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = IdentityServerTest.Roles.Staff, NormalizedName = IdentityServerTest.Roles.Staff.ToUpper() },
                new IdentityRole { Name = IdentityServerTest.Roles.Manager, NormalizedName = IdentityServerTest.Roles.Manager.ToUpper() },
                new IdentityRole { Name = IdentityServerTest.Roles.Admin, NormalizedName = IdentityServerTest.Roles.Admin.ToUpper() });
        }
    }
}