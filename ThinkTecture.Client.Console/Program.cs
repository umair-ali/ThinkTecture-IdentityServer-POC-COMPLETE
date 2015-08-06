using System;
using System.Collections.Generic;
using System.Net.Http;
using ThinkTecture.Common.Identity;
using Thinktecture.IdentityModel.Client;

namespace ThinkTecture.Client.Console
{
    class Program
    {
        static void Main()
        {
            var response = GetUserToken();
            if (response == null) return;
            System.Console.WriteLine(response.AccessToken);
            CallApi(response);

            //TestPasswordHashing();
            
            System.Console.ReadLine();
        }

        private static void TestPasswordHashing()
        {
            var password = "asdfasdf";
            var items = new Dictionary<int, KeyValuePair<string, string>>();
            var hasher = new ApplicationPasswordHasher();
            
            for (var i = 0; i < 10; i++)
            {
                items.Add(i, new KeyValuePair<string, string>(password, hasher.HashPassword(password)));
            }

            foreach (var item in items)
            {
                System.Console.WriteLine(hasher.VerifyHashedPassword(item.Value.Value, password));
            }
        }

        static void CallApi(TokenResponse response)
        {
            var client = new HttpClient();
            client.SetBearerToken(response.AccessToken);
            System.Console.WriteLine(client.GetStringAsync(ApplicationConstants.UrlBaseApi + "/api/values").Result);
        }

        static TokenResponse GetUserToken()
        {
            var client = new OAuth2Client(
                new Uri(ApplicationConstants.TokenEndpoint),
                ApplicationConstants.ClientIdConsoleApp,
                ApplicationConstants.ClientSecretConsoleApp);

            return client.RequestResourceOwnerPasswordAsync("itua.synergy", "asdfasdf", ApplicationScopes.ConsoleApp).Result;
        }

    }
}
