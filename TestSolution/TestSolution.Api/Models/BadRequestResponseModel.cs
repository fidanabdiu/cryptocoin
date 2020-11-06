using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestSolution.Api.Models
{
    public class BadRequestResponseModel
    {
        public BadRequestResponseModel()
        {
            this.Message = "";
            this.Description = "";
            this.Details = new List<string>();
        }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Details")]
        public List<string> Details { get; set; }
    }
}
