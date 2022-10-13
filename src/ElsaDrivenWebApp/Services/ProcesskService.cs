
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ElsaDrivenWebApp.Services
{
    public class ProcessService
    {
        private readonly HttpClient httpClient;

        public ProcessService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<StartedWorkflowsResponse> SendSignal(string signal, object data)
        {
           return await PostObjectJson(data, $"v1/signals/{signal}/execute");
        }

        public async Task<StartedWorkflowsResponse> SendSignal(string signal)
        {
            return await PostObjectJson(new JObject(), $"v1/signals/{signal}/execute");
        }

        public async Task<StartedWorkflowsResponse> SendSignalAsync(string signal, object data)
        {
            return await PostObjectJson(data, $"v1/signals/{signal}/dispatch");
        }

        public async Task<StartedWorkflowsResponse> SendSignalAsync(string signal)
        {
            return await PostObjectJson(new JObject(), $"v1/signals/{signal}/dispatch");
        }

        private async Task<StartedWorkflowsResponse> PostObjectJson(object data, string url)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var result = await httpClient.PostAsync(url, content);
            return await result?.Content?.ReadFromJsonAsync<StartedWorkflowsResponse>();
        }
    }

    public class StartedWorkflowsResponse
    {
        public List<WorkflowInstanceDetails> StartedWorkflows { get; set; } = new List<WorkflowInstanceDetails>();
    }

    public class WorkflowInstanceDetails
    {
        public string WorkflowInstanceId { get; set; } = string.Empty;
    }
}
