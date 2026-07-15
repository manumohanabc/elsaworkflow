import { useState, useEffect } from 'react';
import apiService, { WorkflowDefinition } from '../services/api';
import '../styles/components.css';

interface WorkflowListProps {
  onSelectWorkflow: (workflow: WorkflowDefinition) => void;
}

export const WorkflowList = ({ onSelectWorkflow }: WorkflowListProps) => {
  const [workflows, setWorkflows] = useState<WorkflowDefinition[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchWorkflows = async () => {
      try {
        setIsLoading(true);
        const data = await apiService.getWorkflows();
        setWorkflows(data);
        setError(null);
      } catch (err) {
        const message = err instanceof Error ? err.message : 'Failed to fetch workflows';
        setError(message);
      } finally {
        setIsLoading(false);
      }
    };

    fetchWorkflows();
  }, []);

  if (isLoading) {
    return <div className="loading">Loading workflows...</div>;
  }

  if (error) {
    return <div className="error-message">Error: {error}</div>;
  }

  return (
    <div className="workflow-list">
      <h2>Available Workflows</h2>
      <div className="workflow-grid">
        {workflows.map((workflow) => (
          <div key={workflow.id} className="workflow-card">
            <h3>{workflow.name}</h3>
            <p className="workflow-description">{workflow.description}</p>
            <p className="workflow-tasks">
              <strong>Tasks:</strong> {workflow.tasks.length}
            </p>
            <button
              className="execute-button"
              onClick={() => onSelectWorkflow(workflow)}
            >
              Execute Workflow
            </button>
          </div>
        ))}
      </div>
    </div>
  );
};
