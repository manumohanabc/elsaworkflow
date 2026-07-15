import axios, { AxiosInstance } from 'axios';

const API_URL = '/api';

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  success: boolean;
  message: string;
  token: string;
  username: string;
}

export interface WorkflowDefinition {
  id: string;
  name: string;
  description: string;
  tasks: TaskDefinition[];
}

export interface TaskDefinition {
  id: string;
  name: string;
  description: string;
  durationMs: number;
  dependencies: string[];
}

export interface ExecutionRequest {
  workflowId: string;
  input?: Record<string, unknown>;
}

export interface TaskExecutionDto {
  taskId: string;
  taskName: string;
  status: TaskExecutionStatus;
  startedAt?: string;
  completedAt?: string;
  output?: Record<string, unknown>;
  error?: string;
}

export interface ExecutionDto {
  executionId: string;
  workflowId: string;
  workflowName: string;
  status: ExecutionStatus;
  startedAt: string;
  completedAt?: string;
  tasks: TaskExecutionDto[];
  error?: string;
  output?: Record<string, unknown>;
}

export enum ExecutionStatus {
  Pending = 'Pending',
  Running = 'Running',
  Completed = 'Completed',
  Failed = 'Failed',
  Cancelled = 'Cancelled'
}

export enum TaskExecutionStatus {
  Pending = 'Pending',
  Running = 'Running',
  Completed = 'Completed',
  Failed = 'Failed',
  Skipped = 'Skipped'
}

class ApiService {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: API_URL,
    });

    this.axiosInstance.interceptors.request.use((config) => {
      const token = localStorage.getItem('authToken');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });
  }

  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await this.axiosInstance.post<LoginResponse>('/auth/login', credentials);
    if (response.data.success) {
      localStorage.setItem('authToken', response.data.token);
      localStorage.setItem('username', response.data.username);
    }
    return response.data;
  }

  async getWorkflows(): Promise<WorkflowDefinition[]> {
    const response = await this.axiosInstance.get<{ workflows: WorkflowDefinition[] }>('/workflow');
    return response.data.workflows;
  }

  async getWorkflow(id: string): Promise<WorkflowDefinition> {
    const response = await this.axiosInstance.get<WorkflowDefinition>(`/workflow/${id}`);
    return response.data;
  }

  async startExecution(request: ExecutionRequest): Promise<{ executionId: string; status: string }> {
    const response = await this.axiosInstance.post<{ executionId: string; status: string }>('/execution/start', request);
    return response.data;
  }

  async getExecution(executionId: string): Promise<ExecutionDto> {
    const response = await this.axiosInstance.get<ExecutionDto>(`/execution/${executionId}`);
    return response.data;
  }

  async getRecentExecutions(limit: number = 50): Promise<{ executions: ExecutionDto[]; totalCount: number }> {
    const response = await this.axiosInstance.get<{ executions: ExecutionDto[]; totalCount: number }>('/execution', {
      params: { limit }
    });
    return response.data;
  }

  async cancelExecution(executionId: string): Promise<{ message: string }> {
    const response = await this.axiosInstance.post<{ message: string }>(`/execution/${executionId}/cancel`);
    return response.data;
  }
}

export default new ApiService();
