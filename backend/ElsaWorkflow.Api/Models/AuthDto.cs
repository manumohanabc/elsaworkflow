namespace ElsaWorkflow.Api.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}

public class AuthPayload
{
    public string Username { get; set; } = string.Empty;
    public long IssuedAt { get; set; }
    public long ExpiresAt { get; set; }
}
