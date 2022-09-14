using Elsa.Models;
using UserTask.AddOns.Endpoints.Models;

namespace UserTask.AddOns.Extensions
{
    public static class ViewModelExtensions
    {
        internal static List<WorkfowInstanceUsertaskViewModel> ConvertToWorkflowInstanceUsertaskViewModels(this IEnumerable<WorkflowInstance> source, ServerContext serverContext, IEnumerable<Bookmark> bookmarks)
        {
            var result = new List<WorkfowInstanceUsertaskViewModel>();
            source.ToList().ForEach(x =>
            result.Add(new WorkfowInstanceUsertaskViewModel
            {
                WorkflowName = x.Name ?? "not set",
                LastExecuted = x.LastExecutedActivityId,
                State = x.WorkflowStatus.ToString(),
                DefinitionId = x.DefinitionId,
                UserTasks = x.ConvertToUsertaskViewModels(serverContext)
            })
            );
            return result;
        }

        internal static List<UsertaskViewModel> ConvertToUsertaskViewModels(this IEnumerable<WorkflowInstance> source, ServerContext serverContext)
        {
            var result = new List<UsertaskViewModel>();
            source.ToList().ForEach(i => result.AddRange(i.ConvertToUsertaskViewModels(serverContext)));
            return result;
        }

        internal static List<UsertaskViewModel> ConvertToUsertaskViewModels(this WorkflowInstance i, ServerContext serverContext)
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
