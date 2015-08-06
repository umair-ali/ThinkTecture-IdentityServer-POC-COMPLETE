using IdentityManager.Configuration;
using IdentityManager.Core.Logging;
using IdentityManager.Logging;
using Microsoft.Owin;
using Owin;
using ThinkTecture.Auth.Configuration;
using ThinkTecture.Auth.Identity;
using Thinktecture.IdentityServer.Core.Configuration;

[assembly: OwinStartup(typeof(ThinkTecture.Auth.Startup))]

namespace ThinkTecture.Auth
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configuring logging
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());

            // Configuring thinktecture identity manager
            app.Map("/admin", adminApp =>
            {
                var adminFactory = new IdentityManagerServiceFactory();
                adminFactory.ConfigureSimpleIdentityManagerService();

                adminApp.UseIdentityManager(new IdentityManagerOptions
                {
                    Factory = adminFactory
                });
            });

            // Configuring thinktecture auth server with Asp.Net identity
            var factory = Factory.Configure();
            factory.ConfigureCustomServices();
            var options = new IdentityServerOptions
            {
                SiteName = "My Auth Server",
                RequireSsl = false,
                SigningCertificate = Certificate.Load(),
                Factory = factory,
                CorsPolicy = CorsPolicy.AllowAll
            };
            app.UseIdentityServer(options);
        }
    }
}
