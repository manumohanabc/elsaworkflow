#!/bin/bash

echo "=== ElsaWorkflow Frontend Setup ==="
echo ""

cd frontend

echo "Installing dependencies..."
npm install

if [ $? -ne 0 ]; then
    echo "Error: Failed to install npm dependencies"
    exit 1
fi

echo ""
echo "✅ Frontend setup complete!"
echo "To run the frontend, execute: npm run dev"
echo ""
