using Elsa.Options;

namespace UserTask.AddOns
{
    public static class RegisterUserTaskSignal
    {
        public static ElsaOptionsBuilder AddUserTaskSignalActivities(this ElsaOptionsBuilder options)
        {
            options.AddActivity<UserTaskSignal>();
            return options;
        }
    }
}
