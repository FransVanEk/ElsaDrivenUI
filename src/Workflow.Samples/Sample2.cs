using Elsa.Activities.Console;
using Elsa.Activities.Primitives;
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
                    a.Set(x => x.Signal, "sample2");
                    a.Set(x => x.Id, "SignalReceived2");

                })
                 .Then<SetVariable>(a =>
                    {
                        a.Set(x => x.VariableName, "Name");
                        a.Set(x => x.Value, context =>  context.GetInput<SampleSettings>());
                    })
                .SetName(context => ((SampleSettings)context.GetVariable("Name")).Name)
                .WriteLine($"Starting workflow")
                .Then<UserTaskSignal>(a =>
                {
                    a.Set(x => x.Signal, "usertasksample2");
                    a.Set(x => x.TaskName, "Demo Sample2");
                    a.Set(x => x.TaskTitle, "Press the button to continue");
                    a.Set(x => x.TaskName, "The task will suspend the execution until the button is pressed");
                }
                )
                .WriteLine($"Starting workflow")
                .Then<UserTaskSignal>(a =>
                 {
                     a.Set(x => x.Signal, "usertasksample2a");
                     a.Set(x => x.TaskName, "Demo Sample2a");
                     a.Set(x => x.TaskTitle, "Press the button to continue for 2a");
                     a.Set(x => x.TaskName, "The task will suspend the execution until the button is pressed");
                 })
                .WriteLine($"Workflow is done");
        }
    }
}