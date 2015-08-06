using System.Collections.Generic;
using ThinkTecture.Common.Identity;
using Thinktecture.IdentityServer.Core.Models;

namespace ThinkTecture.Auth.Configuration
{
    public class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                // Resource Credentials - console app
                new Client
                {
                    Enabled = true,
                    ClientName = ApplicationConstants.ClientNameConsoleApp,
                    ClientId = ApplicationConstants.ClientIdConsoleApp,
                    AccessTokenType = AccessTokenType.Reference, 

                    Flow = Flows.ResourceOwner,
                    ClientSecrets = new List<ClientSecret>
                    {
                        new ClientSecret(ApplicationConstants.ClientSecretConsoleApp.Sha256())
                    }
                },

                // Implicit - mvc app
                new Client
                {
                    Enabled = true,
                    ClientName = ApplicationConstants.ClientNameMvcApp,
                    ClientId = ApplicationConstants.ClientIdMvcApp,
                    ClientSecrets = new List<ClientSecret>
                    { 
                        new ClientSecret(ApplicationConstants.ClientSecretMvcApp.Sha256())
                    },
                    Flow = Flows.Implicit,                    
                    RequireConsent = false,
                    //AccessTokenType = AccessTokenType.Jwt,
                    IdentityTokenLifetime = 360,
                    AccessTokenLifetime = 3600,

                    RedirectUris = new List<string>
                    {
                        // MVC form post sample
                        ApplicationConstants.UrlBaseWeb
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        ApplicationConstants.UrlBaseWeb
                    }
                }
            };
        }
    }
}