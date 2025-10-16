# Client Deployment Guide - Student Enrollment System

This guide explains how to prepare a deployment package for client testing using **Option 2: Standalone Published Application**.

## Overview

This deployment method creates a self-contained executable package that:
- Includes the .NET runtime (no installation needed on client machine)
- Works on Windows or Linux
- Contains all dependencies and database
- Is ~60-80 MB in size
- Runs on localhost:5000

## Prerequisites

Before creating the deployment package, ensure you have:
- .NET 9.0 SDK installed on your development machine
- Latest code with all migrations applied
- All tests passing

## Step 1: Choose Target Platform

Decide which platform the client will use:
- **Windows** - Most common for business users
- **Linux** - For Linux servers or workstations

You can create packages for both platforms if needed.

## Step 2: Run the Publish Script

### For Windows Deployment

1. Open Command Prompt or PowerShell
2. Navigate to the project root directory
3. Run the publish script:

```cmd
DEPLOYMENT_SCRIPTS\publish-windows.bat
```

The script will:
- Clean previous builds
- Publish as self-contained Windows x64 application
- Copy documentation files
- Create a launcher script (START_SERVER.bat)
- Output to: `DEPLOYMENT_OUTPUT/windows/StudentEnrollmentSystem/`

### For Linux Deployment

1. Open Terminal
2. Navigate to the project root directory
3. Make the script executable (first time only):

```bash
chmod +x DEPLOYMENT_SCRIPTS/publish-linux.sh
```

4. Run the publish script:

```bash
./DEPLOYMENT_SCRIPTS/publish-linux.sh
```

The script will:
- Clean previous builds
- Publish as self-contained Linux x64 application
- Copy documentation files
- Create a launcher script (start-server.sh)
- Output to: `DEPLOYMENT_OUTPUT/linux/StudentEnrollmentSystem/`

## Step 3: Verify the Build

Check the output directory to ensure it contains:

```
StudentEnrollmentSystem/
├── StudentEnrollmentSystem.exe (Windows) or StudentEnrollmentSystem (Linux)
├── START_SERVER.bat (Windows) or start-server.sh (Linux)
├── README.txt
├── ACCOUNTS.txt
├── app.db (SQLite database - may be created on first run)
├── wwwroot/ (static files)
├── appsettings.json
└── [Many DLL files and dependencies]
```

**Size check**: The folder should be approximately 60-80 MB.

## Step 4: Create the Distribution Package

### For Windows

1. Navigate to `DEPLOYMENT_OUTPUT/windows/`
2. Right-click the `StudentEnrollmentSystem` folder
3. Select "Send to" → "Compressed (zipped) folder"
4. Rename to: `StudentEnrollmentSystem-Windows-v1.0.zip`

**Command line alternative:**
```cmd
cd DEPLOYMENT_OUTPUT\windows
powershell Compress-Archive -Path StudentEnrollmentSystem -DestinationPath StudentEnrollmentSystem-Windows-v1.0.zip
```

### For Linux

1. Navigate to `DEPLOYMENT_OUTPUT/linux/`
2. Create a tar.gz archive:

```bash
cd DEPLOYMENT_OUTPUT/linux
tar -czf StudentEnrollmentSystem-Linux-v1.0.tar.gz StudentEnrollmentSystem/
```

## Step 5: Test the Package Locally

Before sending to the client, test the package yourself:

### Windows Testing
1. Extract the ZIP to a test location (e.g., Desktop)
2. Open the extracted folder
3. Double-click `START_SERVER.bat`
4. Browser should open to http://localhost:5000
5. Verify you can login with test accounts (see ACCOUNTS.txt)
6. Test key features (registration, enrollment, grading)

### Linux Testing
1. Extract the tar.gz: `tar -xzf StudentEnrollmentSystem-Linux-v1.0.tar.gz`
2. Open terminal in the extracted folder
3. Run: `./start-server.sh`
4. Open browser to http://localhost:5000
5. Verify you can login with test accounts
6. Test key features

## Step 6: Prepare for Client Delivery

Create a delivery package that includes:

1. **The Application Archive**
   - `StudentEnrollmentSystem-Windows-v1.0.zip` or
   - `StudentEnrollmentSystem-Linux-v1.0.tar.gz`

2. **Testing Documentation** (copy from project root)
   - `CLIENT_TESTING_GUIDE.md` - Comprehensive testing instructions
   - `PHILIPPINE_FEATURES_TEST.md` - Philippine-specific features test guide

3. **Email Template** (see below)

### Email Template for Client

```
Subject: Student Enrollment System - Ready for Testing

Dear [Client Name],

The Student Enrollment System is ready for your testing and review.

SYSTEM FEATURES:
- Student registration with approval workflow
- Department management (Philippine universities)
- Automatic student ID generation (YYYY-DEPT-0001 format)
- Course enrollment and grading
- Philippine address format (Province, Municipality, Barangay)
- Role-based access (Admin, Dean, Instructor, Student)

GETTING STARTED:
1. Extract the attached ZIP file
2. Open the extracted folder
3. Run START_SERVER.bat (Windows) or start-server.sh (Linux)
4. Your browser will open to http://localhost:5000

TEST ACCOUNTS:
See ACCOUNTS.txt in the extracted folder for login credentials.
I recommend starting with the Admin account to explore all features.

TESTING GUIDES:
- CLIENT_TESTING_GUIDE.md - Complete testing walkthrough
- PHILIPPINE_FEATURES_TEST.md - Focus on new Philippine features

TECHNICAL NOTES:
- No installation required - includes .NET runtime
- Runs entirely on your local machine (localhost)
- Database is SQLite (file-based, no server needed)
- Port 5000 must be available
- Stop the server by pressing Ctrl+C in the terminal

SUPPORT:
Please test all features and provide feedback. I'm available to address any issues or questions.

Best regards,
[Your Name]
```

## Step 7: Send to Client

**Recommended delivery methods:**
1. **File sharing service** - Google Drive, Dropbox, OneDrive (for large files)
2. **Email** - If file is under email size limit (~25MB)
3. **USB drive** - For in-person delivery

**Security note:** The package contains test data only. If using production data in the future, ensure secure delivery methods and encrypted archives.

## Troubleshooting

### Build Fails
- Ensure .NET 9.0 SDK is installed: `dotnet --version`
- Restore packages: `dotnet restore`
- Check for build errors: `dotnet build`

### Package Too Large
- Normal size is 60-80 MB (includes .NET runtime)
- If larger, check for unnecessary files in output

### Client Cannot Run Application
- **Port in use**: Another application using port 5000
  - Solution: Stop other services or change port in appsettings.json
- **Windows SmartScreen**: May block unsigned executable
  - Solution: Click "More info" → "Run anyway"
- **Linux permissions**: Script not executable
  - Solution: `chmod +x start-server.sh StudentEnrollmentSystem`

### Database Issues
- Delete `app.db` file to reset to fresh state
- First run creates database automatically with seed data

## Next Steps After Client Testing

1. **Collect Feedback** - Review client testing results
2. **Fix Issues** - Address any bugs or problems found
3. **Update Features** - Implement requested changes
4. **Prepare Production** - When approved, prepare for production deployment

## Notes

- This deployment is for **testing purposes only**
- For production, consider:
  - Cloud hosting (Azure, AWS, etc.)
  - Proper database server (SQL Server, PostgreSQL)
  - SSL certificates for HTTPS
  - Domain name and public access
  - Backup and monitoring systems

---

**Questions?** Review the testing guides or contact the development team.
