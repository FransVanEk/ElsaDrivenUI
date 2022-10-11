using Elsa.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace UserTask.AddOns.Notifications
{
    public class WorkflowNotifier : INotificationHandler<ActivityExecuted>,
                                    INotificationHandler<WorkflowCompleted>,
                                    INotificationHandler<ActivityResuming>,
                                    INotificationHandler<ActivityExecuting>,
                                    INotificationHandler<ActivityActivating>
    {
        private readonly IHubContext<WorkflowInstanceInfoHub, IWorkflowInstanceInfoHub> _hubContext;

        public WorkflowNotifier(IHubContext<WorkflowInstanceInfoHub, IWorkflowInstanceInfoHub> hubContext)
        {
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        private static WorkflowInstanceInfo GetInfo(ActivityNotification activityNotification, string action)
        {
            var workflowInstance = activityNotification.WorkflowExecutionContext.WorkflowInstance;

            return new WorkflowInstanceInfo
            {
                WorkflowInstanceId = workflowInstance.Id,
                WorkflowState = workflowInstance.WorkflowStatus.ToString(),
                ActivityName = activityNotification.ActivityBlueprint.Name,
                ActivityId = activityNotification.Activity.Id,
                Action = $"Activity.{action}",
                IsUsertask = IsUsertask(activityNotification.Activity.GetType()),
                Description = $"{(string.IsNullOrEmpty(activityNotification.ActivityBlueprint.Name) ? activityNotification.Activity.Id : activityNotification.ActivityBlueprint.Name)}"
            };
        }


        private async Task SendNotification(string workflowInstanceId, WorkflowInstanceInfo workflowInstanceInfo)
        {
            await _hubContext.Clients.Group(workflowInstanceId).WorkflowInstanceUpdate(workflowInstanceInfo);
        }

        public async Task Handle(WorkflowCompleted notification, CancellationToken cancellationToken)
        {
            var workflowInstanceId = notification.WorkflowExecutionContext.WorkflowInstance.Id;
            var workflowInstanceInfo = new WorkflowInstanceInfo
            {
                WorkflowInstanceId = workflowInstanceId,
                ActivityName = String.Empty,
                Action = "Workflow.Ended",
                Description = "Workflow has ended"
            };
            await SendNotification(workflowInstanceId, workflowInstanceInfo);
        }

        public async Task Handle(ActivityResuming notification, CancellationToken cancellationToken)
        {
            await SendNotification(
                notification.ActivityExecutionContext.WorkflowInstance.Id,
                GetInfo(notification, "Resuming")
                );
        }

        public async Task Handle(ActivityExecuting notification, CancellationToken cancellationToken)
        {
            await SendNotification(
                notification.ActivityExecutionContext.WorkflowInstance.Id,
                GetInfo(notification, "Executing")
                );
        }

        public async Task Handle(ActivityExecuted notification, CancellationToken cancellationToken)
        {
            await SendNotification(
                notification.ActivityExecutionContext.WorkflowInstance.Id,
                GetInfo(notification, "Executed")
                );
        }

        public async Task Handle(ActivityActivating notification, CancellationToken cancellationToken)
        {
            var workflowInstance = notification.ActivityExecutionContext.WorkflowInstance;
            
            await SendNotification(
               notification.ActivityExecutionContext.WorkflowInstance.Id,
               new WorkflowInstanceInfo
               {
                   WorkflowInstanceId = workflowInstance.Id,
                   WorkflowState = workflowInstance.WorkflowStatus.ToString(),
                   ActivityName = notification.ActivityExecutionContext.ActivityBlueprint.Name,
                   ActivityId = notification.ActivityExecutionContext.ActivityBlueprint.Id,
                   Action = $"Activity.Activation",
                   IsUsertask = IsUsertask(notification.ActivityExecutionContext.ActivityBlueprint.Type.GetType()),
                   Description = $"Activating"
               }
               );
        }
        private static bool IsUsertask(Type activityType)
        {
            var usertaskType = typeof(UserTaskSignal);
            return activityType == usertaskType || activityType.GetType().IsSubclassOf(typeof(UserTaskSignal));
        }

    
    }
}
