using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AuthorizationContext = System.Web.Mvc.AuthorizationContext;

namespace ThinkTecture.Mvc.Identity
{
    public class AuthorizeClaimAttribute : AuthorizeAttribute
    {
        private string ClaimType { get; set; }
        private string ClaimValue { get; set; }

        public AuthorizeClaimAttribute(string type, string value)
        {
            ClaimType = type;
            ClaimValue = value;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
                return false;

            // Input validation
            if (string.IsNullOrEmpty(ClaimType))
                throw new ArgumentNullException(ClaimType);
            if (string.IsNullOrEmpty(ClaimValue))
                throw new ArgumentNullException(ClaimValue);

            var user = HttpContext.Current.User as ClaimsPrincipal;

            return user != null && user.HasClaim(ClaimType, ClaimValue);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(
                            new
                            {
                                controller = "Home",
                                action = "Unauthorised"
                            })
                        );
        }
    }
}