using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestSolution.Api.Data.Entities
{
    public class CryptoCoin : BaseEntity
    {
        public CryptoCoin()
        {
            this.UserCryptoCoinCollection = new List<UserCryptoCoin>();
        }

        [JsonProperty("Code")]
        public string Code { get; set; }

        [JsonProperty("Rank")]
        public string Rank { get; set; }

        [JsonProperty("Symbol")]
        public string Symbol { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Supply")]
        public string Supply { get; set; }

        [JsonProperty("MaxSupply")]
        public string MaxSupply { get; set; }

        [JsonIgnore]
        public ICollection<UserCryptoCoin> UserCryptoCoinCollection { get; set; }
    }
}
