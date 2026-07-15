using Microsoft.AspNetCore.SignalR;

namespace ElsaWorkflow.Api.Hubs;

public class WorkflowHub : Hub
{
    private readonly ILogger<WorkflowHub> _logger;

    public WorkflowHub(ILogger<WorkflowHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Items["Username"]?.ToString() ?? "Anonymous";
        _logger.LogInformation($"User {userId} connected to WorkflowHub. ConnectionId: {Context.ConnectionId}");

        await Clients.Caller.SendAsync("Connected", new { connectionId = Context.ConnectionId, userId });
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Items["Username"]?.ToString() ?? "Anonymous";
        _logger.LogInformation($"User {userId} disconnected from WorkflowHub. ConnectionId: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinExecutionGroup(string executionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"execution-{executionId}");
        var userId = Context.GetHttpContext()?.Items["Username"]?.ToString() ?? "Anonymous";
        await Clients.Group($"execution-{executionId}")
            .SendAsync("UserJoined", new { userId, connectionId = Context.ConnectionId });
    }

    public async Task LeaveExecutionGroup(string executionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"execution-{executionId}");
    }
}
