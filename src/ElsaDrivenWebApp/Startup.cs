using DynamicBlazorForm.Core;
using DynamicBlazorForm.Themes.HTML;
using ElsaDrivenWebApp.Services;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ElsaDrivenWebApp
{
    public partial class Startup
    {

        private IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        partial void OnConfigureServices(IServiceCollection services);
        partial void OnConfiguringServices(IServiceCollection services);

        public void ConfigureServices(IServiceCollection services)
        {
            OnConfiguringServices(services);

            var baseAddress = _configuration["UsertaskService:BaseAddress"];
            
            // Add services to the container.
            services.AddServerSideBlazor();
            services.AddScoped(sp => new UsertaskService(new HttpClient { BaseAddress = new Uri(baseAddress) }));
            services.AddScoped(sp => new ProcessService(new HttpClient { BaseAddress = new Uri(baseAddress) }));

            //dynamic form
            services.AddScoped(sp =>
                new DynamicElementsRepository()
                    .GetHTMLDefaultSettings()
                    .Add("TextInput", typeof(TextInput))
                    .Add("NumberInput", typeof(NumberInput))
                    .Add("BoolInput", typeof(BoolInput))
                    .Add("DateInput", typeof(DateInput)));


            services.AddHttpContextAccessor();
            services.AddScoped<HttpClient>(serviceProvider =>
            {

                var uriHelper = serviceProvider.GetRequiredService<NavigationManager>();

                return new HttpClient
                {
                    BaseAddress = new Uri(uriHelper.BaseUri)
                };
            });

            services.AddHttpClient();
            services.AddRazorPages();

            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();

            OnConfigureServices(services);
        }

        partial void OnConfigure(IApplicationBuilder app, IWebHostEnvironment env);
        partial void OnConfiguring(IApplicationBuilder app, IWebHostEnvironment env);

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            OnConfiguring(app, env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.Use((ctx, next) =>
                {
                    return next();
                });
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            OnConfigure(app, env);
        }

    }
}
