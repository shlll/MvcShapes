namespace project.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using project.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<project.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "project.Models.ApplicationDbContext";
        }

        protected override void Seed(project.Models.ApplicationDbContext context)
        {
            var roleManager =
              new RoleManager<IdentityRole>(
                  new RoleStore<IdentityRole>(context));
            var userManager =
              new UserManager<ApplicationUser>(
                      new UserStore<ApplicationUser>(context));
            if (!context.Roles.Any(p => p.Name == "Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
            }
            
            ApplicationUser adminUser;
       
            if (!context.Users.Any(p => p.UserName == "admin@shape.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@shape.com";
                adminUser.Email = "admin@shape.com";

                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context.Users.First(p => p.UserName == "admin@shape.com");
            }
            

            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            

        }
    }
}
