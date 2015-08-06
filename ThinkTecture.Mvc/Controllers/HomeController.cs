﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using ThinkTecture.Common.Identity;
using ThinkTecture.Mvc.Identity;

namespace ThinkTecture.Mvc.Controllers
{
    public class HomeController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (CurrentUser.IsAuthenticated)
            {
                var idToken = CurrentUser.FindFirst("identity_token").Value;
                var accessToken = CurrentUser.FindFirst("access_token").Value;

                var claimsIdToken = GetClaimsFromToken(idToken, ApplicationConstants.ClientIdMvcApp);
                var claimsAccessToken = GetClaimsFromToken(accessToken, ApplicationConstants.UrlBaseAuth + "/resources");
            }

            return View();
        }

        [Authorize]
        public ActionResult SignIn()
        {
            return Redirect("/");
        }

        [AllowAnonymous]
        public ActionResult Signout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }

        [AllowAnonymous]
        public ActionResult Unauthorised()
        {
            return View();
        }

        [AuthorizeClaim(ApplicationClaimTypes.CallApi, ApplicationClaimValues.Read)]
        public ActionResult Api()
        {
            var token = Request.GetOwinContext().Authentication.User.FindFirst("access_token").Value;
            ViewBag.Message = CallApi(token);
            return View();
        }

        private static string CallApi(string token)
        {
            var client = new HttpClient();
            client.SetBearerToken(token);
            var result = client.GetStringAsync(ApplicationConstants.UrlBaseApi + "/api/test").Result;
            return result;
        }

        private IEnumerable<Claim> GetClaimsFromToken(string token, String audience)
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