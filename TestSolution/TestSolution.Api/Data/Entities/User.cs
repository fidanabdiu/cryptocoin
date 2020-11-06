using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestSolution.Api.Data.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            this.UserCryptoCoinCollection = new List<UserCryptoCoin>();
        }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Surname")]
        public string Surname { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonIgnore]
        public ICollection<UserCryptoCoin> UserCryptoCoinCollection { get; set; }
    }
}
