using ElsaWorkflow.Api.Models;

namespace ElsaWorkflow.Api.Services;

public class WorkflowService
{
    private readonly List<WorkflowDefinition> _workflows;

    public WorkflowService()
    {
        _workflows = InitializeWorkflows();
    }

    private List<WorkflowDefinition> InitializeWorkflows()
    {
        return new List<WorkflowDefinition>
        {
            new()
            {
                Id = "wf-001",
                Name = "Simple Task Workflow",
                Description = "A basic workflow with sequential task execution",
                Tasks = new List<TaskDefinition>
                {
                    new() { Id = "task-1", Name = "Initialize", Description = "Initialize workflow state", DurationMs = 500 },
                    new() { Id = "task-2", Name = "Process Data", Description = "Process input data", DurationMs = 1500, Dependencies = new() { "task-1" } },
                    new() { Id = "task-3", Name = "Generate Report", Description = "Generate execution report", DurationMs = 1000, Dependencies = new() { "task-2" } },
                    new() { Id = "task-4", Name = "Finalize", Description = "Finalize and return results", DurationMs = 500, Dependencies = new() { "task-3" } }
                }
            },
            new()
            {
                Id = "wf-002",
                Name = "Parallel Task Workflow",
                Description = "A workflow with parallel task execution",
                Tasks = new List<TaskDefinition>
                {
                    new() { Id = "task-1", Name = "Start", Description = "Start workflow", DurationMs = 300 },
                    new() { Id = "task-2", Name = "Task A", Description = "Execute parallel task A", DurationMs = 1200, Dependencies = new() { "task-1" } },
                    new() { Id = "task-3", Name = "Task B", Description = "Execute parallel task B", DurationMs = 1200, Dependencies = new() { "task-1" } },
                    new() { Id = "task-4", Name = "Task C", Description = "Execute parallel task C", DurationMs = 1200, Dependencies = new() { "task-1" } },
                    new() { Id = "task-5", Name = "Aggregate", Description = "Aggregate results from parallel tasks", DurationMs = 800, Dependencies = new() { "task-2", "task-3", "task-4" } }
                }
            },
            new()
            {
                Id = "wf-003",
                Name = "Long Running Workflow",
                Description = "A workflow with longer execution time",
                Tasks = new List<TaskDefinition>
                {
                    new() { Id = "task-1", Name = "Validate Input", Description = "Validate input parameters", DurationMs = 800 },
                    new() { Id = "task-2", Name = "Process Stage 1", Description = "Execute processing stage 1", DurationMs = 2000, Dependencies = new() { "task-1" } },
                    new() { Id = "task-3", Name = "Process Stage 2", Description = "Execute processing stage 2", DurationMs = 2500, Dependencies = new() { "task-2" } },
                    new() { Id = "task-4", Name = "Process Stage 3", Description = "Execute processing stage 3", DurationMs = 2000, Dependencies = new() { "task-3" } },
                    new() { Id = "task-5", Name = "Store Results", Description = "Store execution results", DurationMs = 1000, Dependencies = new() { "task-4" } }
                }
            }
        };
    }

    public List<WorkflowDefinition> GetWorkflows()
    {
        return _workflows;
    }

    public WorkflowDefinition? GetWorkflow(string id)
    {
        return _workflows.FirstOrDefault(w => w.Id == id);
    }
}
