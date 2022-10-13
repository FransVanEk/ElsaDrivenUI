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
using Elsa.Persistence.Specifications.WorkflowDefinitions;
using Elsa.Persistence.Specifications;
using System.Linq.Expressions;
using Elsa.Providers.Workflows;

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
        private readonly IWorkflowDefinitionStore workflowDefinitionStore;

        public UserTaskSignalController(IUserTaskSignalInvoker invoker,IWorkflowRegistry workflowRegistry,  ITriggerFinder triggerFinder, IBookmarkFinder bookmarkFinder, IWorkflowDefinitionStore workflowDefinitionStore, IWorkflowInstanceStore workflowInstanceStore, ServerContext serverContext)
        {
            this.workflowInstanceStore = workflowInstanceStore;
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
            var triggerResults = await triggerFinder.FindTriggersByTypeAsync(typeof(UserTaskSignalBookmark).GetSimpleAssemblyQualifiedName(),"");
            var triggerText = System.Text.Json.JsonSerializer.Serialize(triggerResults);
            var ids = triggerResults.Select(x => x.WorkflowDefinitionId).ToList();
            var workflowDefinitionIds = new myWorkflowDefinitionIdsSpecification(ids);
            var manyFilter = new ManyWorkflowDefinitionIdsSpecification(ids);
            var throughRegistry2 = await workflowRegistry.FindManyByDefinitionIds(ids, VersionOptions.Published, cancellationToken);


            var workflowDefinitions = await workflowDefinitionStore.FindManyAsync(manyFilter);
            var throughRegistry = await workflowRegistry.FindManyByDefinitionIds(ids,VersionOptions.All,cancellationToken);
            var throughRegistryByName = await workflowRegistry.FindManyByNames(new[] { "theName" }, cancellationToken);
            var throughRegistryByIds = await workflowRegistry.FindManyByDefinitionIds(new List<string> { "theId" } , VersionOptions.All, cancellationToken);

            //.FindManyAsync(workflowDefinitionIds, null, null, cancellationToken);

            var viewmodelResult = new List<UsertaskViewModel>
             {
                 new UsertaskViewModel{
                     TaskTitle = "new process",
                     Signal = "usertasksample5",
                     UIDefinition = "{\"groups\":[{\"name\":\"demo start\",\"layoutHint\":\"\",\"subGroups\":[{\"layoutHint\":\"\",\"index\":1,\"items\":[{\"index\":1,\"span\":4,\"path\":\"$.demo1.Lex\",\"typeName\":\"NumberInput\",\"layoutHint\":\"\",\"text\":\"leeftijd\",\"groups\":[],\"customData\":{}},{\"index\":2,\"span\":8,\"path\":\"$.demo1.test2\",\"typeName\":\"TextInput\",\"layoutHint\":\"\",\"text\":\"leeftijd in text\",\"groups\":[],\"customData\":{}}]},{\"layoutHint\":\"\",\"index\":2,\"items\":[{\"index\":1,\"span\":4,\"path\":\"$.demo1.test1\",\"typeName\":\"DateInput\",\"layoutHint\":\"\",\"text\":\"datum\",\"groups\":[],\"customData\":{}},{\"index\":2,\"span\":8,\"path\":\"$.demo1.test2\",\"typeName\":\"TextInput\",\"layoutHint\":\"\",\"text\":\"leeftijd in text\",\"groups\":[],\"customData\":{}}]}]}],\"title\":\"leeftijds vragen\"}",
                     WorkflowInstanceId = null,
                     EngineId = serverContext.EngineId,
                     TaskDescription="starting a process with a user task",
                     TaskName = "start process by usertask",
                 }
             };

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