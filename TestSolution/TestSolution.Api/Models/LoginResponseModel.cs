using Newtonsoft.Json;

namespace TestSolution.Api.Models
{
    public class LoginResponseModel
    {
        public LoginResponseModel() { }

        [JsonProperty("Success")]
        public bool Success { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Token")]
        public string Token { get; set; }
    }
}
