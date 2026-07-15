#!/bin/bash

echo "=== ElsaWorkflow Backend Setup ==="
echo ""

cd backend/ElsaWorkflow.Api

echo "Restoring NuGet packages..."
dotnet restore

if [ $? -ne 0 ]; then
    echo "Error: Failed to restore NuGet packages"
    exit 1
fi

echo ""
echo "Building backend..."
dotnet build

if [ $? -ne 0 ]; then
    echo "Error: Failed to build backend"
    exit 1
fi

echo ""
echo "✅ Backend setup complete!"
echo "To run the backend, execute:"
echo "  cd backend/ElsaWorkflow.Api"
echo "  dotnet run"
echo ""
