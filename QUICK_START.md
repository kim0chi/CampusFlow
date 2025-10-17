# Quick Start Guide

## Prerequisites

✅ **.NET 9.0 SDK** - Already installed
✅ **SQLite** - Bundled with the project (no installation needed!)

## Running the Application

1. **Build the project:**
   ```bash
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

   On first run, the application will:
   - Create the SQLite database file (`app.db`)
   - Apply database migrations
   - Seed sample data (users and courses)
   - Start the web server

3. **Access the application:**
   - Open your browser and navigate to: https://localhost:5001
   - Or: http://localhost:5000

4. **First-time setup:**
   - The database is automatically created and initialized
   - No manual setup required!

## Testing the Complete Workflow

### Step 1: Login as a Student
1. Click "Register" or "Login" in the top right
2. Use credentials: `john.doe@student.edu` / `Student123!`

### Step 2: Enroll in a Course
1. Click "Browse Courses" in the navigation
2. Select a course and click "Enroll"
3. Fill out the enrollment form with optional comments
4. Submit the enrollment request
5. Go to "My Enrollments" to see the status (should show "DeanReview")

### Step 3: Approve as Dean
1. Logout and login as Dean: `dean@university.edu` / `Dean123!`
2. Click "Pending Approvals" in navigation
3. Click "Review" on the enrollment request
4. View details and click "Approve" with optional comments
5. Status moves to "AccountingReview"

### Step 4: Approve as Accounting
1. Logout and login as Accounting: `accounting@university.edu` / `Accounting123!`
2. Click "Pending Approvals"
3. Review and approve the request
4. Status moves to "SAOReview"

### Step 5: Approve as SAO
1. Logout and login as SAO: `sao@university.edu` / `SAO123!`
2. Click "Pending Approvals"
3. Review and approve the request
4. Status moves to "LibraryReview"

### Step 6: Approve as Library
1. Logout and login as Library: `library@university.edu` / `Library123!`
2. Click "Pending Approvals"
3. Review and approve the request
4. Status moves to "RecordsReview"

### Step 7: Final Approval as Records
1. Logout and login as Records: `records@university.edu` / `Records123!`
2. Click "Pending Approvals"
3. Review and approve the request
4. Status changes to "Enrolled"
5. Course enrolled count increases by 1

### Step 8: Verify Enrollment
1. Logout and login back as Student: `john.doe@student.edu` / `Student123!`
2. Go to "My Enrollments"
3. Click "Details" on the enrollment
4. View complete approval history with all approvers and their comments

## Testing Rejection Workflow

1. As a student, submit another enrollment request
2. Login as any approver (e.g., Dean)
3. Click "Review" on the request
4. Select "Reject" and add a comment explaining why
5. Status immediately changes to "Rejected"
6. Workflow ends - no further approvals occur
7. Student can see rejection reason in enrollment details

## Admin Features

### Login as Admin
- Email: `admin@university.edu`
- Password: `Admin123!`

### Manage Courses
1. Click "Admin" → "Manage Courses"
2. Create new courses
3. Edit existing courses
4. Deactivate courses
5. View enrolled student count vs capacity

### Manage Users and Roles
1. Click "Admin" → "Manage Users"
2. View all registered users
3. Click "Manage Roles" for any user
4. Add or remove roles
5. Users can have multiple roles

## Testing Multiple Enrollments

You can test with the three pre-configured student accounts:
- **John Doe**: john.doe@student.edu / Student123!
- **Jane Smith**: jane.smith@student.edu / Student123!
- **Mike Johnson**: mike.johnson@student.edu / Student123!

Each student can enroll in multiple courses simultaneously, and approvers will see all pending requests in their queue.

## Dashboard Features by Role

### Student Dashboard
- Shows recent enrollments with status
- Quick link to browse courses
- Quick link to view all enrollments

### Approver Dashboard (Dean, Accounting, SAO, Library, Records)
- Shows count of pending approvals
- Lists recent pending approvals
- Quick access to review pending items

### Admin Dashboard
- System-wide statistics:
  - Total enrollments
  - Pending enrollments
  - Completed enrollments
  - Total active courses
- Quick links to management functions

## Database Location

The SQLite database is created as `app.db` in the project root directory. You can:
- Delete it to reset all data
- Use SQLite browser tools to inspect data
- Backup/restore by copying the file

## Troubleshooting

### Database Issues
If you encounter database errors:
```bash
# Delete the database and migrations
rm app.db
rm -rf Migrations/

# Create fresh migration
export PATH="$PATH:/home/kimo/.dotnet/tools"
dotnet ef migrations add InitialCreate

# Run the app (database will be created automatically)
dotnet run
```

### Port Already in Use
If ports 5000/5001 are in use:
1. Edit `Properties/launchSettings.json`
2. Change the port numbers in the `applicationUrl` settings
3. Or stop the process using the port

### Login Issues
- Ensure email is confirmed (seeded users have `EmailConfirmed = true`)
- Check password requirements (uppercase, lowercase, digit, min 6 chars)
- Default passwords for all seeded users end with `123!`

## Next Steps

1. Test the complete enrollment workflow end-to-end
2. Try rejecting an enrollment at different stages
3. Create new courses as Admin
4. Register new users and assign them roles
5. Test concurrent enrollments from multiple students
6. Verify course capacity limits
7. Check that enrolled count increments only after final approval

Enjoy testing CampusFlow!
