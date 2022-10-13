namespace UserTask.AddOns.Endpoints.Models
{
    public class ServerContext
    {
        public ServerContext(string engineId)
        {
            EngineId = engineId;
        }
        public string EngineId { get; }
    }
}
