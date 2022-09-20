using Elsa;
using Elsa.Activities.Http.Options;
using Elsa.Events;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;
using UserTask.AddOns;
using UserTask.AddOns.Notifications;
using Workflow.Samples;

namespace ElsaEngine
{
    public class Startup
    {
        public IConfiguration configRoot
        {
            get;
        }
        public Startup(IConfiguration configuration)
        {
            configRoot = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var elsaServerUrl = configRoot.GetValue<string>("Elsa:Server:BaseUrl");
            var engineId = configRoot.GetValue<string>("Elsa:Server:EngineId");
            // Elsa services.
            services
                .AddElsa(elsa => elsa
                    .UseEntityFrameworkPersistence(ef => ef.UseSqlite())
                    .AddConsoleActivities()
                    .AddHttpActivities(a =>
                    {
                        HttpActivityOptions httpActivityOptions = new()
                        { BasePath = "", BaseUrl = new Uri(elsaServerUrl) };
                    })
                    .AddQuartzTemporalActivities()
                    .AddUserTaskSignalActivities(engineId)
                    .AddWorkflowsFrom<Startup>()
                    .AddWorkflowsFrom<Sample1>()
                );

            // Notifications
            services.AddSignalR();
            services.AddNotificationHandler<ActivityExecuted, OnExecuteUserTask>();

            // Elsa API endpoints.
            services.AddElsaApiEndpoints()
                    .AddElsaSwagger(); 
            
            // For Dashboard.
            services.AddRazorPages();
            services.AddServerSideBlazor(); // needed for notifications
        }
        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app
                .UseStaticFiles() // For Dashboard.
                .UseHttpActivities()
                .UseRouting()
                .UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elsa UI API V1"); })
                .UseCors() // needed for signalr to work
                .UseEndpoints(endpoints =>
                {
                    // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
                    endpoints.MapControllers();

                    // For Dashboard.
                    endpoints.MapFallbackToPage("/_Host");
                    
                    // Notifications
                    endpoints.MapBlazorHub();
                    endpoints.MapHub<UserTaskInfoHub>("/usertask-info");
                });
            app.Run();
        }
    }
}

