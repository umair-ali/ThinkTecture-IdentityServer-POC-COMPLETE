using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ThinkTecture.Auth.Identity;
using ThinkTecture.Common.Identity;
using Thinktecture.IdentityModel;
using Thinktecture.IdentityServer.AspNetIdentity;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Thinktecture.IdentityServer.Core.Services.Default;
using Thinktecture.IdentityServer.Core.Validation;

namespace ThinkTecture.Auth.Configuration
{
    #region Extenstions

    public static class CustomServiceExtensions
    {
        public static void ConfigureCustomServices(this IdentityServerServiceFactory factory)
        {
            factory.ConfigureDefaultViewService(CustomViewService.Get());
            factory.Register(new Registration<IClaimsProvider, ApplicationClaimsProvider>());
            factory.UserService = new Registration<IUserService, ApplicationUserService>();
            factory.Register(new Registration<ApplicationUserManager>());
            factory.Register(new Registration<ApplicationUserStore>());
            factory.Register(new Registration<ApplicationDbContext>(resolver => new ApplicationDbContext()));
        }
    }

    #endregion

    #region User Service
    
    public class ApplicationUserService : AspNetIdentityUserService<ApplicationUser, string>
    {
        public ApplicationUserService(ApplicationUserManager userMgr)
            : base(userMgr)
        {
        }

        #region code for adding custom claims in profile data
        //private readonly ApplicationDbContext _context;

        //public ApplicationUserService(ApplicationUserManager userMgr)
        //    : base(userMgr)
        //{
        //    _context = new ApplicationDbContext();
        //}

        //public override async Task<IEnumerable<Claim>> GetProfileDataAsync(ClaimsPrincipal subject, IEnumerable<string> requestedClaimTypes = null)
        //{
        //    var claims = await base.GetProfileDataAsync(subject, requestedClaimTypes);
        //    var customClaims = GetCustomClaims(subject);
        //    return claims.Concat(customClaims).Distinct(new ClaimComparer());
        //}

        ///// <summary>
        ///// Gets all customer claims related to user from Role, UserClaim and RoleClaim tables.
        ///// </summary>
        ///// <param name="subject">Principal of current user</param>
        ///// <returns>List of claims given user</returns>
        //private IEnumerable<Claim> GetCustomClaims(ClaimsPrincipal subject)
        //{
        //    var claims = new List<Claim>();

        //    // Getting userid of current user
        //    var currentUserId = subject.FindFirst("sub").Value;

        //    // Finding claims based on assigned Roles
        //    var roleClaims = from rc in _context.RoleClaims
        //                     join r in _context.Roles on rc.RoleId equals r.Id
        //                     join ur in _context.UserRoles on r.Id equals ur.RoleId
        //                     //join u in _context.Users on ur.UserId equals u.Id
        //                     where ur.UserId == currentUserId
        //                     select new { Type = rc.ClaimType, Value = rc.ClaimValue };

        //    // Finding claims assigned on user
        //    var userClaims = _context.UserClaims.Where(uc => uc.UserId == currentUserId)
        //        .Select(rc => new { Type = rc.ClaimType, Value = rc.ClaimValue });

        //    // Adding roles in claims
        //    var roles = from r in _context.Roles
        //                join ur in _context.UserRoles on r.Id equals ur.RoleId
        //                where ur.UserId == currentUserId
        //                select new { Type = Constants.ClaimTypes.Role, Value = r.Name };

        //    // Adding all to claims
        //    roleClaims.Concat(userClaims).Concat(roles)
        //        .Where(c => c != null && !string.IsNullOrEmpty(c.Type) && !string.IsNullOrEmpty(c.Value))
        //        .ToList().ForEach(c => claims.Add(new Claim(c.Type, c.Value)));


        //    // Adding profile claims
        //    var profile = _context.Users.FirstOrDefault(u => u.Id == currentUserId);
        //    if (profile == null) return claims;
        //    // Adding user type as role
        //    if (profile.UserType.Equals(1))
        //        claims.Add(new Claim(Constants.ClaimTypes.Role, ApplicationRoleTypes.Agent));
        //    else if (profile.UserType.Equals(2))
        //        claims.Add(new Claim(Constants.ClaimTypes.Role, ApplicationRoleTypes.Customer));
        //    // Adding display name
        //    if (!string.IsNullOrEmpty(profile.FirstName))
        //        claims.Add(new Claim(ApplicationClaimTypes.DisplayName, profile.FirstName));

        //    return claims;
        //}
        #endregion
    }

    #endregion

    #region Claims provider

    public class ApplicationClaimsProvider : DefaultClaimsProvider
    {
        private readonly ApplicationDbContext _context;

