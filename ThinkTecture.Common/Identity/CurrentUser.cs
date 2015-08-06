using System.Security.Claims;
using Thinktecture.IdentityServer.Core;

namespace ThinkTecture.Common.Identity
{
    public class AppUser : ClaimsPrincipal
    {
        public AppUser(ClaimsPrincipal principal)
            : base(principal)
        {
        }

        public bool IsAuthenticated
        {
            get { return Identity.IsAuthenticated; }
        }

        public string UserId
        {
            get
            {
                var claim = FindFirst("sub");
                return claim != null ? claim.Value : string.Empty;
            }
        }

        public string Name
        {
            get
            {
                var claim = FindFirst(ApplicationClaimTypes.DisplayName);
                if (claim != null)
                    return claim.Value;

                claim = FindFirst(Constants.ClaimTypes.PreferredUserName);
                if (claim != null)
                    return claim.Value;

                return "name-missing";
            }
        }

        public string AccessToken
        {
            get
            {
                var claim = FindFirst("access_token");
                return claim != null ? claim.Value : string.Empty;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return HasClaim(Constants.ClaimTypes.Role, ApplicationRoleTypes.Admin);
            }
        }

        public bool IsAgent
        {
            get
            {
                return HasClaim(Constants.ClaimTypes.Role, ApplicationRoleTypes.Agent);
            }
        }

        public bool IsCustomer
        {
            get
            {
                return HasClaim(Constants.ClaimTypes.Role, ApplicationRoleTypes.Customer);
            }
        }

        public bool IsDeveloper
        {
            get
            {
                return HasClaim(Constants.ClaimTypes.Role, ApplicationRoleTypes.Developer);
            }
        }
    }
}
