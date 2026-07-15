#!/bin/bash

set -e

echo "================================"
echo "ElsaWorkflow Render Deployment"
echo "================================"
echo ""

# Check if GitHub is configured
if ! git remote get-url origin &>/dev/null; then
    echo "❌ Error: Not a git repository or no remote configured"
    echo ""
    echo "Setup GitHub repository:"
    echo "  git init"
    echo "  git remote add origin https://github.com/YOUR_USERNAME/elsaworkflow.git"
    echo "  git branch -M main"
    exit 1
fi

REPO_URL=$(git remote get-url origin)
echo "📦 Repository: $REPO_URL"
echo ""

# Check uncommitted changes
if ! git diff-index --quiet HEAD --; then
    echo "⚠️  You have uncommitted changes"
    echo ""
    read -p "Continue with deployment? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

echo "📝 Deployment Options:"
echo "1. Deploy to Render (recommended)"
echo "2. Deploy with Docker Compose (local)"
echo "3. Just push to GitHub (without deployment)"
echo ""
read -p "Choose option (1-3): " option

case $option in
    1)
        echo ""
        echo "🚀 Deploying to Render..."
        echo ""
        echo "1. Go to https://dashboard.render.com"
        echo "2. Click 'New +' → 'Blueprint'"
        echo "3. Connect GitHub repository"
        echo "4. Select this repository"
        echo "5. Click 'Deploy'"
        echo ""
        echo "Or use Render CLI (if installed):"
        echo "  render deploy --repo $REPO_URL"
        ;;

    2)
        echo ""
        echo "🐳 Deploying with Docker Compose..."
        echo ""

        if ! command -v docker &> /dev/null; then
            echo "❌ Docker is not installed"
            echo "Install from: https://www.docker.com/products/docker-desktop"
            exit 1
        fi

        if ! command -v docker-compose &> /dev/null; then
            echo "❌ Docker Compose is not installed"
            echo "Install from: https://docs.docker.com/compose/install/"
            exit 1
        fi

        echo "Building and starting services..."
        docker-compose down 2>/dev/null || true
        docker-compose build --no-cache
        docker-compose up -d

        echo ""
        echo "✅ Services started!"
        echo "  Frontend: http://localhost:5173"
        echo "  Backend:  http://localhost:5000"
        echo ""
        echo "Logs:"
        echo "  docker-compose logs -f"
        echo ""
        echo "Stop:"
        echo "  docker-compose down"
        ;;

    3)
        echo ""
        echo "📤 Pushing to GitHub..."
        ;;

    *)
        echo "❌ Invalid option"
        exit 1
        ;;
esac

# Push to GitHub
echo ""
echo "Preparing git..."
git add -A
git status

echo ""
read -p "Commit message (press Enter for 'Deploy to Render'): " commit_msg
commit_msg=${commit_msg:-"Deploy to Render"}

echo ""
echo "Committing..."
git commit -m "$commit_msg" || echo "Nothing to commit"

echo ""
echo "Pushing to GitHub..."
git push origin main

echo ""
echo "✅ Done!"
echo ""

if [ "$option" = "1" ]; then
    echo "🔗 Next steps:"
    echo "1. Go to https://dashboard.render.com"
    echo "2. Click 'New +' → 'Blueprint'"
    echo "3. Paste this URL: $REPO_URL"
    echo "4. Select the blueprint and deploy"
    echo ""
elif [ "$option" = "2" ]; then
    echo "🔗 Access your application:"
    echo "  Frontend: http://localhost:5173"
    echo "  Backend:  http://localhost:5000"
    echo "  Login:    demo / demo123"
fi
