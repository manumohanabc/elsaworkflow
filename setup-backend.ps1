Write-Host "=== ElsaWorkflow Backend Setup ===" -ForegroundColor Green
Write-Host ""

Set-Location backend/ElsaWorkflow.Api

Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to restore NuGet packages" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Building backend..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to build backend" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✅ Backend setup complete!" -ForegroundColor Green
Write-Host "To run the backend, execute:" -ForegroundColor Cyan
Write-Host "  cd backend/ElsaWorkflow.Api" -ForegroundColor Cyan
Write-Host "  dotnet run" -ForegroundColor Cyan
Write-Host ""
