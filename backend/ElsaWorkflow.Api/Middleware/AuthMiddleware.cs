using ElsaWorkflow.Api.Services;

namespace ElsaWorkflow.Api.Middleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AuthService _authService;

    public AuthMiddleware(RequestDelegate next, AuthService authService)
    {
        _next = next;
        _authService = authService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";

        // Skip auth for login endpoint
        if (path.StartsWith("/api/auth/login", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Check for auth token
        var token = ExtractToken(context.Request);

        if (!string.IsNullOrEmpty(token) && _authService.ValidateToken(token))
        {
            var username = _authService.GetUsernameFromToken(token);
            if (!string.IsNullOrEmpty(username))
            {
                context.Items["Username"] = username;
                context.Items["Token"] = token;
            }
        }

        await _next(context);
    }

    private string? ExtractToken(HttpRequest request)
    {
        // Try Authorization header
        var authHeader = request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length);
        }

        // Try query string (for SignalR)
        if (request.Query.TryGetValue("access_token", out var queryToken))
        {
            return queryToken.ToString();
        }

        return null;
    }
}
