using ElsaDrivenWebApp.Services;
using DynamicBlazorForm.Core;
using DynamicBlazorForm.Themes.HTML;
using ElsaDrivenWebApp;

var builder = Host.CreateDefaultBuilder(args) 
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
            });

builder.Build().Run();


