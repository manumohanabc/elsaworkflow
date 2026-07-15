using Microsoft.AspNetCore.Mvc;
using ElsaWorkflow.Api.Models;
using ElsaWorkflow.Api.Services;

namespace ElsaWorkflow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExecutionController : ControllerBase
{
    private readonly ExecutionService _executionService;
    private readonly ILogger<ExecutionController> _logger;

    public ExecutionController(ExecutionService executionService, ILogger<ExecutionController> logger)
    {
        _executionService = executionService;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartExecution([FromBody] ExecutionRequest request)
    {
        if (!IsAuthenticated())
            return Unauthorized(new { message = "Authentication required" });

        try
        {
            _logger.LogInformation($"Starting execution for workflow: {request.WorkflowId}");
            var execution = await _executionService.StartExecution(request.WorkflowId, request.Input);
            return Accepted(new { executionId = execution.ExecutionId, status = execution.Status });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError($"Invalid workflow: {ex.Message}");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error starting execution: {ex.Message}");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{executionId}")]
    public IActionResult GetExecution(string executionId)
    {
        if (!IsAuthenticated())
            return Unauthorized(new { message = "Authentication required" });

        _logger.LogInformation($"Fetching execution: {executionId}");
        var execution = _executionService.GetExecution(executionId);

        if (execution == null)
        {
            _logger.LogWarning($"Execution not found: {executionId}");
            return NotFound(new { message = "Execution not found" });
        }

        return Ok(execution);
    }

    [HttpGet]
    public IActionResult GetRecentExecutions([FromQuery] int limit = 50)
    {
        if (!IsAuthenticated())
            return Unauthorized(new { message = "Authentication required" });

        _logger.LogInformation($"Fetching recent executions (limit: {limit})");
        var executions = _executionService.GetRecentExecutions(limit);
        return Ok(new ExecutionListResponse
        {
            Executions = executions,
            TotalCount = executions.Count
        });
    }

    [HttpPost("{executionId}/cancel")]
    public IActionResult CancelExecution(string executionId)
    {
        if (!IsAuthenticated())
            return Unauthorized(new { message = "Authentication required" });

        try
        {
            _logger.LogInformation($"Cancelling execution: {executionId}");
            _executionService.CancelExecution(executionId);
            return Ok(new { message = "Execution cancelled" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cancelling execution: {ex.Message}");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    private bool IsAuthenticated()
    {
        return HttpContext.Items.ContainsKey("Username");
    }
}
