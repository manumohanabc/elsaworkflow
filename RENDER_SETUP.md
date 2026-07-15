# Render Deployment - Step by Step

Complete guide to deploy both services on Render (Free Tier).

---

## ✅ Prerequisites

- [ ] GitHub account
- [ ] Code pushed to GitHub
- [ ] Render account (https://render.com - free)

---

## 🔄 Step 1: Push Code to GitHub

```bash
cd /Users/manumohan/elsaworkflow

# Initialize if needed
git init

# Add all files
git add .

# Commit
git commit -m "Ready for Render deployment"

# Add remote (replace YOUR_USERNAME)
git remote add origin https://github.com/YOUR_USERNAME/elsaworkflow.git

# Push
git branch -M main
git push -u origin main
```

Verify code appears on GitHub.

---

## 🚀 Step 2: Deploy Backend

1. Go to: **https://dashboard.render.com**

2. Click **"New +"** → **"Web Service"**

3. **Connect Repository**
   - Click "Connect" and select your GitHub account
   - Find and select `elsaworkflow` repository
   - Click "Connect"

4. **Configure Service**
   - **Name**: `elsaworkflow-api`
   - **Environment**: Scroll down and select `Dotnet`
   - **Build Command**: 
     ```
     cd backend/ElsaWorkflow.Api && dotnet publish -c Release
     ```
   - **Start Command**: 
     ```
     ./out/ElsaWorkflow.Api
     ```
   - **Instance Type**: Select `Free` (top option)

5. **Environment Variables**
   - Add these variables (click "Add Environment Variable"):
   
   | Key | Value |
   |-----|-------|
   | ASPNETCORE_ENVIRONMENT | Production |
   | ASPNETCORE_URLS | http://0.0.0.0:10000 |
   | JWT_SECRET_KEY | (Generate a random string) |
   | CORS_ALLOWED_ORIGINS | (We'll update this after frontend deploys) |

   - For JWT_SECRET_KEY, generate something like:
     ```
     ThisIsASecretKey123!@#$%^&*()abcdef
     ```

6. Click **"Create Web Service"**

7. ⏳ **Wait 10-15 minutes** for deployment
   - Watch the "Logs" tab for "Application started"
   - Status should change to "Live"

8. 📝 **Copy the URL** (shown at top, e.g., `https://elsaworkflow-api.onrender.com`)
   - You'll need this for the frontend!

---

## 🎨 Step 3: Deploy Frontend

1. Go back to: **https://dashboard.render.com**

2. Click **"New +"** → **"Web Service"** (same as backend)

3. **Connect Repository**
   - Click "Connect" and select `elsaworkflow` repository again
   - Click "Connect"

4. **Configure Service**
   - **Name**: `elsaworkflow-ui`
   - **Environment**: Select `Node`
   - **Build Command**: 
     ```
     cd frontend && npm install && npm run build
     ```
   - **Start Command**: 
     ```
     npx serve -s dist -l 3000
     ```
   - **Instance Type**: Select `Free`

5. **Environment Variables**
   - Add these variables:
   
   | Key | Value |
   |-----|-------|
   | VITE_API_URL | https://elsaworkflow-api.onrender.com/api |
   | VITE_SIGNALR_URL | https://elsaworkflow-api.onrender.com |

   > Replace the backend URL with the one you copied in Step 2!

6. Click **"Create Web Service"**

7. ⏳ **Wait 5-10 minutes** for deployment
   - Watch the "Logs" tab
   - Status should change to "Live"

8. 📝 **Copy the frontend URL** (e.g., `https://elsaworkflow-ui.onrender.com`)

---

## ✏️ Step 4: Update Backend CORS

1. Go to: **https://dashboard.render.com**

2. Click **"elsaworkflow-api"** service

3. Click **"Environment"** tab

4. Find `CORS_ALLOWED_ORIGINS` and update it:
   ```
   https://elsaworkflow-ui.onrender.com
   ```
   (Use the frontend URL from Step 3)

5. Click **"Save"**

6. Service will auto-redeploy (watch logs)

---

## 🧪 Step 5: Test

1. Open frontend URL in browser:
   ```
   https://elsaworkflow-ui.onrender.com
   ```

2. Login with demo credentials:
   - **Username**: `demo`
   - **Password**: `demo123`

3. You should see:
   - ✅ Workflow list loads
   - ✅ Three workflows available
   - ✅ Can click "Execute Workflow"
   - ✅ Workflow starts executing
   - ✅ Real-time updates appear
   - ✅ Tasks complete successfully

4. **Check browser console** (F12):
   - ✅ No CORS errors
   - ✅ No WebSocket errors
   - ✅ No red errors

---

## 🎯 Success Indicators

| Component | Check |
|-----------|-------|
| Backend | Service shows "Live" in dashboard |
| Frontend | Service shows "Live" in dashboard |
| Login | Can login with `demo/demo123` |
| Workflows | Workflow list appears |
| Execution | Workflows execute and update in real-time |
| WebSocket | No errors in browser console |

---

## 🔧 Troubleshooting

### Backend Won't Deploy

**Problem**: Build fails or service stays "Creating"

**Solution**:
1. Click on `elsaworkflow-api` service
2. Go to "Logs" tab
3. Look for error messages
4. Common issues:
   - Missing files: Check file structure
   - Build command wrong: Verify paths are correct
   - SDK issue: Check .NET target framework

### Frontend Won't Deploy

**Problem**: Build fails or 404 errors

**Solution**:
1. Click on `elsaworkflow-ui` service
2. Go to "Logs" tab
3. Look for errors like:
   - `npm install` failed → Check package.json
   - `npm run build` failed → Test locally with `npm run build`
   - Missing dist files → Verify build command

### Login Fails

**Problem**: Can't login or "Network Error"

**Solution**:
1. Verify backend is "Live" and accessible
2. Test backend directly:
   ```bash
   curl https://elsaworkflow-api.onrender.com/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"username":"demo","password":"demo123"}'
   ```
3. Check browser console for CORS errors
4. Verify CORS_ALLOWED_ORIGINS includes frontend URL

### Real-time Updates Not Working

**Problem**: Workflows execute but don't update in real-time

**Solution**:
1. Open browser console (F12)
2. Check for WebSocket errors
3. Verify `VITE_SIGNALR_URL` is set to backend URL
4. Check backend logs for SignalR errors

### Service Goes to Sleep

**Problem**: Free tier services go to sleep after 15 min inactivity

**Solution**:
- Service will automatically wake up when accessed
- If you need always-on, upgrade to Starter plan ($7/month)
- Click service → "Billing" → Upgrade plan

---

## 📊 Monitor Your Services

### View Logs
1. Click service name in dashboard
2. Click "Logs" tab
3. Search for errors

### Check Health
- Backend: `https://elsaworkflow-api.onrender.com/api/auth/login`
- Frontend: `https://elsaworkflow-ui.onrender.com`

### Monitor Activity
- Click service
- View "Metrics" tab
- Check CPU, memory, response times

---

## 💰 Cost Summary

| Service | Free Tier | Cost |
|---------|-----------|------|
| Frontend (Web Service) | 750 hours/month | Free |
| Backend (Web Service) | 750 hours/month | Free |
| **Total** | **Both free!** | **$0** |

> Free tier services auto-sleep after 15 min inactivity. For always-on, upgrade to Starter ($7/month per service).

---

## 🔄 Update & Redeploy

After making changes locally:

```bash
# Stage changes
git add .

# Commit
git commit -m "Update application"

# Push to GitHub
git push origin main
```

**Render automatically redeploys** when you push to `main` (if auto-deploy is enabled).

---

## Next Steps

- [ ] Test all 3 workflows
- [ ] Verify WebSocket works (real-time updates)
- [ ] Share URLs with team
- [ ] Enable auto-deploy (optional)
- [ ] Set up custom domain (optional)
- [ ] Monitor logs regularly

---

## Support

- **Render Dashboard**: https://dashboard.render.com
- **Render Docs**: https://render.com/docs
- **Check Status**: https://status.render.com
- **Get Help**: https://render.com/support

---

## Quick Reference

| What | Where | URL |
|------|-------|-----|
| Dashboard | Render | https://dashboard.render.com |
| Backend Service | Render | `elsaworkflow-api` |
| Frontend Service | Render | `elsaworkflow-ui` |
| App (Frontend) | Browser | `https://elsaworkflow-ui.onrender.com` |
| Backend API | Browser | `https://elsaworkflow-api.onrender.com` |
| GitHub | Browser | `https://github.com/YOUR_USERNAME/elsaworkflow` |

---

**Congratulations!** Your app is now live on Render! 🎉
