using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace ASPNetUserService.Specs.Drivers
{
    public class UserServiceRequestDriver
    {
        private const string BaseUrl = "https://localhost:5001/";

        private readonly RestClient _client;

        public UserServiceRequestDriver()
        {
            _client = new RestClient(BaseUrl);
        }

        public async Task<RestResponse> RegisterUser(UserInfo user)
        {
            var request = new RestRequest("api/account/register").AddJsonBody(user);
            return await _client.PostAsync(request);
        }

        public async Task<TokenInfo?> LoginUser(UserInfo user)
        {
            var request = new RestRequest("connect/token");

            request.AddParameter("grant_type", "password", ParameterType.GetOrPost);
            request.AddParameter("username", user.Email ?? string.Empty, ParameterType.GetOrPost);
            request.AddParameter("password", user.Password ?? string.Empty, ParameterType.GetOrPost);
            request.AddParameter("scope", "offline_access", ParameterType.GetOrPost);

            var response = await _client.PostAsync(request);
            var tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(response.Content);

            return tokenInfo;
        }

        public async Task<string?> GetMessage(TokenInfo? tokenInfo)
        {
            var request = new RestRequest("api/message");

            request.AddHeader("Authorization", $"Bearer {tokenInfo?.AccessToken}");

            var response = await _client.GetAsync(request);
            return response?.Content?.ToString();
        }
    }
}
