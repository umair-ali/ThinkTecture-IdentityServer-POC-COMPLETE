using Microsoft.AspNet.Identity;
using Constants = Thinktecture.IdentityServer.Core.Constants;

namespace ThinkTecture.Common.Identity
{
    public static class ApplicationConstants
    {
        //public const string UrlBaseAuth = "http://tt.auth.com";
        //public const string UrlBaseApi = "http://tt.api.com";
        //public const string UrlBaseWeb = "http://tt.web.com";
        //public const string ConnectionString = "IdentityConnection";

        public const string UrlBaseAuth = "http://localhost:11627";
        public const string UrlBaseApi = "http://localhost:13301";
        public const string UrlBaseWeb = "http://localhost:14104";
        public const string ConnectionString = "DefaultConnection";

        public const string AuthorizeEndpoint = UrlBaseAuth + "/connect/authorize";
        public const string LogoutEndpoint = UrlBaseAuth + "/connect/endsession";
        public const string TokenEndpoint = UrlBaseAuth + "/connect/token";
        public const string UserInfoEndpoint = UrlBaseAuth + "/connect/userinfo";
        public const string IdentityTokenValidationEndpoint = UrlBaseAuth + "/connect/identitytokenvalidation";
        public const string TokenRevocationEndpoint = UrlBaseAuth + "/connect/revocation";


        public const string ClientNameConsoleApp = "ClientName-ConsoleApp";
        public const string ClientIdConsoleApp = "ClientId-ConsoleApp";
        public const string ClientSecretConsoleApp = "ClientSecret-ConsoleApp";

        public const string ClientNameMvcApp = "ClientName-MvcApp";
        public const string ClientIdMvcApp = "ClientId-MvcApp";
        public const string ClientSecretMvcApp = "ClientSecret-MvcApp";

    }

    public static class ApplicationClaimTypes
    {
        public const string Role = Constants.ClaimTypes.Role;
        public const string DisplayName = "app/claim/displayname";
        public const string CallApi = "app/claim/callapi";
        public const string Values = "app/claim/values";
    }

    public static class ApplicationClaimValues
    {
        public const string Create = "create";
        public const string Read = "read";
        public const string Update = "update";
        public const string Delete = "delete";
    }

    public static class ApplicationScopes
    {
        public const string AppProfile = "app/scope/profile";
        public const string MvcApp = "app/scope/mvcapp";
        public const string ConsoleApp = "app/scope/consoleapp";
    }

    public static class ApplicationRoleTypes
    {
        public static string Admin = "app/role/admin";
        public static string Developer = "app/role/developer";
        public static string Agent = "app/user/type/agent";
        public static string Customer = "app/app/user/cutomer";
    }
}
