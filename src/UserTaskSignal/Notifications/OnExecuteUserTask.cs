using Elsa.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace UserTask.AddOns.Notifications
{
    public class OnExecuteUserTask : INotificationHandler<ActivityExecuted>
    {
        private readonly IHubContext<UserTaskInfoHub, IUserTaskInfoHub> _hubContext;

        public string WorkflowInstanceId { get; private set; }

        public OnExecuteUserTask(IHubContext<UserTaskInfoHub, IUserTaskInfoHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(ActivityExecuted notification, CancellationToken cancellationToken)
        {
            var executedActivity = notification.Activity;
           
            if (CheckType(executedActivity.GetType()) && notification.Resuming == false)
            {
                await _hubContext.Clients.Group(notification.ActivityExecutionContext.WorkflowInstance.Id).UserTaskInitiated(new UserTaskInfo
                {
                    WorkflowInstanceId = notification.ActivityExecutionContext.WorkflowInstance.Id,
                    ActivityName = executedActivity.Name
                });
            }
        }

        private static bool CheckType(Type activityType)
        {
            var usertaskType = typeof(UserTaskSignal);
            return activityType == usertaskType || activityType.GetType().IsSubclassOf(typeof(UserTaskSignal));
        }
    }adob
}
