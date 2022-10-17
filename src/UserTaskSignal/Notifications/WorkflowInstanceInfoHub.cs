using Microsoft.AspNetCore.SignalR;

namespace UserTask.AddOns.Notifications
{
    public interface IWorkflowInstanceInfoHub
    {
        Task WorkflowInstanceUpdate(WorkflowInstanceInfo workflowInstanceInfo);
    }

    public class WorkflowInstanceInfo
    {
        public string WorkflowInstanceId { get; internal set; }
        public string WorkflowState { get; internal set; }
        public string ActivityId { get;internal set; }
        public string? ActivityName { get; internal set; }
        public string Action { get; internal set; }
        public bool IsUsertask { get; internal set; }
        public string Description { get; internal set; }
    }

    public class WorkflowInstanceInfoHub : Hub<IWorkflowInstanceInfoHub>
    {
        public async Task JoinWorkflowInstanceGroup(string workflowInstanceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, workflowInstanceId);
        }

        public async Task LeaveWorkflowInstanceGroup(string workflowInstanceId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, workflowInstanceId);
        }

    }
}
