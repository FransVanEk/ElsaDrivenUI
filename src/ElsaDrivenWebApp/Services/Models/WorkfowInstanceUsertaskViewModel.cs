using Newtonsoft.Json;

namespace ElsaDrivenWebApp.Services.Models
{
    public class WorkfowInstanceUsertaskViewModel
    {
        [JsonProperty("workflowInstanceId")]
        public string WorkflowInstanceId { get; set; } = string.Empty;

        [JsonProperty("lastExecuted")]
        public string? LastExecuted { get; set; }

        [JsonProperty("state")]
        public string State { get; set; } = string.Empty;

        [JsonProperty("definitionId")]
        public string DefinitionId { get; set; } = string.Empty;

        [JsonProperty("workflowName")]
        public string WorkflowName { get; set; } = string.Empty;

        [JsonProperty("userTasks")]
        public List<UsertaskViewModel> UserTasks { get; set; } = new List<UsertaskViewModel>();
    }
}
