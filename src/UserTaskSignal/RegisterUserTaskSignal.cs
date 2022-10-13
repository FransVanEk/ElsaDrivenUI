using Elsa.Options;
using Microsoft.Extensions.DependencyInjection;
using UserTask.AddOns.Bookmarks;
using UserTask.AddOns.Endpoints.Models;

namespace UserTask.AddOns
{
    public static class RegisterUserTaskSignal
    {
        public static ElsaOptionsBuilder AddUserTaskSignalActivities(this ElsaOptionsBuilder options, string engineId)
        {
            options.Services.AddScoped<IUserTaskSignalInvoker, UserTaskSignalInvoker>();
            options.Services.AddBookmarkProvider<UserTaskSignalBookmarkProvider>();
            options.Services.AddSingleton(opt =>  new ServerContext(engineId));
            options.AddActivity<UserTaskSignal>();
            return options;
        }
    }
}
