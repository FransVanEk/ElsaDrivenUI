using Elsa.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace UserTask.AddOns.Notifications
{
    public class OnExecuteUserTask : INotificationHandler<ActivityExecuted>
    {
        private readonly IHubContext<UserTaskInfoHub, IUserTaskInfoHub> _hubContext;

        public OnExecuteUserTask(IHubContext<UserTaskInfoHub, IUserTaskInfoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(ActivityExecuted notification, CancellationToken cancellationToken)
        {
            var executedActivity = notification.Activity;

            if (executedActivity.GetType().IsSubclassOf(typeof(UserTaskSignal)))
            {
                await _hubContext.Clients.Group(notification.ActivityExecutionContext.WorkflowInstance.Id).UserTaskInitiated(new UserTaskInfo());
            }
        }
    }
}
