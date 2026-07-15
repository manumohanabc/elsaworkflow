Write-Host "=== ElsaWorkflow Frontend Setup ===" -ForegroundColor Green
Write-Host ""

Set-Location frontend

Write-Host "Installing dependencies..." -ForegroundColor Yellow
npm install

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to install npm dependencies" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✅ Frontend setup complete!" -ForegroundColor Green
Write-Host "To run the frontend, execute: npm run dev" -ForegroundColor Cyan
Write-Host ""
