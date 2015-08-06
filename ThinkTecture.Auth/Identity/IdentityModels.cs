using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ThinkTecture.Common.Identity;

namespace ThinkTecture.Auth.Identity
{

    #region Context

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public ApplicationDbContext()
            : base(ApplicationConstants.ConnectionString)
        {
        }

        public DbSet<ApplicationRoleClaims> RoleClaims { get; set; }

        public DbSet<IdentityUserRole> UserRoles { get; set; }

        public DbSet<IdentityUserClaim> UserClaims { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>().ToTable("User", "dbo");
            modelBuilder.Entity<ApplicationUser>().ToTable("User", "dbo");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole", "dbo");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin", "dbo");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim", "dbo");
            modelBuilder.Entity<ApplicationRole>().ToTable("Role", "dbo");
            modelBuilder.Entity<ApplicationRoleClaims>().ToTable("RoleClaim", "dbo");
        }
    }

    #endregion

    #region Models

    public class ApplicationUser : IdentityUser 
    {
        [Column(TypeName = "tinyint")]
        public byte UserType { get; set; }

        [MaxLength(225)]
        public string FirstName { get; set; }

        [MaxLength(225)]
        public string LastName { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(128)]
        public string CreatedBy { get; set; }

        [Required]
        public DateTimeOffset CreatedDate { get; set; }

        [MaxLength(128)]
        public string ModifiedBy { get; set; }

        public DateTimeOffset ModifiedDate { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
    }

    public class ApplicationRoleClaims
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Roles")]
        public string RoleId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public virtual ApplicationRole Roles { get; set; }
    }

    #endregion

    #region Stores

    public class ApplicationUserStore : UserStore<ApplicationUser, ApplicationRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public ApplicationUserStore(ApplicationDbContext ctx)
            : base(ctx)
        {
        }
    }

    public class ApplicationRoleStore : RoleStore<ApplicationRole>
    {
        public ApplicationRoleStore(ApplicationDbContext ctx)
            : base(ctx)
        {
        }
    }

    #endregion 

    #region Managers

    public class ApplicationUserManager : UserManager<ApplicationUser, string>
    {
        public ApplicationUserManager(ApplicationUserStore store)
            : base(store)
        {
            PasswordHasher = new ApplicationPasswordHasher();
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(ApplicationRoleStore store)
            : base(store)
        {
        }
    }

    #endregion

}