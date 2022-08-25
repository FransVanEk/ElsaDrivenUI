using Elsa.Activities.Signaling.Models;
using Elsa.Models;
using Elsa.Services;
using Elsa.Services.Models;

namespace UserTask.AddOns.Bookmarks
{
    public class UserTaskSignalInvoker : IUserTaskSignalInvoker
    {
        private readonly IWorkflowLaunchpad workflowLaunchpad;

        public UserTaskSignalInvoker(IWorkflowLaunchpad workflowLaunchpad)
        {
            this.workflowLaunchpad = workflowLaunchpad;
        }

        public async Task<IEnumerable<CollectedWorkflow>> ExecuteWorkflowsAsync(string signal,
            object input = null,
            string workflowInstanceId = null,
            string correlationId = null,
            CancellationToken cancellationToken = default)
        {
            string normalizedSignal = signal.ToLowerInvariant();
            IEnumerable<CollectedWorkflow> collectedWorkflows = await workflowLaunchpad.CollectAndExecuteWorkflowsAsync(
                new WorkflowsQuery(nameof(UserTaskSignal), new UserTaskSignalBookmark()
                {
                    Signal = normalizedSignal
                }, correlationId, workflowInstanceId), new WorkflowInput(new Signal(normalizedSignal, input)),
                cancellationToken).ConfigureAwait(false);
            normalizedSignal = null;
            return collectedWorkflows;
        }

        public async Task<IEnumerable<CollectedWorkflow>> DispatchWorkflowsAsync(string signal,
           object input = null,
           string workflowInstanceId = null,
           string correlationId = null,
           CancellationToken cancellationToken = default)
        {
            string normalizedSignal = signal.ToLowerInvariant();
            IEnumerable<CollectedWorkflow> collectedWorkflows = await workflowLaunchpad.CollectAndDispatchWorkflowsAsync(
                new WorkflowsQuery(nameof(UserTaskSignal), new UserTaskSignalBookmark()
                {
                    Signal = normalizedSignal
                }, correlationId, workflowInstanceId), new WorkflowInput(new Signal(normalizedSignal, input)),
                cancellationToken).ConfigureAwait(false);
            normalizedSignal = null;
            return collectedWorkflows;
        }
    }
}