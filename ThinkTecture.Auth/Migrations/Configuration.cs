using Microsoft.AspNet.Identity.EntityFramework;
using ThinkTecture.Auth.Identity;
using ThinkTecture.Common.Identity;

namespace ThinkTecture.Auth.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            // User
            context.Users.AddOrUpdate(u => u.Id,
                new ApplicationUser
                {
                    Id = "41d3d994-166a-4bd4-a68e-da9e264f9716",
                    Email = "umair.ali@synergy-it.pk",
                    EmailConfirmed = true,
                    PasswordHash = "AEIxmvgxfHcl4/aYb5/jxUBlSlRyJQSKxe+BEHVHcNQ3fHABnkL1dKANFTDMPJNUuw==",
                    SecurityStamp = "f03305aa-3d58-46f0-89f5-03dfe583e050",
                    UserName = "itua.synergy",
                    UserType = 1,
                    FirstName = "Umair",
                    LastName = "Ali",
                    IsActive = true,
                    IsDeleted = false
                });

            // Roles
            context.Roles.AddOrUpdate(r => r.Id, 
                new ApplicationRole
                {
                    Id = "c9475bc2-3072-46ce-a4e1-ebd018e45f4a",
                    Name = ApplicationRoleTypes.Admin
                },
                new ApplicationRole
                {
                    Id = "a71f7a13-b026-4226-82e6-ce4e7a7035b2",
                    Name = ApplicationRoleTypes.Developer
                });

            // UserRole
            context.UserRoles.AddOrUpdate(ur => new { ur.UserId, ur.RoleId },
                new IdentityUserRole
                {
                    UserId = "41d3d994-166a-4bd4-a68e-da9e264f9716",
                    RoleId = "c9475bc2-3072-46ce-a4e1-ebd018e45f4a"
                },
                new IdentityUserRole
                {
                    UserId = "41d3d994-166a-4bd4-a68e-da9e264f9716",
                    RoleId = "a71f7a13-b026-4226-82e6-ce4e7a7035b2"
                });

            // UserClaim
            context.UserClaims.AddOrUpdate(uc => uc.Id,
                new IdentityUserClaim
                {
                    Id = 1,
                    UserId = "41d3d994-166a-4bd4-a68e-da9e264f9716",
                    ClaimType = ApplicationClaimTypes.CallApi,
                    ClaimValue = ApplicationClaimValues.Read
                },
                new IdentityUserClaim
                {
                    Id = 2,
                    UserId = "41d3d994-166a-4bd4-a68e-da9e264f9716",
                    ClaimType = ApplicationClaimTypes.Values,
                    ClaimValue = ApplicationClaimValues.Read
                });
        }
    }
}
