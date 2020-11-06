using Newtonsoft.Json;

namespace TestSolution.Api.Models
{
    public class RegisterModel
    {
        public RegisterModel() { }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }
    }
}
