using System.Web.Http;
using Microsoft.Owin;
using Owin;
using ThinkTecture.Common.Identity;
using Thinktecture.IdentityServer.AccessTokenValidation;

[assembly: OwinStartup(typeof(ThinkTecture.Api.Startup))]

namespace ThinkTecture.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // accept access tokens from identityserver
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = ApplicationConstants.UrlBaseAuth,
                ValidationMode = ValidationMode.ValidationEndpoint
            });

            // configure web api
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            // require authentication for all controllers
            config.Filters.Add(new AuthorizeAttribute());

            app.UseWebApi(config);
        }
    }
}
