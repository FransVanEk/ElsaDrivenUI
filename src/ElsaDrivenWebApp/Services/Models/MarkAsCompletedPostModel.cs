using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ElsaDrivenWebApp.Services.Models
{
    public class MarkAsCompletedPostModel
    {
        [JsonProperty("workflowInstanceId")]
        public string WorkflowInstanceId { get; set; } = string.Empty;

        [JsonProperty("input")]
        public JToken Input { get; set; } = JValue.CreateNull();
    }
}
