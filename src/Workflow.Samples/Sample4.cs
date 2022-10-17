using Elsa.Activities.Console;
using Elsa.Activities.Primitives;
using Elsa.Activities.Signaling;
using Elsa.Builders;
using UserTask.AddOns;
using NodaTime;

namespace Workflow.Samples
{
    internal class Sample4 : IWorkflow
    {
        public void Build(IWorkflowBuilder builder)
        {
            builder
                .StartWith<SignalReceived>(a =>
                {
                    a.Set(x => x.Signal, "sample4");
                    a.Set(x => x.Id, "SignalReceived4");

                }).WithName("Signal received")
                 .Then<SetVariable>(a =>
                    {
                        a.Set(x => x.VariableName, "Name");
                        a.Set(x => x.Value, context =>  context.GetInput<SampleSettings>());
                    }).WithName("Signal received").WithDisplayName("mooi man")
                .SetName(context => ((SampleSettings)context.GetVariable("Name")).Name).WithName("Variable initialized")
                .WriteLine($"Starting workflow").WithName("Write log")
                .Then<UserTaskSignal>(a =>
                {
                    a.Set(x => x.Signal, "usertasksample4");
                    a.Set(x => x.TaskName, "Demo Sample4");
                    a.Set(x => x.TaskTitle, "Dynamic forms");
                    a.Set(x => x.UIDefinition, "{\"groups\":[{\"name\":\"demo\",\"layoutHint\":\"\",\"subGroups\":[{\"layoutHint\":\"\",\"index\":1,\"items\":[{\"index\":1,\"span\":4,\"path\":\"$.demo1.test3\",\"typeName\":\"NumberInput\",\"layoutHint\":\"\",\"text\":\"leeftijd\",\"groups\":[],\"customData\":{}},{\"index\":2,\"span\":8,\"path\":\"$.demo1.test2\",\"typeName\":\"TextInput\",\"layoutHint\":\"\",\"text\":\"leeftijd in text\",\"groups\":[],\"customData\":{}}]},{\"layoutHint\":\"\",\"index\":2,\"items\":[{\"index\":1,\"span\":4,\"path\":\"$.demo1.test1\",\"typeName\":\"DateInput\",\"layoutHint\":\"\",\"text\":\"datum\",\"groups\":[],\"customData\":{}},{\"index\":2,\"span\":8,\"path\":\"$.demo1.test2\",\"typeName\":\"NumberInput\",\"layoutHint\":\"\",\"text\":\"leeftijd in text\",\"groups\":[],\"customData\":{}}]}]}],\"title\":\"input\"}");
                    a.Set(x => x.TaskName, "The task will suspend the execution until the button is pressed");
                }
                ).WithName("Age questions")
                .Then<Elsa.Activities.Timers.Timer>(a => { a.Set(x => x.Timeout, Duration.FromSeconds(3)); }).WithName("Fetching data")
                .SetName(context => ((SampleSettings)context.GetVariable("Name")).Name).WithName("Variable initialized")
                .Then<Elsa.Activities.Timers.Timer>(a => { a.Set(x => x.Timeout, Duration.FromSeconds(2)); }).WithName("Validating")
                .Then<Elsa.Activities.Timers.Timer>(a => { a.Set(x => x.Timeout, Duration.FromSeconds(1)); }).WithName("Calculating")
                .Then<UserTaskSignal>(a =>
                {
                    a.Set(x => x.Signal, "usertasksample4a");
                    a.Set(x => x.TaskName, "Demo Sample4");
                    a.Set(x => x.TaskTitle, "Dynamic forms 2");
                    a.Set(x => x.UIDefinition, "{\"groups\":[{\"name\":\"demo\",\"layoutHint\":\"\",\"subGroups\":[{\"layoutHint\":\"\",\"index\":1,\"items\":[{\"index\":1,\"span\":4,\"path\":\"$.number\",\"typeName\":\"NumberInput\",\"layoutHint\":\"\",\"text\":\"number\",\"groups\":[],\"customData\":{}},{\"index\":2,\"span\":8,\"path\":\"$.myText\",\"typeName\":\"TextInput\",\"layoutHint\":\"\",\"text\":\"initial text\",\"groups\":[],\"customData\":{}}]},{\"layoutHint\":\"\",\"index\":2,\"items\":[{\"index\":1,\"span\":4,\"path\":\"$.datefield\",\"typeName\":\"DateInput\",\"layoutHint\":\"\",\"text\":\"date\",\"groups\":[],\"customData\":{}},{\"index\":2,\"span\":8,\"path\":\"$.otherText\",\"typeName\":\"TextInput\",\"layoutHint\":\"\",\"text\":\"next text\",\"groups\":[],\"customData\":{}}]}]}],\"title\":\"random text\"}");
                    a.Set(x => x.TaskName, "Second usertask");
                }
                ).WithName("Random text question")
                .WriteLine($"Workflow is done").WithName("Log finializing the workflow");
        }
    }
}