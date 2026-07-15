import { useState } from 'react';
import { useAuth } from './hooks/useAuth';
import { LoginForm } from './components/LoginForm';
import { WorkflowList } from './components/WorkflowList';
import { WorkflowExecutor } from './components/WorkflowExecutor';
import { WorkflowDefinition } from './services/api';
import './App.css';

function App() {
  const { isAuthenticated, username, logout } = useAuth();
  const [selectedWorkflow, setSelectedWorkflow] = useState<WorkflowDefinition | null>(null);

  if (!isAuthenticated) {
    return <LoginForm onLoginSuccess={() => window.location.reload()} />;
  }

  return (
    <div className="app">
      <header className="app-header">
        <div className="header-content">
          <h1>⚙️ ElsaWorkflow Showcase</h1>
          <div className="user-info">
            <span className="username">Logged in as: {username}</span>
            <button className="logout-button" onClick={logout}>
              Logout
            </button>
          </div>
        </div>
      </header>

      <main className="app-main">
        {!selectedWorkflow ? (
          <WorkflowList onSelectWorkflow={setSelectedWorkflow} />
        ) : (
          <WorkflowExecutor
            workflow={selectedWorkflow}
            onClose={() => setSelectedWorkflow(null)}
          />
        )}
      </main>

      <footer className="app-footer">
        <p>Real-time Workflow Execution Monitoring with SignalR | ElsaWorkflow + .NET Core 8 + React</p>
      </footer>
    </div>
  );
}

export default App;
