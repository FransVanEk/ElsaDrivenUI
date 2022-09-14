using ElsaDrivenWebApp.Services;
using DynamicBlazorForm.Core;
using DynamicBlazorForm.Themes.HTML;
using Newtonsoft.Json;
using DynamicBlazorForm.Core.Layout;
using DynamicBlazorForm.Core.Layout.FluentApi;

var builder = WebApplication.CreateBuilder(args);
var baseAddress = builder.Configuration["UsertaskService:BaseAddress"];
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped(sp => new UsertaskService(new HttpClient { BaseAddress = new Uri(baseAddress) }));
builder.Services.AddScoped(sp => new ProcessService(new HttpClient { BaseAddress = new Uri(baseAddress) }));

//dynamic form
builder.Services.AddScoped(sp =>
    new DynamicElementsRepository()
        .GetHTMLDefaultSettings());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
