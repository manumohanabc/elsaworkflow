# ElsaWorkflow Showcase Application

A full-stack demonstration of ElsaWorkflow with real-time execution monitoring using React, TypeScript, .NET Core 8, and SignalR.

## 🎯 Features

- **Real-time Workflow Execution**: Monitor workflow execution progress in real-time via SignalR
- **Multiple Workflow Templates**: Three sample workflows (Sequential, Parallel, Long-running)
- **Task Progress Tracking**: View individual task execution with status, duration, and output
- **Authentication**: Simple JWT-based login system
- **In-memory State**: Fast workflow execution without database dependencies

## 🏗️ Architecture

### Backend (.NET Core 8)
- **Framework**: ASP.NET Core 8
- **Workflow Engine**: Elsa Workflows
- **Real-time Communication**: SignalR
- **Authentication**: JWT Bearer tokens
- **Location**: `/backend/ElsaWorkflow.Api`

### Frontend (React + TypeScript)
- **Framework**: React 18 with TypeScript
- **Build Tool**: Vite
- **Real-time Client**: @microsoft/signalr
- **HTTP Client**: Axios
- **Location**: `/frontend`

## 📋 Prerequisites

- .NET 8 SDK or later
- Node.js 18+ and npm
- PowerShell or Bash terminal

## 🚀 Setup and Run

### Backend Setup

1. Navigate to backend directory:
```bash
cd backend
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Run the backend:
```bash
dotnet run --project ElsaWorkflow.Api
```

The backend will start on `http://localhost:5000` (or `http://localhost:5001` for HTTPS)

### Frontend Setup

1. Navigate to frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The frontend will start on `http://localhost:5173`

### Building for Production

**Frontend:**
```bash
cd frontend
npm run build
```

**Backend:**
```bash
cd backend
dotnet publish -c Release
```

## 🔐 Authentication

### Default Credentials
- **Username**: `demo` | **Password**: `demo123`
- **Username**: `admin` | **Password**: `admin123`
- **Username**: `user` | **Password**: `user123`

Login credentials are hardcoded in the demo. For production, implement proper authentication.

## 📚 API Endpoints

### Authentication
- `POST /api/auth/login` - Login and get JWT token

### Workflows
- `GET /api/workflow` - List all available workflows
- `GET /api/workflow/{id}` - Get specific workflow details

### Execution
- `POST /api/execution/start` - Start workflow execution
- `GET /api/execution/{executionId}` - Get execution status
- `GET /api/execution` - Get recent executions
- `POST /api/execution/{executionId}/cancel` - Cancel execution

### SignalR Hub
- **Endpoint**: `/workflowHub` (requires authentication via JWT)
- **Events**:
  - `WorkflowStarted` - Workflow execution started
  - `TaskStarted` - Task execution started
  - `TaskCompleted` - Task completed successfully
  - `TaskFailed` - Task execution failed
  - `WorkflowCompleted` - Workflow execution completed
  - `ExecutionFailed` - Workflow execution failed
  - `ExecutionCancelled` - Workflow execution cancelled

## 🔄 Workflow Examples

### 1. Simple Task Workflow
Sequential execution of 4 tasks:
- Initialize (500ms)
- Process Data (1500ms)
- Generate Report (1000ms)
- Finalize (500ms)

### 2. Parallel Task Workflow
Initial task, followed by 3 parallel tasks, then aggregation:
- Start (300ms)
- Task A, B, C in parallel (1200ms each)
- Aggregate Results (800ms)

### 3. Long Running Workflow
Multi-stage processing workflow:
- Validate Input (800ms)
- Process Stage 1 (2000ms)
- Process Stage 2 (2500ms)
- Process Stage 3 (2000ms)
- Store Results (1000ms)

## 🧪 Testing

1. Open browser to `http://localhost:5173`
2. Login with demo credentials
3. Select a workflow from the list
4. Click "Execute Workflow"
5. Watch real-time progress updates via SignalR
6. View task details, output, and execution timeline

## 🔧 Configuration

### Backend Configuration (`appsettings.json`)
```json
{
  "Jwt": {
    "SecretKey": "your-super-secret-key-change-in-production-min-32-chars",
    "Issuer": "ElsaWorkflowApp",
    "Audience": "ElsaWorkflowUsers"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173", "http://localhost:3000"]
  }
}
```

### Frontend Configuration
Update API base URL in `src/services/api.ts` if backend runs on different port:
```typescript
const API_URL = '/api'; // Uses proxy from vite.config.ts
```

## 📊 Project Structure

```
elsaworkflow/
├── backend/
│   ├── ElsaWorkflow.Api/
│   │   ├── Controllers/
│   │   ├── Hubs/
│   │   ├── Services/
│   │   ├── Models/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   └── ElsaWorkflow.sln
├── frontend/
│   ├── src/
│   │   ├── components/
│   │   ├── hooks/
│   │   ├── services/
│   │   ├── styles/
│   │   ├── App.tsx
│   │   └── main.tsx
│   ├── package.json
│   ├── tsconfig.json
│   └── vite.config.ts
└── README.md
```

## 🐛 Troubleshooting

### SignalR Connection Issues
- Ensure backend is running on correct port
- Check CORS configuration includes frontend origin
- Verify JWT token is valid

### Workflow Not Appearing
- Check backend API response in browser DevTools
- Verify authentication token is present
- Check backend logs for errors

### Frontend Build Issues
- Delete `node_modules` and run `npm install` again
- Clear `.vite` cache
- Check Node.js version compatibility

## 📝 License

This is a demonstration project. Feel free to use as a starting point for your ElsaWorkflow applications.

## 🤝 Contributing

This is a showcase project. For production use, consider:
- Adding proper error handling
- Implementing database persistence
- Adding comprehensive logging
- Securing sensitive configuration
- Adding unit and integration tests
- Implementing rate limiting
- Adding API documentation with Swagger
