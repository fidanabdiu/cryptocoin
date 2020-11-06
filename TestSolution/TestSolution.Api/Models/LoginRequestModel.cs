using Newtonsoft.Json;

namespace TestSolution.Api.Models
{
    public class LoginRequestModel
    {
        public LoginRequestModel()
        {
            this.Expiration = 1440;
        }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("OperatingSystem")]
        public string OperatingSystem { get; set; }

        [JsonProperty("Client")]
        public string Client { get; set; }

        [JsonProperty("Expiration")]
        public int Expiration { get; set; }
    }
}
