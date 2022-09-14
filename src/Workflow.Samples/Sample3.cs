using Elsa.Activities.Console;
using Elsa.Activities.Primitives;
using Elsa.Activities.Signaling;
using Elsa.Builders;
using UserTask.AddOns;

namespace Workflow.Samples
{
    internal class Sample3 : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            builder
                .StartWith<SignalReceived>(a =>
                {
                    a.Set(x => x.Signal, "sample3");
                    a.Set(x => x.Id, "SignalReceived3");

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
                    a.Set(x => x.Signal, "usertasksample3");
                    a.Set(x => x.TaskName, "Demo Sample3");
                    a.Set(x => x.TaskTitle, "Dynamic forms");
                    a.Set(x => x.UIDefinition, "{\"groups\":[{\"name\":\"demo\",\"layoutHint\":\"\",\"subGroups\":[{\"layoutHint\":\"\",\"index\":1,\"items\":[{\"index\":1,\"span\":4,\"path\":\"$.demo1.test3\",\"typeName\":\"NumberInput\",\"layoutHint\":\"\",\"text\":\"leeftijd\",\"groups\":[],\"customData\":{}},{\"index\":2,\"span\":8,\"path\":\"$.demo1.test2\",\"typeName\":\"TextInput\",\"layoutHint\":\"\",\"text\":\"leeftijd in text\",\"groups\":[],\"customData\":{}}]},{\"layoutHint\":\"\",\"index\":2,\"items\":[{\"index\":1,\"span\":4,\"path\":\"$.demo1.test1\",\"typeName\":\"DateInput\",\"layoutHint\":\"\",\"text\":\"datum\",\"groups\":[],\"customData\":{}},{\"index\":2,\"span\":8,\"path\":\"$.demo1.test2\",\"typeName\":\"TextInput\",\"layoutHint\":\"\",\"text\":\"leeftijd in text\",\"groups\":[],\"customData\":{}}]}]}],\"title\":\"leeftijds vragen\"}");
                    a.Set(x => x.TaskName, "The task will suspend the execution until the button is pressed");
                }
                )
                .WriteLine($"Workflow is done");
        }
    }
}