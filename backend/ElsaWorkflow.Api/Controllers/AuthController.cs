using Microsoft.AspNetCore.Mvc;
using ElsaWorkflow.Api.Models;
using ElsaWorkflow.Api.Services;

namespace ElsaWorkflow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation($"Login attempt for user: {request.Username}");

        var response = _authService.Login(request);

        if (!response.Success)
        {
            _logger.LogWarning($"Failed login attempt for user: {request.Username}");
            return Unauthorized(response);
        }

        _logger.LogInformation($"Successful login for user: {request.Username}");
        return Ok(response);
    }
}
