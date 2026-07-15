namespace ElsaWorkflow.Api.Models;

public class WorkflowDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<TaskDefinition> Tasks { get; set; } = new();
}

public class TaskDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMs { get; set; } = 1000;
    public List<string> Dependencies { get; set; } = new();
}

public class WorkflowListResponse
{
    public List<WorkflowDefinition> Workflows { get; set; } = new();
}
