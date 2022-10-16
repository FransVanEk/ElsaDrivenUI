using Elsa.Persistence;
using Elsa.Persistence.Specifications.WorkflowInstances;
using Elsa.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UserTask.AddOns.Bookmarks;
using Elsa.Server.Api.ActionFilters;
using Swashbuckle.AspNetCore.Annotations;
using Elsa.Server.Api.Endpoints.Signals;
using UserTask.AddOns.Endpoints.Models;
using UserTask.AddOns.Extensions;
using Rebus.Extensions;
using Elsa.Models;
using Elsa.Persistence.Specifications;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Elsa;

namespace UserTask.AddOns.Endpoints
{
    [ApiController]
    [Route("v{apiVersion:apiVersion}/usertask-signals")]
    [Produces("application/json")]
    public class UserTaskSignalController : Controller
    {
        private readonly IUserTaskSignalInvoker invoker;
        private readonly IWorkflowRegistry workflowRegistry;
        private readonly ITriggerFinder triggerFinder;
        private readonly IBookmarkFinder bookmarkFinder;
        private readonly ServerContext serverContext;
        private readonly IWorkflowInstanceStore workflowInstanceStore;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IWorkflowDefinitionStore workflowDefinitionStore;

        public UserTaskSignalController(IUserTaskSignalInvoker invoker,
                                        IWorkflowRegistry workflowRegistry,
                                        ITriggerFinder triggerFinder,
                                        IBookmarkFinder bookmarkFinder,
                                        IWorkflowDefinitionStore workflowDefinitionStore,
                                        IWorkflowInstanceStore workflowInstanceStore,
                                        IServiceScopeFactory scopeFactory,
                                        ServerContext serverContext)
        {
            this.workflowInstanceStore = workflowInstanceStore;
            this.scopeFactory = scopeFactory;
            this.serverContext = serverContext;
            this.invoker = invoker;
            this.workflowRegistry = workflowRegistry;
            this.triggerFinder = triggerFinder;
            this.bookmarkFinder = bookmarkFinder;
            this.workflowDefinitionStore = workflowDefinitionStore;
        }

