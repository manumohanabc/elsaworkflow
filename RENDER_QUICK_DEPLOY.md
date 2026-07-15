# 🚀 ElsaWorkflow - Render Deployment (5 Minutes)

Fast track to deploy your app on Render.

## Quick Start

### 1️⃣ Push to GitHub

```bash
cd /Users/manumohan/elsaworkflow

# Initialize git (if not already done)
git init

# Add all files
git add .

# Commit
git commit -m "ElsaWorkflow ready for Render"

# Add GitHub remote
git remote add origin https://github.com/YOUR_USERNAME/elsaworkflow.git

# Push to main branch
git branch -M main
git push -u origin main
```

### 2️⃣ Deploy Backend on Render

1. Go to: https://dashboard.render.com
2. Click **"New +"** → **"Web Service"**
3. Connect GitHub repository
4. Configure:
   - **Name**: `elsaworkflow-api`
   - **Environment**: `Dotnet`
   - **Build Command**: `cd backend/ElsaWorkflow.Api && dotnet publish -c Release`
   - **Start Command**: `./out/ElsaWorkflow.Api`
   - **Instance Type**: `Free` (or Starter for production)
5. Click **"Create Web Service"**
6. ⏳ Wait ~10-15 minutes for deployment
7. 📝 Note the backend URL (e.g., `https://elsaworkflow-api.onrender.com`)

### 2B️⃣ Deploy Frontend on Render

1. Go to: https://dashboard.render.com
2. Click **"New +"** → **"Web Service"**
3. Connect same GitHub repository
4. Configure:
   - **Name**: `elsaworkflow-ui`
   - **Environment**: `Node`
   - **Build Command**: `cd frontend && npm install && npm run build`
   - **Start Command**: `npx serve -s dist -l 3000`
   - **Instance Type**: `Free`
5. Click **"Create Web Service"**
6. ⏳ Wait ~5-10 minutes for deployment
7. 📝 Note the frontend URL (e.g., `https://elsaworkflow-ui.onrender.com`)

### 3️⃣ Set Environment Variables

#### Backend (`elsaworkflow-api`)

After backend deployment completes:

1. Go to Render Dashboard → `elsaworkflow-api`
2. Click **"Environment"** tab
3. Add these variables:

| Key | Value |
|-----|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ASPNETCORE_URLS` | `http://0.0.0.0:10000` |
| `JWT_SECRET_KEY` | `generate-random-32-char-key` |
| `CORS_ALLOWED_ORIGINS` | `https://elsaworkflow-ui.onrender.com` |

4. Click **"Save"**
5. Service will auto-redeploy

#### Frontend (`elsaworkflow-ui`)

After frontend deployment completes:

1. Go to Render Dashboard → `elsaworkflow-ui`
2. Click **"Environment"** tab
3. Add these variables:

| Key | Value |
|-----|-------|
| `VITE_API_URL` | `https://elsaworkflow-api.onrender.com/api` |
| `VITE_SIGNALR_URL` | `https://elsaworkflow-api.onrender.com` |

4. Click **"Save"**
5. Service will auto-redeploy

> **Note**: Get the backend URL from the backend service page after it deploys.

### 4️⃣ Test

Open `https://elsaworkflow-ui.onrender.com`

Login:
- Username: `demo`
- Password: `demo123`

Execute a workflow! ✅

---

## Services URLs

After deployment, you'll have:

- **Frontend**: `https://elsaworkflow-ui.onrender.com`
- **Backend API**: `https://elsaworkflow-api.onrender.com`
- **Dashboard**: `https://dashboard.render.com`

---

## File Structure for Render

```
elsaworkflow/
├── render.yaml                    ← Deployment config
├── backend/
│   ├── Dockerfile                 ← Backend container
│   ├── .dockerignore
│   └── ElsaWorkflow.Api/
│       ├── Program.cs             ← Uses PORT env var
│       └── appsettings.json
├── frontend/
│   ├── Dockerfile                 ← Frontend container
│   ├── .env.production            ← Production URLs
│   ├── vite.config.ts             ← Uses proxy
│   └── package.json
└── .gitignore
```

---

## Environment Variables Reference

| Variable | Backend | Frontend | Value |
|----------|---------|----------|-------|
| Environment | ✅ | ✅ | `Production` |
| Port | ✅ | ✅ | `10000` / `3000` |
| API URL | | ✅ | `https://elsaworkflow-api.onrender.com` |
| SignalR URL | | ✅ | `https://elsaworkflow-api.onrender.com` |
| JWT Secret | ✅ | | Random string (32+ chars) |
| CORS Origins | ✅ | | Frontend URL |

---

## Troubleshooting

### "Build Failed"
- Check build logs on Render dashboard
- Verify project structure is correct
- Ensure all required files are present

### "Connection Refused"
- Backend might still be starting
- Refresh page after 2 minutes
- Check service logs for errors

### "Login Fails"
- Verify backend is running (check URL directly)
- Check environment variables are set
- Review CORS configuration

### "Real-time Updates Not Working"
- Check WebSocket URL in browser console
- Verify backend `/ws` proxy is working
- Check JWT token is valid

### "404 Frontend Not Found"
- Verify publish directory is `frontend/dist`
- Check build command runs successfully
- Review frontend build logs

---

## Monitoring & Logs

### View Logs
1. Dashboard → Select service
2. Click "Logs" tab
3. Search for errors

### Health Check
```bash
# Backend health
curl https://elsaworkflow-api.onrender.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","password":"demo123"}'

# Frontend health
curl https://elsaworkflow-ui.onrender.com
```

### Performance
- Backend: ~1-2 seconds per request
- Frontend: ~2-3 seconds load time
- WebSocket: ~1 second to connect

---

## Cost

**Free Tier:**
- Static Site (Frontend): $0/month
- Web Service (Backend): Free first 750 hours, then spins down

**Paid Tiers:**
- Starter: $7/month per service
- Standard: $21/month per service

---

## Custom Domain (Optional)

### Add to Backend
1. Dashboard → `elsaworkflow-api`
2. Settings → Custom Domain
3. Add `api.yourdomain.com`
4. Update DNS records

### Add to Frontend
1. Dashboard → `elsaworkflow-ui`
2. Settings → Custom Domain
3. Add `app.yourdomain.com`
4. Update DNS records

---

## Auto-Deploy

Enable automatic redeployment on git push:

1. Service Settings
2. Enable "Auto-Deploy"
3. Next `git push` triggers rebuild

---

## Rollback

To rollback to previous version:

1. Dashboard → Service → Deployments
2. Find previous deployment
3. Click "Redeploy"

---

## Next Steps

After deployment:

- [ ] Test all 3 workflows
- [ ] Verify real-time updates work
- [ ] Change JWT secret key
- [ ] Enable auto-deploy
- [ ] Set up monitoring
- [ ] Add custom domain (optional)

---

## Support

- **Render Docs**: https://render.com/docs
- **API Issues**: Check deployment logs
- **Need Help?**: https://render.com/support
- **Status**: https://status.render.com

---

## Related Docs

- **Full Deployment Guide**: `DEPLOY_TO_RENDER.md`
- **Deployment Checklist**: `DEPLOYMENT_CHECKLIST.md`
- **Project README**: `README.md`

---

**Happy deploying!** 🚀

Questions? Check the detailed guides above or Render documentation.
