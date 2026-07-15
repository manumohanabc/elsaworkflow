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
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins("http://localhost:5173", "http://localhost:3000");
    });
});

builder.Services.AddSignalR();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<WorkflowService>();
builder.Services.AddSingleton<ExecutionService>();

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseRouting();
app.UseMiddleware<AuthMiddleware>();

app.MapControllers();
app.MapHub<WorkflowHub>("/workflowHub");

await app.RunAsync("http://localhost:5000");
