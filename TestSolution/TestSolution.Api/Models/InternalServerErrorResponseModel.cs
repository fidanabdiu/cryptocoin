using Newtonsoft.Json;

namespace TestSolution.Api.Models
{
    public class InternalServerErrorResponseModel
    {
        public InternalServerErrorResponseModel()
        {
            this.LogId = "";
            this.Message = "";
            this.StackTrace = "";
        }

        [JsonProperty("LogId")]
        public string LogId { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("StackTrace")]
        public string StackTrace { get; set; }
    }
}
