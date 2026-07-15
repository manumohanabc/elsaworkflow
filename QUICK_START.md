# Quick Start Guide - ElsaWorkflow Backend

## Prerequisites
- .NET 10 SDK (or .NET 8+ if you prefer to use net8.0)
- Node.js 18+ and npm

## Step 1: Run the Backend

Open a **terminal** and run:

```bash
cd /Users/manumohan/elsaworkflow/backend/ElsaWorkflow.Api
dotnet run
```

**Expected Output:**
```
Building...
...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

✅ **Backend is now running on `http://localhost:5000`**

---

## Step 2: Test the Backend (Optional)

Open a **new terminal** and test the login endpoint:

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","password":"demo123"}'
```

**Expected Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "token": "eyJ...",
  "username": "demo"
}
```

---

## Step 3: Run the Frontend

Open a **third terminal** and run:

```bash
cd /Users/manumohan/elsaworkflow/frontend
npm install
npm run dev
```

**Expected Output:**
```
  VITE v5.0.8  ready in 234 ms

  ➜  Local:   http://localhost:5173/
  ➜  press h to show help
```

✅ **Frontend is now running on `http://localhost:5173`**

---

## Step 4: Open in Browser

Open your browser and go to:
```
http://localhost:5173
```

**Login with:**
- Username: `demo`
- Password: `demo123`

---

## Troubleshooting

### Backend won't start

**Problem**: "Address already in use" or port 5000 is taken

**Solution**: 
- Kill any process using port 5000
- Or change the port in `Program.cs`

**On macOS/Linux:**
```bash
lsof -i :5000
kill -9 <PID>
```

### Frontend won't start

**Problem**: "npm: command not found"

**Solution**: Install Node.js from https://nodejs.org/

### Login fails

**Problem**: "Login failed" or network error

**Solution**:
1. Make sure backend is running on `http://localhost:5000`
2. Check browser console for CORS errors
3. Verify both services are running

### SignalR connection issues

**Problem**: Real-time updates not working

**Solution**:
1. Ensure both services are running
2. Open browser DevTools (F12) and check Console tab
3. Verify WebSocket connections in Network tab

---

## Available Demo Credentials

| Username | Password   |
|----------|-----------|
| demo     | demo123   |
| admin    | admin123  |
| user     | user123   |

---

## Keep Both Running

Important: **Keep both the backend and frontend running** in separate terminals while you use the app.

Terminal 1: `dotnet run` (backend - never stop)
Terminal 2: `npm run dev` (frontend - never stop)
Terminal 3: Use for testing/other commands

---

## Next Steps

1. **Explore Workflows**
   - Click on any workflow card
   - Click "Execute Workflow"
   - Watch real-time progress

2. **Monitor Execution**
   - Watch tasks execute in real-time
   - See task details and output
   - Check execution timeline

3. **Test Cancellation**
   - Click "Cancel Execution" during a running workflow

---

## Stop the Application

**To stop backend:** Press `Ctrl+C` in terminal 1
**To stop frontend:** Press `Ctrl+C` in terminal 2

---

## Production Build

When ready for production:

**Frontend:**
```bash
cd frontend
npm run build
# Output in: frontend/dist/
```

**Backend:**
```bash
cd backend/ElsaWorkflow.Api
dotnet publish -c Release -o ./publish
# Output in: backend/ElsaWorkflow.Api/publish/
```

---

## Support

- See `README.md` for full documentation
- See `SETUP.md` for detailed setup instructions
- See `PROJECT_SUMMARY.md` for technical details

Enjoy your ElsaWorkflow application! 🚀
