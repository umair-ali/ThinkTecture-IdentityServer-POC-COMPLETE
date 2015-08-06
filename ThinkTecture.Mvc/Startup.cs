using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using ThinkTecture.Common.Identity;
using Thinktecture.IdentityModel;

[assembly: OwinStartup(typeof(ThinkTecture.Mvc.Startup))]

namespace ThinkTecture.Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Implicit mvc owin
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = ApplicationConstants.ClientIdMvcApp,
                Authority = ApplicationConstants.UrlBaseAuth,
                RedirectUri = ApplicationConstants.UrlBaseWeb,
                PostLogoutRedirectUri = ApplicationConstants.UrlBaseWeb,
                ResponseType = "id_token token",
                Scope = string.Format("openid {0} {1}", ApplicationScopes.AppProfile, ApplicationScopes.MvcApp),
                SignInAsAuthenticationType = "Cookies",

                // sample how to access token on form (when adding the token response type)
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        #region code for getting claims from userinfo endpoint
                        //// filter "protocol" claims
                        //var claims = new List<Claim>(from c in n.AuthenticationTicket.Identity.Claims
                        //                             where c.Type != "iss" &&
                        //                                 c.Type != "aud" &&
                        //                                 c.Type != "nbf" &&
                        //                                 c.Type != "exp" &&
                        //                                 c.Type != "iat" &&
                        //                                 c.Type != "nonce" &&
                        //                                 c.Type != "c_hash" &&
                        //                                 c.Type != "at_hash"
                        //                             select c);

                        //// get userinfo data
                        //var userInfoClient = new UserInfoClient(
                        //    new Uri(ApplicationConstants.UserInfoEndpoint),
                        //    n.ProtocolMessage.AccessToken);

                        //var userInfo = await userInfoClient.GetAsync();
                        //userInfo.Claims.ToList().ForEach(ui => claims.Add(new Claim(ui.Item1, ui.Item2)));

                        //// Adding access token in claims
                        //var accessToken = n.ProtocolMessage.AccessToken;
                        //if (!string.IsNullOrEmpty(accessToken))
                        //{
                        //    claims.Add(new Claim("access_token", accessToken));
                        //}

                        //// Adding identity token in claims
                        //var identityToken = n.ProtocolMessage.IdToken;
                        //if (!string.IsNullOrEmpty(identityToken))
                        //{
                        //    claims.Add(new Claim("identity_token", identityToken));
                        //}

                        //n.AuthenticationTicket = new AuthenticationTicket(new ClaimsIdentity(
                        //    claims.Distinct(new ClaimComparer()), n.AuthenticationTicket.Identity.AuthenticationType), 
                        //    n.AuthenticationTicket.Properties);
                        #endregion

                        #region code for getting claims from access token
                        // filter "protocol" claims
                        var claims = n.AuthenticationTicket.Identity.Claims.ToList();

                        // Getting claims from access token
                        var accessClaims =
                                GetClaimsFromToken(n.ProtocolMessage.AccessToken,
                                    ApplicationConstants.UrlBaseAuth + "/resources");
                        // Adding access claims to identity token
                        accessClaims.ToList().ForEach(ac => claims.Add(new Claim(ac.Type, ac.Value)));

                        // Adding access token in claims
                        var accessToken = n.ProtocolMessage.AccessToken;
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            claims.Add(new Claim("access_token", accessToken));
                        }

                        // Adding identity token in claims
                        var identityToken = n.ProtocolMessage.IdToken;
                        if (!string.IsNullOrEmpty(identityToken))
                        {
                            claims.Add(new Claim("identity_token", identityToken));
                        }

                        // Removing unnecessory claims
                        claims = new List<Claim>(from c in claims
                                                 where c.Type != "iss" &&
                                                     c.Type != "aud" &&
                                                     c.Type != "nbf" &&
                                                     c.Type != "exp" &&
                                                     c.Type != "iat" &&
                                                     c.Type != "nonce" &&
                                                     c.Type != "c_hash" &&
                                                     c.Type != "at_hash"
                                                 select c);

                        n.AuthenticationTicket = new AuthenticationTicket(new ClaimsIdentity(
                            claims.Distinct(new ClaimComparer()), n.AuthenticationTicket.Identity.AuthenticationType),
                            n.AuthenticationTicket.Properties);
                        #endregion

                        #region code for normal logins
                        //// Adding access token in claims
                        //var accessToken = n.ProtocolMessage.AccessToken;
                        //if (!string.IsNullOrEmpty(accessToken))
                        //{
                        //    n.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", accessToken));
                        //}

                        //// Adding identity token in claims
                        //var identityToken = n.ProtocolMessage.IdToken;
                        //if (!string.IsNullOrEmpty(identityToken))
                        //{
                        //    n.AuthenticationTicket.Identity.AddClaim(new Claim("identity_token", identityToken));
                        //}
                        #endregion
                    },

                    RedirectToIdentityProvider = async n =>
                    {
                        // if signing out, add the id_token_hint
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idToken = n.OwinContext.Authentication.User.FindFirst("identity_token");
                            n.ProtocolMessage.IdTokenHint = idToken == null ? null : idToken.Value;
                            n.ProtocolMessage.PostLogoutRedirectUri = ApplicationConstants.UrlBaseWeb;
                        }
                    }
                }
            });
        }

        private static IEnumerable<Claim> GetClaimsFromToken(string token, String audience)
        {
            const string certString = "MIIDBTCCAfGgAwIBAgIQNQb+T2ncIrNA6cKvUA1GWTAJBgUrDgMCHQUAMBIxEDAOBgNVBAMTB0RldlJvb3QwHhcNMTAwMTIwMjIwMDAwWhcNMjAwMTIwMjIwMDAwWjAVMRMwEQYDVQQDEwppZHNydjN0ZXN0MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqnTksBdxOiOlsmRNd+mMS2M3o1IDpK4uAr0T4/YqO3zYHAGAWTwsq4ms+NWynqY5HaB4EThNxuq2GWC5JKpO1YirOrwS97B5x9LJyHXPsdJcSikEI9BxOkl6WLQ0UzPxHdYTLpR4/O+0ILAlXw8NU4+jB4AP8Sn9YGYJ5w0fLw5YmWioXeWvocz1wHrZdJPxS8XnqHXwMUozVzQj+x6daOv5FmrHU1r9/bbp0a1GLv4BbTtSh4kMyz1hXylho0EvPg5p9YIKStbNAW9eNWvv5R8HN7PPei21AsUqxekK0oW9jnEdHewckToX7x5zULWKwwZIksll0XnVczVgy7fCFwIDAQABo1wwWjATBgNVHSUEDDAKBggrBgEFBQcDATBDBgNVHQEEPDA6gBDSFgDaV+Q2d2191r6A38tBoRQwEjEQMA4GA1UEAxMHRGV2Um9vdIIQLFk7exPNg41NRNaeNu0I9jAJBgUrDgMCHQUAA4IBAQBUnMSZxY5xosMEW6Mz4WEAjNoNv2QvqNmk23RMZGMgr516ROeWS5D3RlTNyU8FkstNCC4maDM3E0Bi4bbzW3AwrpbluqtcyMN3Pivqdxx+zKWKiORJqqLIvN8CT1fVPxxXb/e9GOdaR8eXSmB0PgNUhM4IjgNkwBbvWC9F/lzvwjlQgciR7d4GfXPYsE1vf8tmdQaY8/PtdAkExmbrb9MihdggSoGXlELrPA91Yce+fiRcKY3rQlNWVd4DOoJ/cPXsXwry8pWjNCo5JD8Q+RQ5yZEy7YPoifwemLhTdsBz3hlZr28oCGJ3kbnpW0xGvQb3VHSTVVbeei0CfXoW6iz1";
            var cert = new X509Certificate2(Convert.FromBase64String(certString));

            var parameters = new TokenValidationParameters
            {
                ValidAudience = audience,
                ValidIssuer = ApplicationConstants.UrlBaseAuth,
                IssuerSigningToken = new X509SecurityToken(cert)
            };

            var handler = new JwtSecurityTokenHandler();
            SecurityToken jwt;
            var id = handler.ValidateToken(token, parameters, out jwt);

            return id.Claims;
        }
    }
}
