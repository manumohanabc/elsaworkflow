import { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import '../styles/components.css';

export const LoginForm = ({ onLoginSuccess }: { onLoginSuccess: () => void }) => {
  const { login, isLoading, error: authError } = useAuth();
  const [username, setUsername] = useState('demo');
  const [password, setPassword] = useState('demo123');
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    const result = await login({ username, password });
    if (result.success) {
      onLoginSuccess();
    } else {
      setError(result.message);
    }
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <h1>ElsaWorkflow Showcase</h1>
        <p className="subtitle">Real-time Workflow Execution Monitoring</p>

        <form onSubmit={handleSubmit} className="login-form">
          <div className="form-group">
            <label htmlFor="username">Username</label>
            <input
              id="username"
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              disabled={isLoading}
              placeholder="Enter username"
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={isLoading}
              placeholder="Enter password"
            />
          </div>

          {(error || authError) && (
            <div className="error-message">
              {error || authError}
            </div>
          )}

          <button type="submit" disabled={isLoading} className="login-button">
            {isLoading ? 'Logging in...' : 'Login'}
          </button>
        </form>

        <div className="demo-credentials">
          <p><strong>Demo Credentials:</strong></p>
          <p>Username: demo | Password: demo123</p>
          <p>Username: admin | Password: admin123</p>
        </div>
      </div>
    </div>
  );
};
