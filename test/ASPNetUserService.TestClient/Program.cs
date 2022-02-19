﻿using System;
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

            var token = await GetTokenAsync(client, loginEmail, loginPassword);
            Console.WriteLine("Access token: {0}", token);
            Console.WriteLine();

            var resource = await GetResourceAsync(client, token);
            Console.WriteLine("Internal API response: {0}", resource);
            Console.WriteLine();

            var tasklist = await GetTaskListAsync(client, token);
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

        public static async Task<string> GetTokenAsync(HttpClient client, string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5001/connect/token");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = email,
                ["password"] = password
            });

            var response = await client.SendAsync(request, HttpCompletionOption.ResponseContentRead);

            var payload = await response.Content.ReadFromJsonAsync<OpenIddictResponse>();

            if (!string.IsNullOrEmpty(payload.Error))
            {
                throw new InvalidOperationException("An error occurred while retrieving an access token.");
            }

            return payload.AccessToken;
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
