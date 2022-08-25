using Newtonsoft.Json;

namespace UserTask.AddOns.Endpoints.Models
{
    internal class UsertaskViewModel
    {
        [JsonProperty("taskDescription")]
        public string TaskDescription { get; internal set; }

        [JsonProperty("taskName")]
        public string TaskName { get; internal set; }

        [JsonProperty("taskTitle")]
        public string TaskTitle { get; internal set; }

        [JsonProperty("workflowInstanceId")]
        public string WorkflowInstanceId { get; internal set; }

        [JsonProperty("uiDefinition")]
        public string UIDefinition { get; internal set; }

        [JsonProperty("signal")]
        public string Signal { get; internal set; }

        [JsonProperty("engineId")]
        public string EngineId { get; internal set; }

        [JsonProperty("taskData")]
        public string TaskData { get; internal set; }
    }
}