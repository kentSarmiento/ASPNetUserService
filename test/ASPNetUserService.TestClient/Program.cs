using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OpenIddict.Abstractions;

namespace ASPNetUserService.TestClient
{
    public class Program
    {
        struct Tokens
        {
            public string AccessToken;
            public string RefreshToken;
        }

        enum Options
        {
            Default = 0,
            Register,
            Login,
            Display,
            Refresh,
            Test,
            Quit
        };

        private static readonly string AUTH_SERVICE = "https://localhost:5001";
        private static readonly string TASK_SERVICE = "https://localhost:6001";

        private static readonly string DEFAULT_LOGIN = "kentsarmiento@gmail.com";
        private static readonly string DEFAULT_PASSWORD = "P@ssw0rd1234";

        public static async Task Main(string[] args)
        {
            using var client = new HttpClient();
            Options option = Options.Default;

            Tokens? tokens = null;
            while (option != Options.Quit)
            {
                Console.WriteLine("Choose operation:");
                Console.WriteLine($"[{Options.Register:D}] Register account");
                Console.WriteLine($"[{Options.Login:D}] Login account");
                Console.WriteLine($"[{Options.Display:D}] Display tokens");
                Console.WriteLine($"[{Options.Refresh:D}] Refresh tokens");
                Console.WriteLine($"[{Options.Test:D}] Test access");
                Console.WriteLine($"[{Options.Quit:D}] Quit");
                Console.Write("> ");

                if (!Enum.TryParse<Options>(Console.ReadLine(), out option))
                {
                    continue;
                }

                switch (option)
                {
                    case Options.Register:
                        var registerAccount = PromptLoginPassword();
                        await CreateAccountAsync(client, registerAccount.email, registerAccount.password);
                        break;

                    case Options.Login:
                        var loginAccount = PromptLoginPassword();
                        tokens = await GetTokenAsync(client, loginAccount.email, loginAccount.password);
                        break;

                    case Options.Display:
                        Console.WriteLine();
                        Console.WriteLine("Access token: {0}", tokens?.AccessToken);
                        Console.WriteLine("Refresh token: {0}", tokens?.RefreshToken);
                        break;

                    case Options.Refresh:
                        tokens = await RefreshTokensAsync(client, tokens);
                        break;

                    case Options.Test:
                        if (tokens != null)
                        {
                            var resource = await GetResourceAsync(client, tokens);
                            Console.WriteLine();
                            Console.WriteLine("Internal API response: {0}", resource);

                            var tasklist = await GetTaskListAsync(client, tokens);
                            Console.WriteLine();
                            Console.WriteLine("External Service API response: {0}", tasklist);
                        }
                        break;

                    default:
                        break;
                }
                Console.WriteLine();
            }
        }

        private static (string email, string password) PromptLoginPassword()
        {
            Console.Write("Input login: ");
            var email = Console.ReadLine();
            if (string.IsNullOrEmpty(email))
            {
                email = DEFAULT_LOGIN;
                Console.WriteLine("User login: {0}", email);
            }

            Console.Write("Input password: ");
            var password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                password = DEFAULT_PASSWORD;
                Console.WriteLine("User password: {0}", password);
            }

            return (email, password);
        }


        private static async Task CreateAccountAsync(HttpClient client, string email, string password)
        {
            var response = await client.PostAsJsonAsync($"{AUTH_SERVICE}/api/account/register", new { email, password });

            // Ignore 409 responses, as they indicate that the account already exists.
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return;
            }

            response.EnsureSuccessStatusCode();
        }

        private static async Task<Tokens> GetTokenAsync(HttpClient client, string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{AUTH_SERVICE}/connect/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = email,
                ["password"] = password,
                ["scope"] = "offline_access"
            });

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            var payload = await response.Content.ReadFromJsonAsync<OpenIddictResponse>();

            if (!string.IsNullOrEmpty(payload.Error))
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            return new Tokens {AccessToken = payload.AccessToken, RefreshToken = payload.RefreshToken};
        }

        private static async Task<Tokens> RefreshTokensAsync(HttpClient client, Tokens? tokens)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{AUTH_SERVICE}/connect/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = tokens?.RefreshToken 
            });

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            var payload = await response.Content.ReadFromJsonAsync<OpenIddictResponse>();

            if (!string.IsNullOrEmpty(payload.Error))
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            return new Tokens {AccessToken = payload.AccessToken, RefreshToken = payload.RefreshToken};
        }

        private static async Task<string> GetResourceAsync(HttpClient client, Tokens? tokens)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{AUTH_SERVICE}/api/message");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens?.AccessToken);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<string> GetTaskListAsync(HttpClient client, Tokens? tokens)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{TASK_SERVICE}/api/todoitems");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens?.AccessToken);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
