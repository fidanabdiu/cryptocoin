using Newtonsoft.Json;

namespace TestSolution.Api.Data.Entities
{
    public class BaseEntity
    {
        public BaseEntity() { }

        [JsonProperty("Id")]
        public int Id { get; set; }
    }
}
