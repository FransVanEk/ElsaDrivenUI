using Elsa.Models;
using Newtonsoft.Json;

namespace UserTask.AddOns.Endpoints.Models
{
    internal class WorkfowInstanceUsertaskViewModel
    {
        [JsonProperty("lastExecuted")]
        internal string? LastExecuted { get;  set; }
        [JsonProperty("state")] 
        internal string State { get; set; } = string.Empty;
        [JsonProperty("definitionId")] 
        internal string DefinitionId { get;  set; } = string.Empty;
        [JsonProperty("workflowName")] 
        internal string WorkflowName { get; set; } = string.Empty;
        [JsonProperty("userTasks")] 
        internal List<UsertaskViewModel> UserTasks { get; set; } = new List<UsertaskViewModel>();
    }
}
