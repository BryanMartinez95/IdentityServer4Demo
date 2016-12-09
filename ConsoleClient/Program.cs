using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
           Console.WriteLine("========================Request Resource Owner Password===============================");
            var disco = await DiscoveryClient.GetAsync("http://localhost:5001");

            var tokenClientTest = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");

            var tokenResponseTest = await tokenClientTest.RequestResourceOwnerPasswordAsync("Bryan", "password", "ConsoleApi");

            if (tokenResponseTest.IsError)
            {
                Console.WriteLine(tokenResponseTest.Error);
                return;
            }

            Console.WriteLine(tokenResponseTest.Json);
            Console.WriteLine("\n\n");
            Console.WriteLine("=======================Using the Web API================================");

            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("ConsoleApi");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            Console.WriteLine(tokenResponse.Json);

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5000/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
            Console.ReadLine();
        }
    }
}
