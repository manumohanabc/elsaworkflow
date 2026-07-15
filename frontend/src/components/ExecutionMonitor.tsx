import { useState, useEffect } from 'react';
import { ExecutionDto, TaskExecutionDto, ExecutionStatus, TaskExecutionStatus } from '../services/api';
import { useSignalR } from '../hooks/useSignalR';
import '../styles/components.css';

interface ExecutionMonitorProps {
  execution: ExecutionDto;
  onCancel: () => void;
  isCompleted: boolean;
}

export const ExecutionMonitor = ({ execution: initialExecution, onCancel, isCompleted }: ExecutionMonitorProps) => {
  const [execution, setExecution] = useState(initialExecution);
  const { subscribe } = useSignalR(true);

  useEffect(() => {
    const unsubscribeTaskStarted = subscribe('TaskStarted', (event) => {
      if (event.executionId === execution.executionId) {
        setExecution(prev => ({
          ...prev,
          tasks: prev.tasks.map(t =>
            t.taskId === event.taskId
              ? { ...t, status: TaskExecutionStatus.Running, startedAt: new Date().toISOString() }
              : t
          )
        }));
      }
    });

    const unsubscribeTaskCompleted = subscribe('TaskCompleted', (event) => {
      if (event.executionId === execution.executionId) {
        setExecution(prev => ({
          ...prev,
          tasks: prev.tasks.map(t =>
            t.taskId === event.taskId
              ? {
                  ...t,
                  status: TaskExecutionStatus.Completed,
                  completedAt: new Date().toISOString(),
                  output: event.output
                }
              : t
          )
        }));
      }
    });

    const unsubscribeTaskFailed = subscribe('TaskFailed', (event) => {
      if (event.executionId === execution.executionId) {
        setExecution(prev => ({
          ...prev,
          tasks: prev.tasks.map(t =>
            t.taskId === event.taskId
              ? {
                  ...t,
                  status: TaskExecutionStatus.Failed,
                  completedAt: new Date().toISOString(),
                  error: event.error
                }
              : t
          )
        }));
      }
    });

    const unsubscribeWorkflowCompleted = subscribe('WorkflowCompleted', (event) => {
      if (event.executionId === execution.executionId) {
        setExecution(prev => ({
          ...prev,
          status: ExecutionStatus.Completed,
          completedAt: new Date().toISOString(),
          output: event.output
        }));
      }
    });

    const unsubscribeExecutionFailed = subscribe('ExecutionFailed', (event) => {
      if (event.executionId === execution.executionId) {
        setExecution(prev => ({
          ...prev,
          status: ExecutionStatus.Failed,
          completedAt: new Date().toISOString(),
          error: event.error
        }));
      }
    });

    return () => {
      unsubscribeTaskStarted();
      unsubscribeTaskCompleted();
      unsubscribeTaskFailed();
      unsubscribeWorkflowCompleted();
      unsubscribeExecutionFailed();
    };
  }, [execution.executionId, subscribe]);

  const getStatusColor = (status: ExecutionStatus | TaskExecutionStatus) => {
    switch (status) {
      case ExecutionStatus.Running:
      case TaskExecutionStatus.Running:
        return '#4CAF50';
      case ExecutionStatus.Completed:
      case TaskExecutionStatus.Completed:
        return '#2196F3';
      case ExecutionStatus.Failed:
      case TaskExecutionStatus.Failed:
        return '#f44336';
      case TaskExecutionStatus.Pending:
      case ExecutionStatus.Pending:
        return '#FF9800';
      default:
        return '#757575';
    }
  };

  const getDuration = (task: TaskExecutionDto) => {
    if (!task.startedAt || !task.completedAt) return '-';
    const start = new Date(task.startedAt).getTime();
    const end = new Date(task.completedAt).getTime();
    return `${end - start}ms`;
  };

  return (
    <div className="monitor-container">
      <div className="execution-header">
        <h3>Execution Progress</h3>
        <div className="execution-meta">
          <p><strong>ID:</strong> {execution.executionId.substring(0, 8)}...</p>
          <p><strong>Status:</strong> <span style={{ color: getStatusColor(execution.status) }}>
            ● {execution.status}
          </span></p>
          {execution.completedAt && (
            <p><strong>Duration:</strong> {
              new Date(execution.completedAt).getTime() - new Date(execution.startedAt).getTime()
            }ms</p>
          )}
        </div>
      </div>

      <div className="tasks-container">
        <h4>Task Execution</h4>
        <div className="tasks-list">
          {execution.tasks.map((task) => (
            <div key={task.taskId} className="task-item">
              <div className="task-header">
                <span className="task-status" style={{ backgroundColor: getStatusColor(task.status) }}>
                  {task.status}
                </span>
                <h5>{task.taskName}</h5>
                <span className="task-duration">{getDuration(task)}</span>
              </div>
              {task.output && (
                <div className="task-output">
                  <small>{JSON.stringify(task.output, null, 2)}</small>
                </div>
              )}
              {task.error && (
                <div className="task-error">
                  <small>Error: {task.error}</small>
                </div>
              )}
            </div>
          ))}
        </div>
      </div>

      {execution.output && (
        <div className="execution-output">
          <h4>Output</h4>
          <pre>{JSON.stringify(execution.output, null, 2)}</pre>
        </div>
      )}

      {execution.error && (
        <div className="error-message">
          Error: {execution.error}
        </div>
      )}

      <div className="monitor-actions">
        {!isCompleted && (
          <button className="cancel-button" onClick={onCancel}>
            Cancel Execution
          </button>
        )}
      </div>
    </div>
  );
};
