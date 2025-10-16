========================================
STUDENT ENROLLMENT SYSTEM
Quick Start Guide
========================================

Thank you for testing the Student Enrollment System!
This guide will help you get started in just a few simple steps.

========================================
SYSTEM REQUIREMENTS
========================================

- Windows 10/11 (64-bit) OR Linux (64-bit)
- 100 MB free disk space
- Port 5000 available (not used by another program)
- Web browser (Chrome, Firefox, Edge, or Safari)
- No additional software installation required!

========================================
HOW TO START THE APPLICATION
========================================

WINDOWS:
1. Extract the ZIP file to a folder on your computer
2. Open the extracted folder
3. Double-click: START_SERVER.bat
4. Your web browser will automatically open to http://localhost:5000
5. You're ready to go!

LINUX:
1. Extract the tar.gz file: tar -xzf StudentEnrollmentSystem.tar.gz
2. Open terminal in the extracted folder
3. Run: ./start-server.sh
4. Your web browser will automatically open to http://localhost:5000
5. You're ready to go!

========================================
LOGGING IN
========================================

See the ACCOUNTS.txt file for all test account credentials.

RECOMMENDED: Start with the Admin account to explore all features
Email:    admin@university.edu
Password: Admin123!

Other available accounts:
- Dean account (approve student registrations)
- Student accounts (enrolled students with complete profiles)
- Staff accounts (accounting, SAO, library, records)

========================================
MAIN FEATURES TO TEST
========================================

1. STUDENT REGISTRATION & APPROVAL WORKFLOW
   - Register a new student account
   - Login as Admin/Dean to approve pending registrations
   - Student ID is automatically generated (format: YYYY-DEPT-0001)
   - Student must complete profile before system access

2. DEPARTMENT MANAGEMENT
   - Create and manage Philippine university departments
   - 8 pre-configured departments (CCS, COE, CBA, CAS, COED, CON, CCJ, CHM)
   - Activate/deactivate departments

3. STUDENT PROFILE (PHILIPPINE ADDRESS FORMAT)
   - Province, Municipality, Barangay, Postal Code
   - Complete academic information
   - Emergency contact details

4. COURSE ENROLLMENT
   - Browse available courses
   - Enroll in courses (with prerequisite checking)
   - View enrollment status

5. GRADING SYSTEM
   - Admin/Dean can assign grades
   - Track completed courses
   - GPA calculation

========================================
STOPPING THE APPLICATION
========================================

Press Ctrl+C in the terminal/command window where the server is running.
The application will stop and you can close the window.

========================================
TROUBLESHOOTING
========================================

PROBLEM: Browser doesn't open automatically
SOLUTION: Manually open your browser and go to: http://localhost:5000

PROBLEM: "Port 5000 is already in use" error
SOLUTION:
  - Stop any other programs using port 5000
  - Or change the port in appsettings.json and restart

PROBLEM: "Cannot find app.db" or database errors
SOLUTION:
  - Delete the app.db file (if exists)
  - Restart the application
  - Database will be recreated with fresh test data

PROBLEM: Login not working
SOLUTION:
  - Verify email and password from ACCOUNTS.txt
  - Passwords are case-sensitive
  - Try copy/paste to avoid typos

PROBLEM: Windows security warning when running .exe
SOLUTION:
  - This is normal for unsigned applications
  - Click "More info" then "Run anyway"
  - The application is safe to run

========================================
TESTING GUIDES
========================================

For detailed testing instructions, see:
- CLIENT_TESTING_GUIDE.md - Comprehensive testing walkthrough
- PHILIPPINE_FEATURES_TEST.md - Philippine-specific features

========================================
FEEDBACK & QUESTIONS
========================================

While testing, please note:
- Any features that don't work as expected
- Any confusing or unclear interface elements
- Suggestions for improvements
- Missing features you'd like to see

We appreciate your thorough testing and feedback!

========================================
IMPORTANT NOTES
========================================

1. This is a TEST version with sample data
2. All data is stored locally in a file (app.db)
3. No internet connection required
4. No data leaves your computer
5. You can delete everything when done testing

========================================
THANK YOU FOR TESTING!
========================================

We look forward to your feedback and suggestions.

For technical support, please contact the development team.
