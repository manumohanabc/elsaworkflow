using ElsaWorkflow.Api.Models;

namespace ElsaWorkflow.Api.Services;

public class AuthService
{
    private readonly Dictionary<string, string> _users = new()
    {
        { "demo", "demo123" },
        { "admin", "admin123" },
        { "user", "user123" }
    };

    private readonly Dictionary<string, UserSession> _activeSessions = new();

    public LoginResponse Login(LoginRequest request)
    {
        if (!_users.TryGetValue(request.Username, out var password))
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        if (password != request.Password)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password"
            };
        }

        var token = GenerateToken(request.Username);

        return new LoginResponse
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            Username = request.Username
        };
    }

    private string GenerateToken(string username)
    {
        var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32)) + "|" + username + "|" + DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        _activeSessions[token] = new UserSession { Username = username, IssuedAt = DateTimeOffset.UtcNow };
        return token;
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        if (!_activeSessions.TryGetValue(token, out var session))
            return false;

        // Token valid for 24 hours
        if (DateTimeOffset.UtcNow.Subtract(session.IssuedAt).TotalHours > 24)
        {
            _activeSessions.Remove(token);
            return false;
        }

        return true;
    }

    public string? GetUsernameFromToken(string token)
    {
        if (ValidateToken(token) && _activeSessions.TryGetValue(token, out var session))
        {
            return session.Username;
        }
        return null;
    }

    private class UserSession
    {
        public string Username { get; set; } = string.Empty;
        public DateTimeOffset IssuedAt { get; set; }
    }
}
