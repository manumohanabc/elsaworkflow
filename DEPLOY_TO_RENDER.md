# Deploy ElsaWorkflow to Render

Complete guide to deploy the backend and frontend to Render.com

## Prerequisites

- Render account (free tier available at https://render.com)
- GitHub account with repository containing the code
- Git configured on your machine

## Step 1: Prepare Your Repository

### 1.1 Create a GitHub Repository

```bash
cd /Users/manumohan/elsaworkflow
git init
git add .
git commit -m "Initial commit: ElsaWorkflow application"
git remote add origin https://github.com/YOUR_USERNAME/elsaworkflow.git
git branch -M main
git push -u origin main
```

### 1.2 Add Required Files

The following files should be in your repository root:
- ✅ `.gitignore` (already created)
- ✅ `backend/` directory
- ✅ `frontend/` directory
- ✅ `.env.example` (we'll create this)

---

## Step 2: Configure Backend for Render

### 2.1 Create `backend/render.yaml`

```yaml
services:
  - type: web
    name: elsaworkflow-api
    env: dotnet
    buildCommand: cd backend/ElsaWorkflow.Api && dotnet publish -c Release -o /opt/render/project/src/out
    startCommand: /opt/render/project/src/out/ElsaWorkflow.Api
    envVars:
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
      - key: ASPNETCORE_URLS
        value: http://0.0.0.0:10000
```

### 2.2 Update `backend/ElsaWorkflow.Api/Program.cs`

Change the hardcoded port to use environment variable:

```csharp
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var aspnetcoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? $"http://localhost:{port}";

await app.RunAsync(aspnetcoreUrls);
```

### 2.3 Add `backend/.dockerignore`

```
bin/
obj/
.vs/
.vscode/
*.user
.DS_Store
node_modules
```

---

## Step 3: Configure Frontend for Render

### 3.1 Update `frontend/.env.production`

Create this file:

```env
VITE_API_URL=https://elsaworkflow-api.onrender.com/api
VITE_SIGNALR_URL=https://elsaworkflow-api.onrender.com
```

### 3.2 Update `frontend/vite.config.ts`

Add environment-based configuration:

```typescript
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
      '/ws': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        ws: true,
        rewrite: (path) => path.replace(/^\/ws/, ''),
      }
    }
  }
})
```

### 3.3 Add `frontend/.renderignore`

```
node_modules
.git
.gitignore
.env.local
.env.*.local
dist
build
.DS_Store
```

---

## Step 4: Deploy on Render

### 4.1 Deploy Backend

1. Go to https://dashboard.render.com
2. Click **"New +"** → **"Web Service"**
3. Connect your GitHub repository
4. Configure:
   - **Name:** `elsaworkflow-api`
   - **Environment:** `Dotnet`
   - **Build Command:** `cd backend/ElsaWorkflow.Api && dotnet publish -c Release`
   - **Start Command:** `./bin/Release/net10.0/ElsaWorkflow.Api`
   - **Instance Type:** `Free` (or Starter for production)

5. Add Environment Variables:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://0.0.0.0:10000
   JWT_SECRET_KEY=your-production-secret-key-min-32-chars
   ```

6. Click **"Create Web Service"**
7. Wait for deployment (~10 minutes)
8. Copy the URL (e.g., `https://elsaworkflow-api.onrender.com`)

### 4.2 Deploy Frontend

1. Go to https://dashboard.render.com
2. Click **"New +"** → **"Static Site"**
3. Connect your GitHub repository
4. Configure:
   - **Name:** `elsaworkflow-ui`
   - **Environment:** `Node`
   - **Build Command:** `cd frontend && npm install && npm run build`
   - **Publish Directory:** `frontend/dist`

5. Add Environment Variables:
   ```
   VITE_API_URL=https://elsaworkflow-api.onrender.com/api
   VITE_SIGNALR_URL=https://elsaworkflow-api.onrender.com
   ```

6. Click **"Create Static Site"**
7. Wait for deployment (~5 minutes)
8. Your frontend will be at `https://elsaworkflow-ui.onrender.com`

---

## Step 5: Configure CORS on Backend

Update `Program.cs` to include Render domain:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins(
                "http://localhost:5173",           // Local dev
                "http://localhost:3000",           // Local dev alt
                "https://elsaworkflow-ui.onrender.com"  // Production
            );
    });
});
```

---

## Step 6: Update Frontend API URLs

Update `frontend/src/services/api.ts`:

```typescript
const API_URL = import.meta.env.VITE_API_URL || '/api';
```

Update `frontend/src/hooks/useSignalR.ts`:

```typescript
const hubUrl = `${import.meta.env.VITE_SIGNALR_URL}/ws/workflowHub`;
```

---

## Step 7: Redeploy After Changes

### Push Changes to GitHub

```bash
git add .
git commit -m "Configure deployment settings"
git push origin main
```

### Redeploy on Render

1. Go to Render Dashboard
2. Select the service
3. Go to **"Deployments"**
4. Click **"Deploy latest commit"**

Or enable **Auto-Deploy**:
- Service Settings → Auto-Deploy: `Yes`
- Redeploys automatically on git push

---

## Step 8: Test Deployment

1. Open https://elsaworkflow-ui.onrender.com
2. Login with credentials:
   - Username: `demo`
   - Password: `demo123`
3. Test workflow execution
4. Verify real-time updates via WebSocket

---

## Troubleshooting Deployment

### Backend Build Fails

**Problem:** `.NET SDK not found`

**Solution:**
- Ensure `ElsaWorkflow.Api.csproj` has correct target framework
- Check SDK version matches Render's .NET support

### Frontend Build Fails

**Problem:** `npm install` times out

**Solution:**
- Increase build timeout in Render settings
- Clear npm cache: `npm cache clean --force`

### CORS Errors

**Problem:** WebSocket connection blocked

**Solution:**
1. Verify CORS configuration includes Render domain
2. Check that `/ws` path is included in proxy config
3. Ensure `ws: true` is set in proxy configuration

### WebSocket Connection Fails

**Problem:** "Failed to connect to SignalR hub"

**Solution:**
1. Verify backend is running (check Render logs)
2. Confirm WebSocket URL is correct
3. Check that token is being passed correctly
4. Ensure Auth middleware is not blocking hub

---

## Monitoring & Logs

### View Backend Logs

1. Go to Render Dashboard
2. Select `elsaworkflow-api`
3. Click **"Logs"** tab
4. Search for errors

### View Frontend Logs

1. Go to Render Dashboard
2. Select `elsaworkflow-ui`
3. Click **"Logs"** tab

### Check Service Status

- Backend: `https://elsaworkflow-api.onrender.com/api/auth/login`
- Frontend: `https://elsaworkflow-ui.onrender.com`

