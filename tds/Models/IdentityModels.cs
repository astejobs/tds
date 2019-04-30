using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace tds.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new MyDBInitializer());
        }
        public class MyDBInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
        {
            protected override void Seed(ApplicationDbContext context)
            {
                // Initialize default identity roles
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                // RoleTypes is a class containing constant string values for different roles
                List<IdentityRole> identityRoles = new List<IdentityRole>();
                identityRoles.Add(new IdentityRole() { Name = "Admin" });
                foreach (IdentityRole role in identityRoles)
                {
                    manager.Create(role);
                }

                // Initialize default user
                var ustore = new UserStore<ApplicationUser>(context);
                var umanager = new UserManager<ApplicationUser>(ustore);
                var admin = new ApplicationUser {Email="admin@admin.com",PhoneNumber="123456789",UserName="admin"};
                umanager.Create(admin, "123456");
                umanager.AddToRole(admin.Id, "Admin");

                // Add code to initialize context tables

                base.Seed(context);

            }
        }












        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}