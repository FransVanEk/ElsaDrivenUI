
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

        public async Task SendSignal(string signal, object data)
        {
            await PostObjectJson(data, $"v1/signals/{signal}/execute");
        }

        public async Task SendSignal(string signal)
        {
            await PostObjectJson(new JObject(), $"v1/signals/{signal}/execute");
        }

        private async Task PostObjectJson(object data, string url)
        {
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            await httpClient.PostAsync(url, content);
        }
    }
}
