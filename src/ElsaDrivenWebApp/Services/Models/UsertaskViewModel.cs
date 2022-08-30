using Newtonsoft.Json;

namespace ElsaDrivenWebApp.Services.Models
{
    public class UsertaskViewModel
    {
        [JsonProperty("taskDescription")]
        public string TaskDescription { get; set; } = string.Empty;

        [JsonProperty("taskName")]
        public string TaskName { get; set; } = string.Empty;

        [JsonProperty("taskTitle")]
        public string TaskTitle { get; set; } = string.Empty;

        [JsonProperty("taskData")]
        public string TaskData { get; set; } = string.Empty;

        [JsonProperty("workflowInstanceId")]
        public string WorkflowInstanceId { get; set; } = string.Empty;

        [JsonProperty("uIDefinition")]
        public string UIDefinition { get; set; } = string.Empty;

        [JsonProperty("signal")]
        public string Signal { get; set; } = string.Empty;

        [JsonProperty("engineId")]
        public string EngineId { get; set; } = string.Empty;
    }
}

