using Elsa.Models;
using Newtonsoft.Json;
using NodaTime;

namespace UserTask.AddOns.Endpoints.Models
{
    internal class WorkfowInstanceUsertaskViewModel
    {
        [JsonProperty("workflowInstanceId")]
        public string WorkflowInstanceId { get; set; } = string.Empty;
        
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
