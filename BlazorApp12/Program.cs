using Azure.Identity;
using BlazorApp12;
using BlazorApp12.AIPlugins;
using BlazorApp12.Pages;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TemplateEngine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddServerComponents();

builder.Services.AddSingleton<IPromptTemplateEngine, PromptTemplateEngine>();

builder.Services.AddTransient(sp =>
{
    var section = builder.Configuration.GetSection("AzureChatCompletionService");
    return Kernel.Builder
        .WithAzureChatCompletionService(
            section.GetValue<string>("DeployName")!,
            section.GetValue<string>("Endpoint")!,
            new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ExcludeVisualStudioCredential = true,
            }))
        .WithLogger(sp.GetRequiredService<ILogger<Kernel>>())
        .WithPromptTemplateEngine(sp.GetRequiredService<IPromptTemplateEngine>())
        .Build();
});

builder.Services.AddSingleton<IPageInfoProvider>(sp =>
{
    var pageInfoProvider = new PageInfoProvider();
    pageInfoProvider.AddPage("WeatherForecast", "showdata", "気温の予報の一覧を表示するページです。");
    pageInfoProvider.AddPage("StockForecast", "showstockdata", "株価の予測の一覧を表示するページです。");
    pageInfoProvider.AddPage("Counter", "counter", "ボタンを押した回数をカウントするページ");
    return pageInfoProvider;
});

builder.Services.AddScoped<PageSuggestionService>();

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

app.MapRazorComponents<App>();

app.Run();
