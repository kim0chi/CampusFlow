#!/bin/bash

# ========================================
# Student Enrollment System - Linux Publish Script
# ========================================
# This script creates a self-contained Linux deployment
# that includes the .NET runtime (no installation needed)
# ========================================

echo ""
echo "========================================"
echo "Student Enrollment System"
echo "Linux Deployment Publisher"
echo "========================================"
echo ""

# Change to project root directory
cd "$(dirname "$0")/.."

# Clean previous builds
echo "[1/4] Cleaning previous builds..."
rm -rf bin/Release
rm -rf DEPLOYMENT_OUTPUT/linux

# Publish for Linux x64 (self-contained)
echo ""
echo "[2/4] Publishing for Linux (x64)..."
echo "This may take a few minutes..."
dotnet publish -c Release -r linux-x64 --self-contained true -o DEPLOYMENT_OUTPUT/linux/StudentEnrollmentSystem

if [ $? -ne 0 ]; then
    echo ""
    echo "ERROR: Publish failed. Please check the error messages above."
    exit 1
fi

# Copy additional files
echo ""
echo "[3/4] Copying documentation files..."
cp -f ACCOUNTS.txt DEPLOYMENT_OUTPUT/linux/StudentEnrollmentSystem/ 2>/dev/null || true
cp -f README_FOR_CLIENT.txt DEPLOYMENT_OUTPUT/linux/StudentEnrollmentSystem/README.txt 2>/dev/null || true

# Create launch script
echo ""
echo "[4/4] Creating launcher script..."
cat > DEPLOYMENT_OUTPUT/linux/StudentEnrollmentSystem/start-server.sh << 'EOF'
#!/bin/bash
echo "Starting Student Enrollment System..."
echo ""
echo "Server will be available at: http://localhost:5000"
echo "Press Ctrl+C to stop the server"
echo ""

# Make executable if not already
chmod +x ./StudentEnrollmentSystem

# Try to open browser (works on most Linux desktop environments)
if command -v xdg-open > /dev/null; then
    xdg-open http://localhost:5000 &
fi

# Start the server
./StudentEnrollmentSystem
EOF

# Make scripts executable
chmod +x DEPLOYMENT_OUTPUT/linux/StudentEnrollmentSystem/start-server.sh
chmod +x DEPLOYMENT_OUTPUT/linux/StudentEnrollmentSystem/StudentEnrollmentSystem

# Success message
echo ""
echo "========================================"
echo "SUCCESS!"
echo "========================================"
echo ""
echo "Deployment package created at:"
echo "$(pwd)/DEPLOYMENT_OUTPUT/linux/StudentEnrollmentSystem"
echo ""
echo "Package size: ~60-80 MB"
echo ""
echo "Next steps:"
echo "1. Compress the 'StudentEnrollmentSystem' folder to tar.gz:"
echo "   cd DEPLOYMENT_OUTPUT/linux"
echo "   tar -czf StudentEnrollmentSystem.tar.gz StudentEnrollmentSystem/"
echo "2. Send tar.gz file to client"
echo "3. Client extracts and runs: ./start-server.sh"
echo ""
echo "========================================"
