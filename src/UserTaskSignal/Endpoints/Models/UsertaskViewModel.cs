using Newtonsoft.Json;

namespace UserTask.AddOns.Endpoints.Models
{
    internal class UsertaskViewModel
    {
        [JsonProperty("taskDescription")]
        internal string TaskDescription { get;  set; }

        [JsonProperty("taskName")]
        internal string TaskName { get;  set; }

        [JsonProperty("taskTitle")]
        internal string TaskTitle { get;  set; }

        [JsonProperty("workflowInstanceId")]
        internal string WorkflowInstanceId { get;  set; }

        [JsonProperty("uiDefinition")]
        internal string UIDefinition { get;  set; }

        [JsonProperty("signal")]
        internal string Signal { get;  set; }

        [JsonProperty("engineId")]
        internal string EngineId { get;  set; }

        [JsonProperty("taskData")]
        internal string TaskData { get;  set; }
    }
}