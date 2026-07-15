using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ElsaWorkflow.Api.Hubs;
using ElsaWorkflow.Api.Middleware;
using ElsaWorkflow.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = new[]
        {
            "http://localhost:5173",
            "http://localhost:3000",
            "https://elsaworkflow-ui.onrender.com",
            "http://127.0.0.1:5173",
            "http://127.0.0.1:3000"
        };

        // Add any origins from environment variable
        var envOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS");
        if (!string.IsNullOrEmpty(envOrigins))
        {
            allowedOrigins = allowedOrigins.Concat(envOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToArray();
        }

        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins(allowedOrigins);
    });
});

builder.Services.AddSignalR();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<WorkflowService>();
builder.Services.AddSingleton<ExecutionService>();

// Add logging
builder.Logging.AddConsole();

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseRouting();
app.UseMiddleware<AuthMiddleware>();

app.MapControllers();
app.MapHub<WorkflowHub>("/workflowHub");

// Determine port from environment or use default
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var aspnetcoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? $"http://0.0.0.0:{port}";

await app.RunAsync(aspnetcoreUrls);
