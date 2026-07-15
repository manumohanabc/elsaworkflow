using Microsoft.AspNetCore.Mvc;
using ElsaWorkflow.Api.Models;
using ElsaWorkflow.Api.Services;

namespace ElsaWorkflow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly WorkflowService _workflowService;
    private readonly ILogger<WorkflowController> _logger;

    public WorkflowController(WorkflowService workflowService, ILogger<WorkflowController> logger)
    {
        _workflowService = workflowService;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetWorkflows()
    {
        if (!IsAuthenticated())
            return Unauthorized(new { message = "Authentication required" });

        _logger.LogInformation("Fetching all workflows");
        var workflows = _workflowService.GetWorkflows();
        return Ok(new WorkflowListResponse { Workflows = workflows });
    }

    [HttpGet("{id}")]
    public IActionResult GetWorkflow(string id)
    {
        if (!IsAuthenticated())
            return Unauthorized(new { message = "Authentication required" });

        _logger.LogInformation($"Fetching workflow: {id}");
        var workflow = _workflowService.GetWorkflow(id);

        if (workflow == null)
        {
            _logger.LogWarning($"Workflow not found: {id}");
            return NotFound(new { message = "Workflow not found" });
        }

        return Ok(workflow);
    }

    private bool IsAuthenticated()
    {
        return HttpContext.Items.ContainsKey("Username");
    }
}
