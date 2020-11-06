using Newtonsoft.Json;

namespace TestSolution.Api.Data.Entities
{
    public class UserCryptoCoin : BaseEntity
    {
        public UserCryptoCoin() { }

        [JsonProperty("UserId")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User UserObject { get; set; }

        [JsonProperty("CryptoCoinId")]
        public int CryptoCoinId { get; set; }

        [JsonIgnore]
        public CryptoCoin CryptoCoinObject { get; set; }
    }
}
