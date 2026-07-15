# ElsaWorkflow Application - Setup Guide

This guide provides step-by-step instructions for setting up and running the ElsaWorkflow application.

## Prerequisites

### System Requirements
- **Windows, macOS, or Linux**
- **RAM**: At least 4GB (8GB recommended)
- **Disk Space**: At least 2GB

### Required Software

#### 1. .NET 8 SDK or later
Download from: https://dotnet.microsoft.com/download

**Verify Installation:**
```bash
dotnet --version
```

Should show version 8.0.0 or higher.

#### 2. Node.js 18+ and npm
Download from: https://nodejs.org/

**Verify Installation:**
```bash
node --version
npm --version
```

Should show v18.0.0+ for Node and 9.0.0+ for npm.

## Installation Steps

### Option 1: Using Setup Scripts (Recommended)

#### On macOS/Linux:
```bash
cd /Users/manumohan/elsaworkflow

# Make scripts executable
chmod +x setup-backend.sh setup-frontend.sh

# Setup backend
./setup-backend.sh

# In a new terminal, setup frontend
./setup-frontend.sh
```

#### On Windows (PowerShell):
```powershell
cd C:\Users\manumohan\elsaworkflow

# Setup backend
.\setup-backend.ps1

# In a new terminal, setup frontend
.\setup-frontend.ps1
```

### Option 2: Manual Setup

#### Backend Setup:
```bash
cd backend

# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Run the backend
dotnet run --project ElsaWorkflow.Api
```

#### Frontend Setup (in a new terminal):
```bash
cd frontend

# Install npm dependencies
npm install

# Start development server
npm run dev
```

## Running the Application

### Starting Backend
```bash
cd backend
dotnet run --project ElsaWorkflow.Api
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Starting Frontend (new terminal)
```bash
cd frontend
npm run dev
```

**Expected Output:**
```
  VITE v5.0.8  ready in 234 ms

  ➜  Local:   http://localhost:5173/
  ➜  press h to show help
```

### Access the Application
1. Open your browser
2. Go to: **http://localhost:5173**
3. Login with demo credentials:
   - Username: `demo`
   - Password: `demo123`

## Troubleshooting

### 1. NuGet Restore Fails
**Problem**: "Unable to load the service index for source..."

**Solutions**:
- Check internet connection
- Clear NuGet cache:
  ```bash
  dotnet nuget locals all --clear
  ```
- Try again with `dotnet restore`
- If network is restricted, configure NuGet sources:
  ```bash
  dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
  ```

### 2. Port Already in Use
**Problem**: "Address already in use" error

**Solution**: Change port in `backend/appsettings.json`:
```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5002"
      }
    }
  }
}
```

### 3. Node Modules Issues
**Problem**: npm dependencies won't install

**Solution**:
```bash
cd frontend
rm -rf node_modules package-lock.json
npm install
```

### 4. SignalR Connection Issues
**Problem**: Real-time updates not working

**Solutions**:
- Verify backend is running on correct port
- Check browser console for errors
- Ensure JWT token is valid
- Check CORS configuration in `Program.cs`

### 5. Frontend Won't Start
**Problem**: Vite proxy errors

**Solution**: Update `vite.config.ts`:
```typescript
server: {
  port: 5173,
  proxy: {
    '/api': {
      target: 'http://localhost:5000',  // Match your backend port
      changeOrigin: true,
    }
  }
}
```

## Project Verification

### Backend Health Check
```bash
# After backend is running, in new terminal:
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","password":"demo123"}'
```

**Expected Response**:
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJhbGc...",
  "username": "demo"
}
```

### Frontend Health Check
- Browser DevTools Console: Should have no CORS errors
- Network tab: API calls should return 200 status
- SignalR: Should show "Connected" status in ExecutionMonitor

## Configuration

### Backend Configuration
Edit `backend/appsettings.json`:
- **JWT Secret**: Change for production
- **CORS Origins**: Add your frontend URL
- **Logging**: Adjust log level

### Frontend Configuration
Edit `frontend/.env`:
```env
VITE_API_URL=http://localhost:5000
VITE_SIGNALR_URL=http://localhost:5000/workflowHub
```

## Building for Production

### Frontend Build
```bash
cd frontend
npm run build

# Output will be in frontend/dist/
```

### Backend Build
```bash
cd backend
dotnet publish -c Release -o ./publish

# Output will be in backend/publish/
```

## Next Steps

After successful setup:

1. **Explore Workflows**:
   - Simple Task Workflow: Sequential execution
   - Parallel Task Workflow: Concurrent execution
   - Long Running Workflow: Multi-stage processing

2. **Monitor Execution**:
   - Watch real-time updates
   - View task details and output
   - Check execution timeline

3. **Customize**:
   - Add new workflows in `WorkflowService.cs`
   - Modify task durations for testing
   - Add authentication with your system
   - Persist execution history to a database

## Performance Tips

- **Backend**: Consider adding caching for workflow definitions
- **Frontend**: Use React.memo for heavy components
- **SignalR**: Monitor active connections and scalability

## Getting Help

### Common Commands

**Backend**:
```bash
dotnet --info                    # .NET info
dotnet clean                      # Clean build artifacts
dotnet build --verbose            # Verbose build output
```

**Frontend**:
```bash
npm list                         # List dependencies
npm outdated                     # Check for updates
npm audit                        # Security audit
```

### Useful URLs
- Swagger API Docs: http://localhost:5000/swagger
- Workflow Hub: ws://localhost:5000/workflowHub
- Frontend App: http://localhost:5173

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review console logs (Browser DevTools, Terminal)
3. Verify all prerequisites are installed
4. Check firewall/network settings
