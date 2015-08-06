using System.Collections.Generic;
using ThinkTecture.Common.Identity;
using Thinktecture.IdentityServer.Core.Models;

namespace ThinkTecture.Auth.Configuration
{
    public class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>();

            scopes.AddRange(StandardScopes.All);

            var profileScope = new Scope
            {
                Name = ApplicationScopes.AppProfile,
                DisplayName = "User ProfileScope",
                Description = "Scope for Application specific user details.",
                Type = ScopeType.Identity,
                IncludeAllClaimsForUser = true,
                ShowInDiscoveryDocument = true,
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim(ApplicationClaimTypes.DisplayName, alwaysInclude: true)
                }
            };
            scopes.Add(profileScope);

            var mvcScope = new Scope
            {
                Name = ApplicationScopes.MvcApp,
                DisplayName = "Mvc App Scope",
                Description = "Scope for Mvc Application.",
                Type = ScopeType.Resource,
                IncludeAllClaimsForUser = true,
                ShowInDiscoveryDocument = true,
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim(ApplicationClaimTypes.Role, alwaysInclude: true),
                    new ScopeClaim(ApplicationClaimTypes.CallApi, alwaysInclude: true)
                }
            };
            scopes.Add(mvcScope);

            var consoleScope = new Scope
            {
                Name = ApplicationScopes.ConsoleApp,
                DisplayName = "Console App Scope",
                Description = "Scope for Console Application.",
                Type = ScopeType.Resource,
                IncludeAllClaimsForUser = true,
                ShowInDiscoveryDocument = true,
                Claims = new List<ScopeClaim>
                {
                    new ScopeClaim(ApplicationClaimTypes.Role, alwaysInclude: true),
                    new ScopeClaim(ApplicationClaimTypes.Values, alwaysInclude: true)
                }
            };
            scopes.Add(consoleScope);

            return scopes;
        }
    }
}