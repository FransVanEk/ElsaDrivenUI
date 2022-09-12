using Elsa;
using Elsa.Activities.Signaling.Models;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Design;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;

namespace UserTask.AddOns
{
    /// <summary>
    /// Suspends workflow execution until the specified signal is received.
    /// </summary>
    [Trigger(
        Category = "Usertasks",
        Description = "Suspend workflow execution until the specified signal is received.",
        Outcomes = new[] { OutcomeNames.Done }
    )]
    public class UserTaskSignal : Activity
    {
        [ActivityInput(Hint = "The name of the signal to wait for.",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string Signal { get; set; } = default!;

        [ActivityOutput(Hint = "The input that was received with the signal.")]
        public object SignalInput { get; set; }

        [ActivityInput(
        Hint = "The task name",
         Category = "Task"
        )]
        public string TaskName { get; set; }

        [ActivityInput(
          Hint = "The title of the task that needs to be executed.",
           Category = "Task"
          )]
        public string TaskTitle { get; set; }

        [ActivityInput(
            Hint = "The description of the task that needs to be executed.",
             UIHint = ActivityInputUIHints.MultiLine,
             Category = "Task"
            )]
        public string TaskDescription { get; set; }

        [ActivityInput(
            Hint = "The definition of the data expected to be returned",
            UIHint = ActivityInputUIHints.MultiLine,
            Category = "Task",
            SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string UIDefinition { get; set; }

        [ActivityInput(
           Hint = "Context data for the usertask",
           UIHint = ActivityInputUIHints.MultiLine,
           Category = "Task",
           SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public object TaskData { get; set; }

        [ActivityOutput] public object Output { get; set; }

        protected override bool OnCanExecute(ActivityExecutionContext context)
        {
            if (context.Input is Signal triggeredSignal)
                return string.Equals(triggeredSignal.SignalName, Signal, StringComparison.OrdinalIgnoreCase);
            return false;
        }

        protected override IActivityExecutionResult OnExecute(ActivityExecutionContext context) =>
            context.WorkflowExecutionContext.IsFirstPass ? OnResume(context) : Suspend();

        protected override IActivityExecutionResult OnResume(ActivityExecutionContext context)
        {
            var triggeredSignal = context.GetInput<Signal>()!;
            SignalInput = triggeredSignal.Input;
            Output = triggeredSignal.Input;
            context.LogOutputProperty(this, nameof(Output), Output);
            return Done();
        }
    }
}