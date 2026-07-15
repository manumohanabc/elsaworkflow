namespace ElsaWorkflow.Api.Models;

public class ExecutionRequest
{
    public string WorkflowId { get; set; } = string.Empty;
    public Dictionary<string, object>? Input { get; set; }
}

public class ExecutionDto
{
    public string ExecutionId { get; set; } = string.Empty;
    public string WorkflowId { get; set; } = string.Empty;
    public string WorkflowName { get; set; } = string.Empty;
    public ExecutionStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<TaskExecutionDto> Tasks { get; set; } = new();
    public string? Error { get; set; }
    public Dictionary<string, object>? Output { get; set; }
}

public class TaskExecutionDto
{
    public string TaskId { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public TaskExecutionStatus Status { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Dictionary<string, object>? Output { get; set; }
    public string? Error { get; set; }
}

public enum ExecutionStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Cancelled
}

public enum TaskExecutionStatus
{
    Pending,
    Running,
    Completed,
    Failed,
    Skipped
}

public class ExecutionListResponse
{
    public List<ExecutionDto> Executions { get; set; } = new();
    public int TotalCount { get; set; }
}
