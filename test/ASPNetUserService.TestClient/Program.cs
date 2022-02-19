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
        public static async Task Main(string[] args)
        {
            using var client = new HttpClient();

            const string registeredEmail = "kentsarmiento@gmail.com", registeredPassword = "P@ssw0rd1234";
            await CreateAccountAsync(client, registeredEmail, registeredPassword);

            Console.Write("Input login: ");
            var loginEmail = Console.ReadLine();
            if (string.IsNullOrEmpty(loginEmail))
            {
                loginEmail = registeredEmail;
                Console.WriteLine("User login: {0}", loginEmail);
            }

            Console.Write("Input password: ");
            var loginPassword = Console.ReadLine();
            if (string.IsNullOrEmpty(loginPassword))
            {
                loginPassword = registeredPassword;
                Console.WriteLine("User password: {0}", loginPassword);
            }

            var tokens = await GetTokenAsync(client, loginEmail, loginPassword);
            Console.WriteLine();
            Console.WriteLine("Initial access token: {0}", tokens.AccessToken);
            Console.WriteLine("Initial refresh token: {0}", tokens.RefreshToken);
            Console.WriteLine();

            var resource = await GetResourceAsync(client, tokens.AccessToken);
            Console.WriteLine("Internal API response: {0}", resource);
            Console.WriteLine();

            var tasklist = await GetTaskListAsync(client, tokens.AccessToken);
            Console.WriteLine("External Service API response: {0}", tasklist);
            Console.WriteLine();

            // Test multiple use of access and refresh token
            for (int i=0; i<100; i++)
            {
                tokens = await RefreshTokensAsync(client, tokens.RefreshToken);

                resource = await GetResourceAsync(client, tokens.AccessToken);
                tasklist = await GetTaskListAsync(client, tokens.AccessToken);

                System.Threading.Thread.Sleep(500);
            }

            tokens = await RefreshTokensAsync(client, tokens.RefreshToken);
            Console.WriteLine();
            Console.WriteLine("New access token: {0}", tokens.AccessToken);
            Console.WriteLine("New refresh token: {0}", tokens.RefreshToken);
            Console.WriteLine();

            resource = await GetResourceAsync(client, tokens.AccessToken);
            Console.WriteLine("Internal API response: {0}", resource);
            Console.WriteLine();

            tasklist = await GetTaskListAsync(client, tokens.AccessToken);
            Console.WriteLine("External Service API response: {0}", tasklist);
            Console.WriteLine();
        }

        public static async Task CreateAccountAsync(HttpClient client, string email, string password)
        {
            var response = await client.PostAsJsonAsync("https://localhost:5001/api/account/register", new { email, password });

            // Ignore 409 responses, as they indicate that the account already exists.
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return;
            }

            response.EnsureSuccessStatusCode();
        }

        public static async Task<(string AccessToken, string RefreshToken)> GetTokenAsync(HttpClient client, string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5001/connect/token");
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

            return (payload.AccessToken, payload.RefreshToken);
        }

        public static async Task<(string AccessToken, string RefreshToken)> RefreshTokensAsync(HttpClient client, string refreshToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5001/connect/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            });

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            var payload = await response.Content.ReadFromJsonAsync<OpenIddictResponse>();

            if (!string.IsNullOrEmpty(payload.Error))
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            return (payload.AccessToken, payload.RefreshToken);
        }

        public static async Task<string> GetResourceAsync(HttpClient client, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/api/message");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetTaskListAsync(HttpClient client, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:6001/api/todoitems");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
