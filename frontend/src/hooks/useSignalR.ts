import { useEffect, useRef, useCallback, useState } from 'react';
import * as signalR from '@microsoft/signalr';

export interface WorkflowEvent {
  executionId: string;
  workflowName?: string;
  taskId?: string;
  taskName?: string;
  output?: Record<string, unknown>;
  error?: string;
  status?: string;
}

export const useSignalR = (enabled: boolean = true) => {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const [connectionError, setConnectionError] = useState<string | null>(null);
  const listenersRef = useRef<Map<string, Set<(event: WorkflowEvent) => void>>>(new Map());

  const connect = useCallback(async () => {
    if (!enabled) return;

    try {
      const token = localStorage.getItem('authToken');
      if (!token) {
        setConnectionError('No authentication token found');
        return;
      }

      const hubUrl = `/ws/workflowHub`;

      const connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
          // skipNegotiation: true,
          accessTokenFactory: () => token,
          transport: signalR.HttpTransportType.WebSockets,
        })
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .withHubProtocol(new signalR.JsonHubProtocol())
        .configureLogging(signalR.LogLevel.Information)
        .build();

      connection.on('Connected', (data) => {
        console.log('Connected to hub:', data);
        setIsConnected(true);
        setConnectionError(null);
      });

      connection.on('WorkflowStarted', (event: WorkflowEvent) => {
        notifyListeners('WorkflowStarted', event);
      });

      connection.on('TaskStarted', (event: WorkflowEvent) => {
        notifyListeners('TaskStarted', event);
      });

      connection.on('TaskCompleted', (event: WorkflowEvent) => {
        notifyListeners('TaskCompleted', event);
      });

      connection.on('TaskFailed', (event: WorkflowEvent) => {
        notifyListeners('TaskFailed', event);
      });

      connection.on('WorkflowCompleted', (event: WorkflowEvent) => {
        notifyListeners('WorkflowCompleted', event);
      });

      connection.on('ExecutionFailed', (event: WorkflowEvent) => {
        notifyListeners('ExecutionFailed', event);
      });

      connection.on('ExecutionCancelled', (event: WorkflowEvent) => {
        notifyListeners('ExecutionCancelled', event);
      });

      connection.onreconnecting(() => {
        setIsConnected(false);
        setConnectionError('Reconnecting...');
      });

      connection.onreconnected(() => {
        setIsConnected(true);
        setConnectionError(null);
      });

      await connection.start();
      connectionRef.current = connection;
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Connection failed';
      setConnectionError(message);
      console.error('SignalR connection error:', error);
    }
  }, [enabled]);

  const disconnect = useCallback(async () => {
    if (connectionRef.current) {
      try {
        await connectionRef.current.stop();
        setIsConnected(false);
      } catch (error) {
        console.error('Error disconnecting:', error);
      }
    }
  }, []);

  const notifyListeners = (eventType: string, event: WorkflowEvent) => {
    const listeners = listenersRef.current.get(eventType);
    if (listeners) {
      listeners.forEach(listener => listener(event));
    }
  };

  const subscribe = useCallback((eventType: string, listener: (event: WorkflowEvent) => void) => {
    if (!listenersRef.current.has(eventType)) {
      listenersRef.current.set(eventType, new Set());
    }
    listenersRef.current.get(eventType)!.add(listener);

    return () => {
      const listeners = listenersRef.current.get(eventType);
      if (listeners) {
        listeners.delete(listener);
      }
    };
  }, []);

  const joinExecutionGroup = useCallback(async (executionId: string) => {
    if (connectionRef.current && isConnected) {
      try {
        await connectionRef.current.invoke('JoinExecutionGroup', executionId);
      } catch (error) {
        console.error('Error joining group:', error);
      }
    }
  }, [isConnected]);

  useEffect(() => {
    if (enabled) {
      connect();
    }

    return () => {
      disconnect();
    };
  }, [enabled, connect, disconnect]);

  return {
    isConnected,
    connectionError,
    subscribe,
    joinExecutionGroup,
  };
};
