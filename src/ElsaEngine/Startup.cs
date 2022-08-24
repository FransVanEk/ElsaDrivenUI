using Elsa;
using Elsa.Activities.Http.Options;
using Elsa.Persistence.EntityFramework.Core.Extensions;
using Elsa.Persistence.EntityFramework.Sqlite;

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
                    .AddWorkflowsFrom<Startup>()
                );

            // Elsa API endpoints.
            services.AddElsaApiEndpoints();

            // For Dashboard.
            services.AddRazorPages();
        }
        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app
                .UseStaticFiles() // For Dashboard.
                .UseHttpActivities()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    // Elsa API Endpoints are implemented as regular ASP.NET Core API controllers.
                    endpoints.MapControllers();

                    // For Dashboard.
                    endpoints.MapFallbackToPage("/_Host");
                });
            app.Run();
        }
    }
}

