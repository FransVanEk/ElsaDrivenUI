using Elsa.Activities.Console;
using Elsa.Activities.Http;
using Elsa.Activities.Signaling;
using Elsa.Builders;
using UserTask.AddOns;

namespace Workflow.Samples
{
    internal class Sample2 : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            builder
                .StartWith<SignalReceived>(a =>
                {
                    a.Set(x => x.Signal, "Sample2");
                    a.Set(x => x.Id, "SignalReceived2");
                })
                .WriteLine($"Starting workflow")
                .Then<UserTaskSignal>(a =>
                {
                    a.Set(x => x.Signal, "usertasksample2"); 
                    a.Set(x => x.TaskName, "Demo Sample2");
                    a.Set(x => x.TaskTitle, "Press the button to continue");
                    a.Set(x => x.TaskName, "The task will suspend the execution until the button is pressed");
                }
                )
                .WriteLine($"Workflow is done");
        }
    }
}