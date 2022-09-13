using ElsaDrivenWebApp.Services.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ElsaDrivenWebApp.Services
{
    public class UsertaskService
    {
        private readonly HttpClient httpClient;
        public Dictionary<string, UsertaskViewModel> workflowInstancesCache = new Dictionary<string, UsertaskViewModel>();

        public UsertaskService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<UsertaskViewModel[]> GetWorkflowsForSignal(string signal)
        {
            return await httpClient.GetFromJsonAsync<UsertaskViewModel[]>($"/v1/usertask-signals/{signal}");
        }

        public async Task<UsertaskViewModel[]> GetWorkflowsWaitingOnUserTask()
        {
            return await httpClient.GetFromJsonAsync<UsertaskViewModel[]>($"/v1/usertask-signals/workflowinstances");
        }

        public async Task<UsertaskViewModel[]> GetWorkflowsForSignals(List<string> signals)
        {
            var result = new List<UsertaskViewModel>();
            await Task.WhenAll(signals.Select(async i => result.AddRange(await GetWorkflowsForSignal(i))));
            return result.ToArray();
        }


        public async Task MarkAsCompleteAsync(string workflowInstanceId, string signal, JToken signalData)
        {
            var data = new MarkAsCompletedPostModel
            {
                WorkflowInstanceId = workflowInstanceId,
                Input = signalData == null ? JValue.CreateNull() : signalData
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            await httpClient.PostAsync($"/v1/usertask-signals/{signal}/dispatch", content);
        }
    }
}
