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
    pageInfoProvider.AddPage("WeatherForecast", "showdata", "�C���̗\��̈ꗗ��\������y�[�W�ł��B");
    pageInfoProvider.AddPage("StockForecast", "showstockdata", "�����̗\���̈ꗗ��\������y�[�W�ł��B");
    pageInfoProvider.AddPage("Counter", "counter", "�{�^�����������񐔂��J�E���g����y�[�W");
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