---

## Environment Variables Checklist

### Backend (.env)
- [ ] `ASPNETCORE_ENVIRONMENT=Production`
- [ ] `ASPNETCORE_URLS=http://0.0.0.0:10000`
- [ ] `JWT_SECRET_KEY` (change from default)
- [ ] `CORS_ALLOWED_ORIGINS` (include Render domain)

### Frontend (.env.production)
- [ ] `VITE_API_URL=https://elsaworkflow-api.onrender.com/api`
- [ ] `VITE_SIGNALR_URL=https://elsaworkflow-api.onrender.com`

---

## Custom Domain (Optional)

### Add Custom Domain to Frontend

1. Render Dashboard → `elsaworkflow-ui`
2. Settings → **Custom Domain**
3. Add your domain (e.g., `app.example.com`)
4. Update DNS records as instructed

### Add Custom Domain to Backend

1. Render Dashboard → `elsaworkflow-api`
2. Settings → **Custom Domain**
3. Add your domain (e.g., `api.example.com`)

---

## Cost Considerations

| Component | Free Tier | Cost |
|-----------|-----------|------|
| Static Site (Frontend) | Yes | $0/month |
| Web Service (Backend) | Yes* | $7+/month |
| Database (if added) | No | $7+/month |

*Free tier: Auto-sleep after 15 min inactivity

### For Always-On Service

- **Upgrade to Starter Plan:** $7/month
- **Load balancing:** $5/month additional
- **PostgreSQL:** $7-15/month

---

## Production Checklist

- [ ] Change JWT secret key
- [ ] Update CORS allowed origins
- [ ] Enable HTTPS (automatic on Render)
- [ ] Set up proper logging
- [ ] Configure database (if needed)
- [ ] Add monitoring/alerts
- [ ] Test all workflows
- [ ] Verify WebSocket connections
- [ ] Check file uploads work (if applicable)
- [ ] Review security settings

---

## Performance Tips

1. **Enable Caching**
   - Add Cache-Control headers for static files
   - Implement API caching

2. **Optimize Images**
   - Compress frontend assets
   - Use WebP format

3. **Database Optimization**
   - Add indexes on frequently queried columns
   - Use connection pooling

4. **Monitoring**
   - Set up alerts for errors
   - Monitor response times
   - Track WebSocket connections

---

## Support & Resources

- Render Docs: https://render.com/docs
- .NET on Render: https://render.com/docs/deploy-dotnet
- Node.js on Render: https://render.com/docs/deploy-node
- Troubleshooting: https://render.com/docs/common-issues

---

## Next Steps

1. Push code to GitHub
2. Deploy backend first
3. Deploy frontend
4. Test all features
5. Set up monitoring
6. Configure auto-deploy
7. Add custom domain (optional)

Good luck with your deployment! 🚀
