using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ThinkTecture.Api.Identity
{
    public class AuthorizeClaimAttribute : AuthorizationFilterAttribute
    {
        private string ClaimType { get; set; }
        private string ClaimValue { get; set; }

        public AuthorizeClaimAttribute(string type, string value)
        {
            ClaimType = type;
            ClaimValue = value;
        }

        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            // Input validation
            if (string.IsNullOrEmpty(ClaimType))
                throw new ArgumentNullException(ClaimType);
            if (string.IsNullOrEmpty(ClaimValue))
                throw new ArgumentNullException(ClaimValue);

            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            // Verify authentication 
            if (principal == null || !principal.Identity.IsAuthenticated)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return Task.FromResult<object>(null);
            }

            // Verify authorization
            if (!(principal.HasClaim(x => x.Type == ClaimType && x.Value == ClaimValue)))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return Task.FromResult<object>(null);
            }

            //User is Authorized, complete execution
            return Task.FromResult<object>(null);
        }
    }
}