        public ApplicationClaimsProvider(IUserService users)
            : base(users)
        {
            _context = new ApplicationDbContext();
        }

        public override async Task<IEnumerable<Claim>> GetAccessTokenClaimsAsync(ClaimsPrincipal subject, Client client,
            IEnumerable<Scope> scopes, ValidatedRequest request)
        {
            var claims = await base.GetAccessTokenClaimsAsync(subject, client, scopes, request);
            var customClaims = GetCustomClaims(subject, scopes, ScopeType.Resource);
            return claims.Concat(customClaims).Distinct(new ClaimComparer());
        }

        public override async Task<IEnumerable<Claim>> GetIdentityTokenClaimsAsync(ClaimsPrincipal subject, Client client,
            IEnumerable<Scope> scopes, bool includeAllIdentityClaims, ValidatedRequest request)
        {
            var claims = await base.GetIdentityTokenClaimsAsync(subject, client, scopes, includeAllIdentityClaims, request);
            var customClaims = GetCustomClaims(subject, scopes, ScopeType.Identity);
            return claims.Concat(customClaims).Distinct(new ClaimComparer());
        }

        /// <summary>
        /// Gets all customer claims related to user from Role, UserClaim and RoleClaim tables.
        /// </summary>
        /// <param name="subject">Principal of current user</param>
        /// <param name="scopes">Scopes demanded by client</param>
        /// <param name="scopeType">Type of scope either Identity or Resource</param>
        /// <param name="applyScopeFilteration">Filter claims based on demanded scopes</param>
        /// <returns>List of claims given user</returns>
        private IEnumerable<Claim> GetCustomClaims(ClaimsPrincipal subject, IEnumerable<Scope> scopes, ScopeType scopeType, bool applyScopeFilteration = true)
        {
            var claims = new List<Claim>();

            // Getting userid of current user
            var currentUserId = subject.FindFirst("sub").Value;

            // No user found
            if (string.IsNullOrEmpty(currentUserId))
                return claims;

            // Finding claims based on assigned Roles
            var roleClaims = from rc in _context.RoleClaims
                             join r in _context.Roles on rc.RoleId equals r.Id
                             join ur in _context.UserRoles on r.Id equals ur.RoleId
                             //join u in _context.Users on ur.UserId equals u.Id
                             where ur.UserId == currentUserId
                             select new { Type = rc.ClaimType, Value = rc.ClaimValue };

            // Finding claims assigned on user
            var userClaims = _context.UserClaims.Where(uc => uc.UserId == currentUserId)
                .Select(rc => new { Type = rc.ClaimType, Value = rc.ClaimValue });
            
            // Adding roles in claims
            var roles = from r in _context.Roles
                        join ur in _context.UserRoles on r.Id equals ur.RoleId
                        where ur.UserId == currentUserId
                        select new { Type = Constants.ClaimTypes.Role, Value = r.Name };
            
            // Adding all to claims
            roleClaims.Concat(userClaims).Concat(roles)
                .Where(c => c != null && !string.IsNullOrEmpty(c.Type) && !string.IsNullOrEmpty(c.Value))
                .ToList().ForEach(c => claims.Add(new Claim(c.Type, c.Value)));

            // Adding profile claims
            var profile = _context.Users.FirstOrDefault(u => u.Id == currentUserId);
            if (profile == null) return claims;
            // Adding user type as role
            if (profile.UserType.Equals(1))
                claims.Add(new Claim(ApplicationClaimTypes.Role, ApplicationRoleTypes.Agent));
            else if (profile.UserType.Equals(2))
                claims.Add(new Claim(ApplicationClaimTypes.Role, ApplicationRoleTypes.Customer));
            // Adding display name
            if (!string.IsNullOrEmpty(profile.FirstName))
                claims.Add(new Claim(ApplicationClaimTypes.DisplayName, profile.FirstName));

            if (!applyScopeFilteration) return claims;

            // Filter claims based on scope
            var scopeClaims = scopes.Where(s => s.Enabled && s.Type == scopeType).SelectMany(s => s.Claims.Select(c => c.Name)).Distinct();
            return claims.Distinct(new ClaimComparer()).Where(c => scopeClaims.Contains(c.Type)).Select(c => c);
        }
    }

    #endregion

    #region View Service

    public static class CustomViewService
    {

        public static DefaultViewServiceOptions Get()
        {
            var viewOptions = new DefaultViewServiceOptions();
            viewOptions.Stylesheets.Add("/Content/Site.css");
            viewOptions.CacheViews = false;
            return viewOptions;
        }
    }

    #endregion
}
