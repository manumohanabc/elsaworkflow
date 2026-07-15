using ElsaWorkflow.Api.Models;
using ElsaWorkflow.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ElsaWorkflow.Api.Services;

public class ExecutionService
{
    private readonly WorkflowService _workflowService;
    private readonly IHubContext<WorkflowHub> _hubContext;
    private readonly Dictionary<string, ExecutionDto> _executions = new();
    private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens = new();

    public ExecutionService(WorkflowService workflowService, IHubContext<WorkflowHub> hubContext)
    {
        _workflowService = workflowService;
        _hubContext = hubContext;
    }

    public async Task<ExecutionDto> StartExecution(string workflowId, Dictionary<string, object>? input)
    {
        var workflow = _workflowService.GetWorkflow(workflowId);
        if (workflow == null)
        {
            throw new ArgumentException($"Workflow {workflowId} not found");
        }

        var executionId = Guid.NewGuid().ToString();
        var execution = new ExecutionDto
        {
            ExecutionId = executionId,
            WorkflowId = workflowId,
            WorkflowName = workflow.Name,
            Status = ExecutionStatus.Running,
            StartedAt = DateTime.UtcNow,
            Tasks = workflow.Tasks.Select(t => new TaskExecutionDto
            {
                TaskId = t.Id,
                TaskName = t.Name,
                Status = TaskExecutionStatus.Pending
            }).ToList()
        };

        _executions[executionId] = execution;
        var cts = new CancellationTokenSource();
        _cancellationTokens[executionId] = cts;

        await _hubContext.Clients.All.SendAsync("WorkflowStarted", new { executionId, workflowName = workflow.Name });

        _ = ExecuteWorkflowAsync(execution, workflow, cts.Token);

        return execution;
    }

    private async Task ExecuteWorkflowAsync(ExecutionDto execution, WorkflowDefinition workflow, CancellationToken cancellationToken)
    {
        try
        {
            var executedTasks = new HashSet<string>();

            while (executedTasks.Count < workflow.Tasks.Count)
            {
                var readyTasks = workflow.Tasks.Where(t =>
                    !executedTasks.Contains(t.Id) &&
                    t.Dependencies.All(dep => executedTasks.Contains(dep))
                ).ToList();

                if (!readyTasks.Any())
                {
                    break;
                }

                var tasks = readyTasks.Select(t => ExecuteTaskAsync(execution, t, cancellationToken)).ToList();
                await Task.WhenAll(tasks);

                foreach (var task in readyTasks)
                {
                    executedTasks.Add(task.Id);
                }
            }

            execution.Status = ExecutionStatus.Completed;
            execution.CompletedAt = DateTime.UtcNow;
            execution.Output = new Dictionary<string, object>
            {
                { "message", "Workflow completed successfully" },
                { "executedTasks", executedTasks.Count },
                { "totalTasks", workflow.Tasks.Count }
            };

            await _hubContext.Clients.All.SendAsync("WorkflowCompleted", new
            {
                executionId = execution.ExecutionId,
                status = "Success",
                output = execution.Output
            });
        }
        catch (OperationCanceledException)
        {
            execution.Status = ExecutionStatus.Cancelled;
            execution.CompletedAt = DateTime.UtcNow;
            await _hubContext.Clients.All.SendAsync("ExecutionCancelled", new { executionId = execution.ExecutionId });
        }
        catch (Exception ex)
        {
            execution.Status = ExecutionStatus.Failed;
            execution.CompletedAt = DateTime.UtcNow;
            execution.Error = ex.Message;

            await _hubContext.Clients.All.SendAsync("ExecutionFailed", new
            {
                executionId = execution.ExecutionId,
                error = ex.Message
            });
        }

        _cancellationTokens.Remove(execution.ExecutionId);
    }

    private async Task ExecuteTaskAsync(ExecutionDto execution, TaskDefinition task, CancellationToken cancellationToken)
    {
        var taskExecution = execution.Tasks.First(t => t.TaskId == task.Id);
        taskExecution.Status = TaskExecutionStatus.Running;
        taskExecution.StartedAt = DateTime.UtcNow;

        await _hubContext.Clients.All.SendAsync("TaskStarted", new
        {
            executionId = execution.ExecutionId,
            taskId = task.Id,
            taskName = task.Name
        });

        try
        {
            await Task.Delay(task.DurationMs, cancellationToken);

            taskExecution.Status = TaskExecutionStatus.Completed;
            taskExecution.CompletedAt = DateTime.UtcNow;
            taskExecution.Output = new Dictionary<string, object>
            {
                { "result", $"Task {task.Name} executed successfully" },
                { "duration", task.DurationMs }
            };

            await _hubContext.Clients.All.SendAsync("TaskCompleted", new
            {
                executionId = execution.ExecutionId,
                taskId = task.Id,
                taskName = task.Name,
                output = taskExecution.Output
            });
        }
        catch (OperationCanceledException)
        {
            taskExecution.Status = TaskExecutionStatus.Skipped;
            taskExecution.CompletedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            taskExecution.Status = TaskExecutionStatus.Failed;
            taskExecution.CompletedAt = DateTime.UtcNow;
            taskExecution.Error = ex.Message;

            await _hubContext.Clients.All.SendAsync("TaskFailed", new
            {
                executionId = execution.ExecutionId,
                taskId = task.Id,
                taskName = task.Name,
                error = ex.Message
            });
        }
    }

    public ExecutionDto? GetExecution(string executionId)
    {
        _executions.TryGetValue(executionId, out var execution);
        return execution;
    }

    public List<ExecutionDto> GetRecentExecutions(int limit = 50)
    {
        return _executions.Values
            .OrderByDescending(e => e.StartedAt)
            .Take(limit)
            .ToList();
    }

    public void CancelExecution(string executionId)
    {
        if (_cancellationTokens.TryGetValue(executionId, out var cts))
        {
            cts.Cancel();
        }
    }
}