        [HttpPost("{signalName}/execute")]
        [ElsaJsonFormatter]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExecuteSignalResponse))]
        [SwaggerOperation(
            Summary = "Signals all workflows waiting on the specified signal name synchronously.",
            Description = "Signals all workflows waiting on the specified signal name synchronously.",
            OperationId = "UsertaskSignals.Execute",
            Tags = new[] { "UsertaskSignals" })
        ]
        public async Task<IActionResult> Handle(string signalName, ExecuteSignalRequest request,
            CancellationToken cancellationToken = default)
        {
            var collectedWorkflows = await invoker.ExecuteWorkflowsAsync(signalName, request.Input,
                request.WorkflowInstanceId, request.CorrelationId, cancellationToken);
            return Ok(collectedWorkflows.ToList());
        }

        [HttpPost("{signalName}/dispatch")]
        [ElsaJsonFormatter]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExecuteSignalResponse))]
        [SwaggerOperation(
           Summary = "Signals all workflows waiting on the specified signal name asynchronously.",
           Description = "Signals all workflows waiting on the specified signal name asynchronously.",
           OperationId = "UsertaskSignals.Dispatch",
           Tags = new[] { "UsertaskSignals" })
       ]
        public async Task<IActionResult> HandleDispatch(string signalName, ExecuteSignalRequest request,
           CancellationToken cancellationToken = default)
        {
            var collectedWorkflows = await invoker.DispatchWorkflowsAsync(signalName, request.Input,
                request.WorkflowInstanceId, request.CorrelationId, cancellationToken);
            return Ok(collectedWorkflows.ToList());
        }

        [HttpGet]
        // [ElsaJsonFormatter]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExecuteSignalResponse))]
        [SwaggerOperation(
           Summary = "Gets all workflows waiting any usertaks signal",
           Description = "return a list of workflow instances",
           OperationId = "UsertaskSignals.Query",
           Tags = new[] { "UsertaskSignals" })
       ]
        public async Task<IActionResult> CollectWaitingWorkflowInstances([FromQuery] string? workflowinstanceId, CancellationToken cancellationToken = default)
        {
            var bookmarkResults = await bookmarkFinder.FindBookmarksByTypeAsync(typeof(UserTaskSignalBookmark).GetSimpleAssemblyQualifiedName());
            var workflowInstanceIds = new WorkflowInstanceIdsSpecification(bookmarkResults.Select(x => x.WorkflowInstanceId).ToList());
            if (workflowinstanceId != null)
            {
                if (bookmarkResults.All(b => b.WorkflowInstanceId != workflowinstanceId))
                {
                    var workflow = await workflowInstanceStore.FindManyAsync(new WorkflowInstanceIdsSpecification(new List<string> { workflowinstanceId }), null, null, cancellationToken);
                    return Ok(workflow.ConvertToWorkflowInstanceUsertaskViewModels(serverContext, new List<Bookmark>()));
                }
                else
                {
                    workflowInstanceIds = new WorkflowInstanceIdsSpecification(new List<string> { workflowinstanceId });
                }
            }
            var workflowInstances = await workflowInstanceStore.FindManyAsync(workflowInstanceIds, null, null, cancellationToken);
            var viewmodelResult = workflowInstances.ConvertToWorkflowInstanceUsertaskViewModels(serverContext, bookmarkResults);
            return Ok(viewmodelResult.ToList());
        }

        [HttpGet("triggers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExecuteSignalResponse))]
        [SwaggerOperation(
       Summary = "Gets all Triggers that await a usertask start activity.",
       Description = "return a list of usertask Triggers",
       OperationId = "CustomSignals.Query",
       Tags = new[] { "UsertaskSignals" })
       ]
        public async Task<IActionResult> Triggers(CancellationToken cancellationToken = default)
        {
            using var scope = scopeFactory.CreateScope();

            var triggerResults = await triggerFinder.FindTriggersByTypeAsync(typeof(UserTaskSignalBookmark).GetSimpleAssemblyQualifiedName(), "");
            var ids = triggerResults.Select(x => new { WorkflowDefinitionId = x.WorkflowDefinitionId , ActivityId = x.ActivityId }).ToList();
            var workflowBlueprintReflector = scope.ServiceProvider.GetRequiredService<IWorkflowBlueprintReflector>();
              var viewmodelResult = new List<UsertaskViewModel>();

            foreach (var trigger in ids)
            {
                var blueprints = await workflowRegistry.FindManyByDefinitionIds(new List<string> { trigger.WorkflowDefinitionId }, VersionOptions.All, cancellationToken);
                foreach (var blueprint in blueprints)
                {

                    var workflowBlueprintWrapper = await workflowBlueprintReflector.ReflectAsync(scope.ServiceProvider, blueprint, cancellationToken);

                    foreach (var activity in workflowBlueprintWrapper.Filter<UserTaskSignal>().Where(a => a.ActivityBlueprint.Id == trigger.ActivityId))
                    {
                        viewmodelResult.Add(new UsertaskViewModel
                        {
                            TaskTitle = await activity.EvaluatePropertyValueAsync(x => x.TaskTitle, cancellationToken),
                            Signal = await activity.EvaluatePropertyValueAsync(x => x.Signal, cancellationToken),
                            UIDefinition = await activity.EvaluatePropertyValueAsync(x => x.UIDefinition, cancellationToken),
                            WorkflowInstanceId = null,
                            EngineId = serverContext.EngineId,
                            TaskDescription = await activity.EvaluatePropertyValueAsync(x => x.TaskDescription, cancellationToken),
                            TaskName = await activity.EvaluatePropertyValueAsync(x => x.TaskName, cancellationToken),
                        });
                    }
                }
            }
            return Ok(viewmodelResult);
        }


        [HttpGet("{signalName}")]
        // [ElsaJsonFormatter]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExecuteSignalResponse))]
        [SwaggerOperation(
          Summary = "Gets all workflows waiting for a specific signal to be returned",
          Description = "return a list of workflow instances",
          OperationId = "CustomSignals.Query",
          Tags = new[] { "UsertaskSignals" })
      ]
        public async Task<IActionResult> Collect(string signalName,
          CancellationToken cancellationToken = default)
        {
            string normalizedSignal = signalName.ToLowerInvariant();

            var bookmarkFilter = new UserTaskSignalBookmark { Signal = normalizedSignal };
            var bookmarkResults = await bookmarkFinder.FindBookmarksAsync(nameof(UserTaskSignal), new[] { bookmarkFilter });
            var workflowInstanceIds = new WorkflowInstanceIdsSpecification(bookmarkResults.Select(x => x.WorkflowInstanceId).ToList());
            var workflowInstances = await workflowInstanceStore.FindManyAsync(workflowInstanceIds, null, null, cancellationToken);

            var viewmodelResult = workflowInstances.ConvertToUsertaskViewModels(serverContext);
            normalizedSignal = null;
            return Ok(viewmodelResult.ToList());
        }



    }

    //next section is needed for a bug in the Elsa core implementation.
    public class myWorkflowDefinitionIdsSpecification : Specification<WorkflowDefinition>
    {
        public myWorkflowDefinitionIdsSpecification(IEnumerable<string> workflowDefinitionIds)
        {
            WorkflowDefinitionIds = workflowDefinitionIds;
        }

        public IEnumerable<string> WorkflowDefinitionIds { get; }

        public override Expression<Func<WorkflowDefinition, bool>> ToExpression() => workflowDefinition => WorkflowDefinitionIds.Contains(workflowDefinition.Id);
    }
}