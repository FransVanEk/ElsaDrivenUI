using Microsoft.AspNetCore.SignalR;

namespace UserTask.AddOns.Notifications
{
    public interface IUserTaskInfoHub
    {
        Task UserTaskInitiated(UserTaskInfo userTask);
    }

    public class UserTaskInfo
    {
    }

    public class UserTaskInfoHub : Hub<IUserTaskInfoHub>
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
