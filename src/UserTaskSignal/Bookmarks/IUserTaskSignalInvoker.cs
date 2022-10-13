using Elsa.Services.Models;

namespace UserTask.AddOns.Bookmarks
{
    public interface IUserTaskSignalInvoker
    {
        Task<IEnumerable<CollectedWorkflow>> ExecuteWorkflowsAsync(string signal,
            object input = null,
            string workflowInstanceId = null,
            string correlationId = null,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<CollectedWorkflow>> DispatchWorkflowsAsync(string signal,
            object input = null,
            string workflowInstanceId = null,
            string correlationId = null,
            CancellationToken cancellationToken = default);
    }
}