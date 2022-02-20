using Newtonsoft.Json;

namespace ASPNetTodoService.Specs
{
    public class TokenInfo
    {
        [JsonProperty(PropertyName = "access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string? RefreshToken { get; set; }
    }
}
