# Render Deployment Checklist

Complete this checklist before deploying to Render.

## Pre-Deployment Setup

### Code Repository
- [ ] Code is committed to GitHub
- [ ] Repository is public (or you have access)
- [ ] Branch name is `main` or `master`
- [ ] `.gitignore` is properly configured
- [ ] No sensitive data in code (API keys, passwords, etc.)

### Code Quality
- [ ] Backend builds locally: `dotnet build`
- [ ] Frontend builds locally: `npm run build`
- [ ] No console errors/warnings in local testing
- [ ] All workflows execute successfully locally
- [ ] Login works with demo credentials

### Environment Variables
- [ ] Backup current `.env` files
- [ ] Review `.env.example` for all variables
- [ ] Create `.env.production` for frontend
- [ ] Have production JWT secret key prepared
- [ ] CORS domains configured correctly

### Configuration Files
- [ ] `render.yaml` is present and correct
- [ ] `docker-compose.yml` is updated (if using Docker)
- [ ] `Dockerfile` files are created for both services
- [ ] `appsettings.json` uses environment variables
- [ ] `appsettings.Production.json` is configured

---

## Deployment Steps

### Step 1: Create GitHub Repository
- [ ] Create repository on GitHub
- [ ] Add repository as remote
- [ ] Push code to `main` branch
- [ ] Verify code appears on GitHub

```bash
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/YOUR_USERNAME/elsaworkflow.git
git branch -M main
git push -u origin main
```

### Step 2: Deploy Backend to Render

#### Option A: Using render.yaml (Recommended)
- [ ] Go to https://dashboard.render.com
- [ ] Click "New +" → "Blueprint"
- [ ] Select GitHub repository
- [ ] Confirm `render.yaml` is detected
- [ ] Add required environment variables
- [ ] Click "Deploy"

#### Option B: Manual Setup
- [ ] Go to https://dashboard.render.com
- [ ] Click "New +" → "Web Service"
- [ ] Connect GitHub repository
- [ ] Configure as `.NET` service
- [ ] Set Build Command: `cd backend/ElsaWorkflow.Api && dotnet publish -c Release`
- [ ] Set Start Command: `./out/ElsaWorkflow.Api`
- [ ] Add environment variables (see Environment Variables section)
- [ ] Click "Create Web Service"
- [ ] Wait 10-15 minutes for deployment
- [ ] Note the backend URL (e.g., `https://elsaworkflow-api.onrender.com`)

### Step 3: Deploy Frontend to Render

#### Option A: Using render.yaml
- [ ] Blueprint should include frontend service
- [ ] Verify deployment completed

#### Option B: Manual Setup
- [ ] Go to https://dashboard.render.com
- [ ] Click "New +" → "Static Site"
- [ ] Connect GitHub repository
- [ ] Set Build Command: `cd frontend && npm install && npm run build`
- [ ] Set Publish Directory: `frontend/dist`
- [ ] Add environment variables:
  - [ ] `VITE_API_URL=https://elsaworkflow-api.onrender.com/api`
  - [ ] `VITE_SIGNALR_URL=https://elsaworkflow-api.onrender.com`
- [ ] Click "Create Static Site"
- [ ] Wait 5-10 minutes for deployment
- [ ] Note the frontend URL (e.g., `https://elsaworkflow-ui.onrender.com`)

### Step 4: Configure Environment Variables

#### Backend Environment Variables
Set these on the Render backend service:

- [ ] `ASPNETCORE_ENVIRONMENT` = `Production`
- [ ] `ASPNETCORE_URLS` = `http://0.0.0.0:10000`
- [ ] `JWT_SECRET_KEY` = `<your-production-secret-key>`
- [ ] `CORS_ALLOWED_ORIGINS` = `https://elsaworkflow-ui.onrender.com`

#### Frontend Environment Variables
Set these on the Render frontend service:

- [ ] `VITE_API_URL` = `https://elsaworkflow-api.onrender.com/api`
- [ ] `VITE_SIGNALR_URL` = `https://elsaworkflow-api.onrender.com`

### Step 5: Wait for Deployment

- [ ] Backend service shows "Live" status
- [ ] Frontend site shows "Live" status
- [ ] Check deployment logs for errors
- [ ] Note both URLs for testing

---

## Post-Deployment Testing

### Backend Testing
- [ ] Backend URL is accessible
- [ ] Login endpoint returns JWT token

