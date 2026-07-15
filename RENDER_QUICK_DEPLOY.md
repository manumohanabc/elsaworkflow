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

### 2️⃣ Deploy on Render

**Option A: One-Click Blueprint (Easiest)**

1. Go to: https://render.com/i/c/add-blueprint
2. Paste repository URL
3. Confirm settings
4. Click "Create"
5. Wait ~20 minutes for deployment

**Option B: Manual Deploy**

1. Go to: https://dashboard.render.com
2. Click "New +" → "Web Service" (for backend)
3. Select GitHub repo
4. Configure:
   - Name: `elsaworkflow-api`
   - Environment: `Dotnet`
   - Build: `cd backend/ElsaWorkflow.Api && dotnet publish -c Release`
   - Start: `./out/ElsaWorkflow.Api`
5. Click "Create Web Service"
6. Wait for deployment
7. Click "New +" → "Static Site" (for frontend)
8. Configure:
   - Name: `elsaworkflow-ui`
   - Build: `cd frontend && npm install && npm run build`
   - Publish Dir: `frontend/dist`
9. Click "Create Static Site"

### 3️⃣ Set Environment Variables

**Backend** (`elsaworkflow-api`):
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:10000
JWT_SECRET_KEY=<generate-random-key>
CORS_ALLOWED_ORIGINS=https://elsaworkflow-ui.onrender.com
```

**Frontend** (`elsaworkflow-ui`):
```
VITE_API_URL=https://elsaworkflow-api.onrender.com/api
VITE_SIGNALR_URL=https://elsaworkflow-api.onrender.com
```

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
