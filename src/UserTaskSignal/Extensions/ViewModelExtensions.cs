using Elsa.Models;
using UserTask.AddOns.Endpoints.Models;

namespace UserTask.AddOns.Extensions
{
    public static class ViewModelExtensions
    {
        internal static List<UsertaskViewModel> ConvertToViewModels(this IEnumerable<WorkflowInstance> source, ServerContext serverContext)
        {
            var result = new List<UsertaskViewModel>();
            source.ToList().ForEach(i => result.AddRange(i.ConvertToViewModels(serverContext)));
            return result;
        }

        internal static List<UsertaskViewModel> ConvertToViewModels(this WorkflowInstance i, ServerContext serverContext)
        {
            var result = new List<UsertaskViewModel>();
            i.BlockingActivities.ToList().ForEach(b => result.Add(b.ConvertToViewModel(i, serverContext)));
            return result;
        }
        internal static UsertaskViewModel ConvertToViewModel(this BlockingActivity b, WorkflowInstance i, ServerContext serverContext)
        {
            var result = new UsertaskViewModel();
            var data = i.ActivityData[b.ActivityId];
            result.WorkflowInstanceId = i.Id;
            result.Signal = data["Signal"].ToString();
            result.TaskDescription = data["TaskDescription"]?.ToString();
            result.TaskName = data["TaskName"]?.ToString();
            result.TaskTitle = data["TaskTitle"]?.ToString();
            result.TaskData = data["TaskData"]?.ToString();
            result.UIDefinition = data["UIDefinition"]?.ToString();
            result.EngineId = serverContext.EngineId;
            return result;
        }
    }
}