```bash
curl -X POST https://YOUR_BACKEND_URL/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"demo","password":"demo123"}'
```

- [ ] Health check passes (if configured)
- [ ] Logs show no errors

### Frontend Testing
- [ ] Frontend URL is accessible
- [ ] Page loads without 404 errors
- [ ] Login form appears
- [ ] Can login with demo credentials
- [ ] Workflow list loads
- [ ] Can select a workflow
- [ ] Can start workflow execution
- [ ] Real-time updates appear (check browser console)
- [ ] Task progress updates in real-time
- [ ] Workflow completes successfully

### Integration Testing
- [ ] [ ] All three workflows execute successfully
- [ ] WebSocket connection established (green indicator)
- [ ] Concurrent executions work
- [ ] Cancel execution works
- [ ] Logout works
- [ ] Re-login works

### Browser Console
- [ ] No CORS errors
- [ ] No WebSocket errors
- [ ] No JavaScript errors
- [ ] API calls return 200 status

### Performance
- [ ] Frontend loads in < 3 seconds
- [ ] API responses are < 1 second
- [ ] WebSocket connects within 2 seconds
- [ ] No timeouts or freezing

---

## Troubleshooting Issues

### Backend Won't Start
- [ ] Check build logs for compile errors
- [ ] Verify all dependencies are available
- [ ] Check environment variables are set
- [ ] Review .NET SDK version compatibility

### Frontend Won't Deploy
- [ ] Check npm build logs
- [ ] Verify package.json is correct
- [ ] Check node_modules are not committed
- [ ] Verify build command works locally

### CORS Errors
- [ ] Verify frontend URL is in CORS_ALLOWED_ORIGINS
- [ ] Check CORS config includes all origins
- [ ] Restart backend service after CORS changes

### WebSocket Won't Connect
- [ ] Verify ws:// proxy is configured
- [ ] Check JWT token is being passed
- [ ] Verify backend hub endpoint is `/workflowHub`
- [ ] Check browser WebSocket support

### Login Fails
- [ ] Verify backend is responding
- [ ] Check token is stored in localStorage
- [ ] Verify demo credentials are correct
- [ ] Check CORS allows login endpoint

---

## Performance Optimization

After deployment, consider:

- [ ] Enable caching headers on static assets
- [ ] Minimize frontend bundle size
- [ ] Optimize images and assets
- [ ] Consider upgrading from free tier to Starter
- [ ] Set up monitoring and alerts
- [ ] Configure auto-deploy on git push

---

## Security Checklist

- [ ] JWT_SECRET_KEY is changed from default
- [ ] HTTPS is enabled (Render provides this)
- [ ] CORS origins are restricted
- [ ] No sensitive data in environment variables
- [ ] Database credentials are not in code
- [ ] API keys are stored as environment variables
- [ ] Logging doesn't expose sensitive data

---

## Post-Deployment Maintenance

### Monitoring
- [ ] Set up error alerts
- [ ] Monitor response times
- [ ] Track WebSocket connections
- [ ] Review logs regularly

### Updates
- [ ] Keep .NET SDK updated
- [ ] Update npm packages regularly
- [ ] Review Render service updates
- [ ] Test updates in staging first

### Backup & Recovery
- [ ] Document all environment variables
- [ ] Keep deployment configuration in version control
- [ ] Test recovery procedures
- [ ] Document any manual configurations

---

## Support

### Useful Links
- Render Dashboard: https://dashboard.render.com
- Render Docs: https://render.com/docs
- .NET Deployment: https://render.com/docs/deploy-dotnet
- Node.js Deployment: https://render.com/docs/deploy-node

### Troubleshooting
- Render Status: https://status.render.com
- Common Issues: https://render.com/docs/common-issues
- Support: https://render.com/support

---

## Rollback Plan

If deployment fails:

1. [ ] Check Render logs for errors
2. [ ] Revert last commit if code change caused issue
3. [ ] Redeploy previous working version
4. [ ] Verify services are back online
5. [ ] Test all functionality

---

## Sign-Off

- [ ] All tests passed
- [ ] All security checks completed
- [ ] Documentation updated
- [ ] Team notified
- [ ] Monitoring configured
- [ ] Production ready ✅

**Date Deployed:** _______________  
**Deployed By:** _______________  
**Notes:** _______________________________________________

---

Good luck with your deployment! 🚀
