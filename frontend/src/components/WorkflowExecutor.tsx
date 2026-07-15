import { useState } from 'react';
import apiService, { WorkflowDefinition, ExecutionDto, ExecutionStatus } from '../services/api';
import { useSignalR } from '../hooks/useSignalR';
import { ExecutionMonitor } from './ExecutionMonitor';
import '../styles/components.css';

interface WorkflowExecutorProps {
  workflow: WorkflowDefinition;
  onClose: () => void;
}

export const WorkflowExecutor = ({ workflow, onClose }: WorkflowExecutorProps) => {
  const [isExecuting, setIsExecuting] = useState(false);
  const [execution, setExecution] = useState<ExecutionDto | null>(null);
  const [error, setError] = useState<string | null>(null);
  const { isConnected } = useSignalR(true);

  const handleExecute = async () => {
    try {
      setIsExecuting(true);
      setError(null);

      const result = await apiService.startExecution({
        workflowId: workflow.id,
        input: {}
      });

      const executionData = await apiService.getExecution(result.executionId);
      setExecution(executionData);
    } catch (err) {
      const message = err instanceof Error ? err.message : 'Failed to start execution';
      setError(message);
    } finally {
      setIsExecuting(false);
    }
  };

  const handleCancel = async () => {
    if (execution) {
      try {
        await apiService.cancelExecution(execution.executionId);
      } catch (err) {
        console.error('Failed to cancel execution:', err);
      }
    }
  };

  return (
    <div className="executor-container">
      <div className="executor-header">
        <h2>{workflow.name}</h2>
        <button className="close-button" onClick={onClose}>×</button>
      </div>

      <div className="executor-info">
        <p>{workflow.description}</p>
        <p><strong>Tasks:</strong> {workflow.tasks.length}</p>
        <p><strong>SignalR Status:</strong> <span className={isConnected ? 'connected' : 'disconnected'}>
          {isConnected ? '● Connected' : '● Disconnected'}
        </span></p>
      </div>

      {error && (
        <div className="error-message">{error}</div>
      )}

      {!execution ? (
        <div className="executor-controls">
          <button
            className="execute-button"
            onClick={handleExecute}
            disabled={isExecuting || !isConnected}
          >
            {isExecuting ? 'Starting...' : 'Start Execution'}
          </button>
        </div>
      ) : (
        <ExecutionMonitor
          execution={execution}
          onCancel={handleCancel}
          isCompleted={
            execution.status === ExecutionStatus.Completed ||
            execution.status === ExecutionStatus.Failed ||
            execution.status === ExecutionStatus.Cancelled
          }
        />
      )}
    </div>
  );
};
