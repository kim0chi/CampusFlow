# Client Testing Guide
## CampusFlow - Comprehensive Feature Testing

This guide will walk you through testing **all features** of CampusFlow, including enrollment workflows, prerequisite checking, prospectus tracking, grading, and administrative functions.

---

## Table of Contents
1. [Getting Started](#getting-started)
2. [Feature 1: Landing Page & Login Portals](#feature-1-landing-page--login-portals)
3. [Feature 2: Student Enrollment Workflow](#feature-2-student-enrollment-workflow)
4. [Feature 3: Prerequisite System](#feature-3-prerequisite-system)
5. [Feature 4: Year-Level Restrictions](#feature-4-year-level-restrictions)
6. [Feature 5: Student Profile & Prospectus](#feature-5-student-profile--prospectus)
7. [Feature 6: Course Grading System](#feature-6-course-grading-system)
8. [Feature 7: Student Academic Management (Dean)](#feature-7-student-academic-management-dean)
9. [Feature 8: Administrative Functions](#feature-8-administrative-functions)
10. [Feature 9: Philippine Features - NEW](#feature-9-philippine-features---new)
11. [Complete Test Scenarios](#complete-test-scenarios)

---

## Getting Started

### Running the Application

1. **Navigate to project directory:**
   ```bash
   cd /home/kimo/Documents/Code/StudentEnrollmentSystem
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Access in browser:**
   - Open: https://localhost:5001
   - Or: http://localhost:5000

4. **Database:** The SQLite database (`app.db`) is automatically created and seeded on first run.

### Test Accounts Overview

| Role | Email | Password | Purpose |
|------|-------|----------|---------|
| **Students** |
| John Doe (Junior) | john.doe@student.edu | Student123! | Test advanced courses, prospectus |
| Jane Smith (Senior) | jane.smith@student.edu | Student123! | Test senior-level access |
| Mike Johnson (Sophomore) | mike.johnson@student.edu | Student123! | Test sophomore restrictions |
| Alex Wilson (Freshman) | alex.wilson@student.edu | Student123! | Test prerequisite blocking |
| **Staff** |
| Admin | admin@university.edu | Admin123! | Full system access |
| Dean | dean@university.edu | Dean123! | First approval stage, student management |
| Accounting | accounting@university.edu | Accounting123! | Second approval stage |
| SAO | sao@university.edu | Sao123! | Third approval stage |
| Library | library@university.edu | Library123! | Fourth approval stage |
| Records | records@university.edu | Records123! | Final approval stage |

---

## Feature 1: Landing Page & Login Portals

### Purpose
Test the new dual login system that separates student and staff access.

### Steps

1. **View Landing Page:**
   - Open https://localhost:5001
   - You should see two login cards: "Students" and "Department Staff"
   - View the 5-stage approval workflow diagram

2. **Test Student Login Portal:**
   - Click "Student Login" button
   - Notice the blue theme and student-focused messaging
   - You should see test student accounts listed
   - Login with: `alex.wilson@student.edu` / `Student123!`
   - ✅ **Expected:** Successfully logged in, redirected to Dashboard

3. **Test Staff Login Portal:**
   - Logout (top right)
   - Return to home page
   - Click "Staff Login" button
   - Notice the green theme and staff-focused messaging
   - You should see test staff accounts listed
   - Login with: `dean@university.edu` / `Dean123!`
   - ✅ **Expected:** Successfully logged in, redirected to Dashboard

4. **Test Login Portal Validation:**
   - Try logging into Student portal with staff account (`dean@university.edu`)
   - ✅ **Expected:** Error message: "This login portal is for students only. Please use the Staff Login."
   - Try logging into Staff portal with student account (`john.doe@student.edu`)
   - ✅ **Expected:** Error message: "This login portal is for staff only. Please use the Student Login."

---

## Feature 2: Student Enrollment Workflow

### Purpose
Test the complete 5-stage approval workflow from submission to enrollment.

### Steps

#### Part A: Submit Enrollment Request

1. **Login as Student:**
   - Email: `john.doe@student.edu`
   - Password: `Student123!`

2. **Browse Courses:**
   - Click "Browse Courses" in navigation
   - View available courses with details
   - Note: Some courses may show "Prerequisites Not Met" or year-level warnings

3. **Select a Course:**
   - Click "Enroll" on **CS101** (should be available to all students)

4. **Submit Enrollment:**
   - Review course requirements (should show "✓ All requirements met")
   - Add optional comments: "Excited to learn programming!"
   - Click "Submit Enrollment Request"
   - ✅ **Expected:** Success message, redirected to My Enrollments

5. **View Enrollment Status:**
   - Click "My Enrollments" in navigation
   - Find your CS101 enrollment
   - ✅ **Expected:** Status shows "DeanReview" with yellow badge
   - Click "View Details" to see approval progress

#### Part B: Dean Approval

1. **Login as Dean:**
   - Logout and login with `dean@university.edu` / `Dean123!`

2. **View Pending Approvals:**
   - Dashboard shows pending count
   - Click "Approvals" → "Pending Approvals"
   - ✅ **Expected:** See John Doe's CS101 enrollment request

3. **Review and Approve:**
   - Click "Review" on the enrollment
   - View student details, course information
   - Add comment: "Approved - meets academic requirements"
   - Click "Approve"
   - ✅ **Expected:** Success message, status moves to "AccountingReview"

#### Part C: Accounting Approval

1. **Login as Accounting:**
   - Logout and login with `accounting@university.edu` / `Accounting123!`

2. **Approve Request:**
   - Navigate to Pending Approvals
   - Review and approve John Doe's enrollment
   - Add comment: "Financial clearance confirmed"
   - ✅ **Expected:** Status moves to "SAOReview"

#### Part D: SAO Approval

1. **Login as SAO:**
   - Logout and login with `sao@university.edu` / `Sao123!`

2. **Approve Request:**
   - Navigate to Pending Approvals
   - Approve the enrollment
   - Add comment: "Student conduct check passed"
   - ✅ **Expected:** Status moves to "LibraryReview"

#### Part E: Library Approval

1. **Login as Library:**
   - Logout and login with `library@university.edu` / `Library123!`

2. **Approve Request:**
   - Navigate to Pending Approvals
   - Approve the enrollment
   - Add comment: "Library access granted"
   - ✅ **Expected:** Status moves to "RecordsReview"

#### Part F: Final Records Approval

1. **Login as Records:**
   - Logout and login with `records@university.edu` / `Records123!`

2. **Final Approval:**
   - Navigate to Pending Approvals
   - Approve the enrollment
   - Add comment: "Officially enrolled"
   - ✅ **Expected:** Status changes to "Enrolled" (green badge)

#### Part G: Verify Enrollment

1. **Login Back as Student:**
   - Login with `john.doe@student.edu` / `Student123!`

2. **Check Enrollment Status:**
   - Go to "My Enrollments"
   - ✅ **Expected:** CS101 shows status "Enrolled" with green badge
   - Click "View Details"
   - ✅ **Expected:** See complete approval history with all 5 approvers and their comments

3. **Check Dashboard:**
   - View Dashboard
   - ✅ **Expected:** CS101 appears in recent enrollments

---

## Feature 3: Prerequisite System

### Purpose
Test that students cannot enroll in courses without completing prerequisites.

### Course Prerequisites
- **CS201** requires **CS101**
- **CS301** requires **CS201**
- **ENG201** requires **ENG101**
- **BUS201** requires **BUS101**

### Steps

1. **Login as Freshman Student:**
   - Email: `alex.wilson@student.edu` (Freshman, no completed courses)
   - Password: `Student123!`

2. **Try to Enroll in CS201:**
   - Go to "Browse Courses"
   - Click "Enroll" on **CS201 - Data Structures and Algorithms**
   - ✅ **Expected:** Requirements section shows:
     - "✗ Prerequisites not met: CS101"
     - "Enroll" button should be disabled

3. **Check Course Details:**
   - View CS201 details
   - ✅ **Expected:** Prerequisites listed: "CS101"

4. **Try CS301:**
   - Click "Enroll" on **CS301 - Database Systems**
   - ✅ **Expected:** Shows "✗ Prerequisites not met: CS201"

5. **Verify Available Courses:**
   - ✅ **Expected:** Only courses with no prerequisites or met prerequisites show as available:
     - CS101 (no prerequisites)
     - ENG101 (no prerequisites)
     - BUS101 (no prerequisites)
     - MATH201 (no prerequisites)

---

## Feature 4: Year-Level Restrictions

### Purpose
Test that students cannot enroll in courses above their year level.

### Course Year-Level Requirements
- **Freshman courses:** CS101, ENG101, BUS101, MATH201
- **Sophomore courses:** CS201, ENG201, BUS201
- **Junior courses:** CS301

### Steps

1. **Login as Freshman:**
   - Email: `alex.wilson@student.edu`
   - Password: `Student123!`

2. **View Course Browse:**
   - Go to "Browse Courses"
   - Look at CS301 (Junior-level course)
   - ✅ **Expected:** Shows "Year Level: Junior or higher required" warning

3. **Try to Enroll in Junior Course:**
   - Try to enroll in CS301
   - ✅ **Expected:** Requirements show "✗ Year level insufficient: Junior required"

4. **Login as Junior Student:**
   - Logout and login with `john.doe@student.edu` (Junior)
   - Browse courses
   - ✅ **Expected:** CS301 should show as available (if prerequisites met)

5. **Check Sophomore Access:**
   - Login as `mike.johnson@student.edu` (Sophomore)
   - ✅ **Expected:** Can enroll in Sophomore courses (CS201, ENG201, BUS201)
   - ✅ **Expected:** Cannot enroll in Junior courses (CS301)

---

## Feature 5: Student Profile & Prospectus

### Purpose
Test student profile management and curriculum progress tracking.

### Part A: View Student Profile

1. **Login as Student:**
   - Email: `john.doe@student.edu`
   - Password: `Student123!`

2. **Access Profile:**
   - Click "My Profile" in navigation
   - ✅ **Expected:** See complete profile with:
     - Personal information
     - Academic information (Student Number, Department, Year, GPA, Major, Minor)
     - Contact information
     - Emergency contact
     - Course history (Completed, In Progress, Failed)
     - Credits earned
     - Profile completion percentage

3. **Edit Profile:**
   - Click "Edit Profile"
   - Try to edit academic information (Student Number, GPA, Major)
   - ✅ **Expected:** Fields are READ-ONLY with info message:
     - "Academic information is managed by your academic advisor. Please contact the Dean's office to request changes."
   - Edit personal information (Phone, Address, Bio)
   - ✅ **Expected:** Can successfully update personal info

4. **Upload Profile Picture:**
   - In Edit Profile, find Profile Picture section
   - Upload an image (JPG, PNG, or GIF, max 5MB)
   - ✅ **Expected:** Picture uploaded successfully
   - ✅ **Expected:** Picture appears in profile and nav bar

### Part B: View Prospectus

1. **Access Prospectus:**
   - Click "My Prospectus" in navigation
   - ✅ **Expected:** See program overview for student's major (Computer Science)

2. **View Progress Bar:**
   - At top of prospectus
   - ✅ **Expected:** Shows credits earned / total credits required
   - ✅ **Expected:** Progress bar with percentage

3. **View Course Categories:**
   - ✅ **Expected:** See 6 categories with legend:
     - **✅ Completed** (green) - Courses passed with grades
     - **📚 Currently Taking** (blue) - Enrolled courses in progress
     - **⏳ Pending Approval** (yellow) - Enrollment requests being processed
     - **❌ Failed** (red) - Courses failed (can re-enroll)
     - **📖 Available** (teal) - Courses ready to take
     - **🔒 Locked** (gray) - Prerequisites/year-level not met

4. **Check Course Cards:**
   - Each course shows:
     - Course code and name
     - Credits
     - Recommended year/semester
     - Status badge
     - Grade (if completed/failed)
     - Enrollment status (if pending)
     - "Enroll" button (if available)
     - "Re-enroll" button (if failed)

5. **View Summary:**
   - At bottom of prospectus
   - ✅ **Expected:** Summary stats showing count for each category

---

## Feature 6: Course Grading System

### Purpose
Test admin/dean ability to grade students and mark courses as completed or failed.

### Steps

#### Part A: Assign Grades (Admin/Dean)

1. **Login as Admin:**
   - Email: `admin@university.edu`
   - Password: `Admin123!`

2. **Access Grade Management:**
   - Click "Admin" → "Grade Students"
   - ✅ **Expected:** See list of all active courses

3. **Select a Course:**
   - Click "Manage Grades" on CS101
   - ✅ **Expected:** See list of enrolled students with their current status

4. **Assign Grade to Student:**
   - Find John Doe's enrollment (should be "Enrolled" status)
   - Click "Grade" button
   - Modal opens with grade options

5. **Mark as Completed:**
   - Select Grade: **A**
   - Select Status: **Completed (Passed)**
   - Click "Save Grade"
   - ✅ **Expected:** Success message
   - ✅ **Expected:** John's enrollment now shows:
     - Status: "Completed" (green badge)
     - Grade: "A"
     - Graded By: Admin User
     - Graded Date: Today's date

#### Part B: Verify in Student View

1. **Login as Student:**
   - Login with `john.doe@student.edu`

2. **Check My Enrollments:**
   - Go to "My Enrollments"
   - ✅ **Expected:** CS101 shows "Completed" status with grade "A"

3. **Check Profile:**
   - Go to "My Profile"
   - View "Completed Courses" section
   - ✅ **Expected:** CS101 appears with grade "A"
   - ✅ **Expected:** Credits earned increased by 3

4. **Check Prospectus:**
   - Go to "My Prospectus"
   - ✅ **Expected:** CS101 moved to "Completed Courses" section with grade
   - ✅ **Expected:** Progress bar updated
   - ✅ **Expected:** CS201 may now be unlocked (if year level allows)

#### Part C: Mark Course as Failed

1. **Login as Admin:**
   - Create and approve an enrollment for testing (or use existing)

2. **Assign Failing Grade:**
   - Go to Admin → Grade Students → Select course
   - Click "Grade" on a student
   - Select Grade: **F**
   - Select Status: **Failed**
   - Click "Save Grade"
   - ✅ **Expected:** Success message

3. **Verify as Student:**
   - Login as that student
   - Check "My Enrollments"
   - ✅ **Expected:** Course shows "Failed" status with grade "F"
   - Check "My Profile" → "Failed Courses"
   - ✅ **Expected:** Course appears in failed section
   - Check "My Prospectus"
   - ✅ **Expected:** Course appears in "Failed Courses" section with "Re-enroll" button
   - ✅ **Expected:** Course does NOT count toward degree progress
   - ✅ **Expected:** Does NOT unlock dependent courses

#### Part D: Re-enroll After Failure

1. **From Prospectus:**
   - In "Failed Courses" section
   - Click "Re-enroll" button on failed course
   - ✅ **Expected:** Redirected to enrollment page with course pre-selected
   - Submit new enrollment request
   - ✅ **Expected:** New enrollment created, goes through approval workflow again

---

## Feature 7: Student Academic Management (Dean)

### Purpose
Test Dean/Admin ability to manage student academic information.

### Steps

1. **Login as Dean:**
   - Email: `dean@university.edu`
   - Password: `Dean123!`

2. **Access Student Management:**
   - Click "Admin" → "Manage Students"
   - ✅ **Expected:** See list of all students with academic info

3. **Search and Filter:**
   - Use search box to find "Alex Wilson"
   - ✅ **Expected:** List filtered to matching students
   - Filter by Department: "Computer Science"
   - ✅ **Expected:** Only CS students shown
   - Filter by Year Level: "Freshman"
   - ✅ **Expected:** Only freshmen shown

4. **Edit Student Academics:**
   - Click "Edit Academics" for Alex Wilson
   - ✅ **Expected:** See student information and edit form

5. **View Student Summary:**
   - Right panel shows:
     - Enrollment history
     - Academic summary (completed courses, credits, GPA)

6. **Update Academic Information:**
   - Change Student Number to: 2024999
   - Change Department to: Mathematics
   - Change Year of Study to: Sophomore
   - Change Major to: Computer Science (dropdown)
   - Change GPA to: 3.90
   - Click "Save Changes"
   - ✅ **Expected:** Success message

7. **Verify as Student:**
   - Login as `alex.wilson@student.edu`
   - Go to "My Profile"
   - ✅ **Expected:** All updated information displayed
   - ✅ **Expected:** Can now enroll in Sophomore courses
   - Try to edit profile
   - ✅ **Expected:** Academic fields are READ-ONLY with advisor contact message

---

## Feature 8: Administrative Functions

### Purpose
Test admin-only features for system management.

### Part A: User Management

1. **Login as Admin:**
   - Email: `admin@university.edu`
   - Password: `Admin123!`

2. **Access User Management:**
   - Click "Admin" → "Manage Users"
   - ✅ **Expected:** See all registered users

3. **Manage User Roles:**
   - Click "Manage Roles" for any user
   - ✅ **Expected:** See current roles and available roles
   - Add a new role to user
   - ✅ **Expected:** Success message
   - Remove a role
   - ✅ **Expected:** Success message

### Part B: Course Management

1. **Access Course Management:**
   - Click "Admin" → "Manage Courses"
   - ✅ **Expected:** See list of all courses

2. **Create New Course:**
   - Click "Create New Course"
   - Fill in:
     - Course Code: TEST101
     - Name: Test Course
     - Description: For testing purposes
     - Department: Computer Science
     - Credits: 3
     - Capacity: 25
     - Semester: Fall 2025
     - Min Year Level: Freshman
   - Click "Create"
   - ✅ **Expected:** Course created, appears in list

3. **Edit Course:**
   - Click "Edit" on any course
   - Modify details
   - Click "Save"
   - ✅ **Expected:** Changes saved

4. **Manage Prerequisites:**
   - Edit a course
   - Add/remove prerequisites
   - ✅ **Expected:** Prerequisites saved
   - ✅ **Expected:** Students see updated requirements

5. **Deactivate Course:**
   - Edit course and uncheck "Is Active"
   - Save
   - ✅ **Expected:** Course no longer appears in student browse

### Part C: Dashboard Overview

1. **View Admin Dashboard:**
   - Go to Dashboard
   - ✅ **Expected:** See statistics:
     - Total Enrollments
     - Pending Enrollments
     - Completed Enrollments
     - Total Courses

2. **Quick Actions:**
   - ✅ **Expected:** See 4 quick action buttons:
     - Manage Courses
     - Grade Students
     - Manage Students
     - Manage Users

---

## Feature 9: Philippine Features - NEW

### Purpose
Test Philippine-specific features including student registration approval workflow, department management, automatic student ID generation, and Philippine address format.

### Part A: Student Registration & Approval Workflow

#### Register New Student

1. **Logout** (if logged in)
   - Go to home page

2. **Register New Student Account:**
   - Click "Student Login"
   - Click "Register" link
   - Fill in registration form:
     - Email: `testuser@student.edu`
     - Password: `Test123!`
     - Confirm Password: `Test123!`
     - First Name: `Maria`
     - Last Name: `Santos`
   - Click "Register"
   - ✅ **Expected:** Account created successfully

3. **Login with New Account:**
   - Login with `testuser@student.edu` / `Test123!`
   - ✅ **Expected:** Redirected to "Registration Pending Approval" page
   - ✅ **Expected:** Page shows:
     - Yellow hourglass icon
     - Welcome message with full name
     - Status: "Registration Pending Approval"
     - Explanation of approval process
     - Information about what happens next

4. **Verify Limited Access:**
   - Try to navigate to other pages
   - ✅ **Expected:** Always redirected back to pending approval page
   - ✅ **Expected:** Cannot access courses, enrollments, or other features

#### Admin/Dean Approval

5. **Login as Admin:**
   - Logout and login with `admin@university.edu` / `Admin123!`

6. **View Pending Registrations:**
   - Click "Admin" → "Pending Registrations"
   - ✅ **Expected:** See Maria Santos in pending list
   - ✅ **Expected:** Shows email, name, registration date
   - ✅ **Expected:** Department dropdown available for selection

7. **Approve Student:**
   - Select Department: **College of Computer Studies (CCS)**
   - Click "Approve" button
   - ✅ **Expected:** Success message appears
   - ✅ **Expected:** Student ID auto-generated (format: 2025-CCS-0003 or similar)
   - ✅ **Expected:** Success message shows generated student ID
   - ✅ **Expected:** Student removed from pending list

8. **Verify Department Assignment:**
   - Click "Admin" → "Manage Students"
   - Search for "Maria Santos"
   - ✅ **Expected:** Student appears with:
     - Student ID: 2025-CCS-XXXX
     - Department: College of Computer Studies
     - Status: Approved

#### Profile Completion Requirement

9. **Login as Approved Student:**
   - Logout and login with `testuser@student.edu` / `Test123!`
   - ✅ **Expected:** Redirected to Profile Edit page
   - ✅ **Expected:** Alert message: "Please complete your profile to access the system"
   - ✅ **Expected:** Required fields marked with red asterisk (*)

10. **Complete Profile with Philippine Address:**
    - Fill in all required fields:
      - Phone Number: `+63 917 555 1234`
      - Date of Birth: `01/15/2003`
      - Address: `123 Main Street`
      - City/Municipality: `Quezon City`
      - Barangay: `Barangay Commonwealth` (REQUIRED)
      - Province: Select **Metro Manila** (REQUIRED)
      - Postal Code: `1121`
      - Major: `Computer Science`
      - Emergency Contact Name: `Juan Santos`
      - Emergency Contact Phone: `+63 918 555 5678`
      - Emergency Contact Relationship: `Father`
    - Click "Save Profile"
    - ✅ **Expected:** Success message
    - ✅ **Expected:** Redirected to Dashboard
    - ✅ **Expected:** Full system access now available

11. **Verify Philippine Address Display:**
    - Go to "My Profile"
    - ✅ **Expected:** Address displays in Philippine format:
      - Barangay: Barangay Commonwealth
      - City/Municipality: Quezon City
      - Province: Metro Manila
      - Postal Code: 1121
    - ✅ **Expected:** No "State" or "Zip Code" fields

### Part B: Department Management

#### View Departments

1. **Login as Admin or Dean:**
   - Email: `admin@university.edu` / `Admin123!`

2. **Access Department Management:**
   - Click "Admin" → "Manage Departments"
   - ✅ **Expected:** See list of Philippine university departments:
     - College of Arts and Sciences (CAS)
     - College of Engineering (COE)
     - College of Business Administration (CBA)
     - College of Education (COED)
     - College of Nursing (CON)
     - College of Computer Studies (CCS)
     - College of Criminal Justice (CCJ)
     - College of Hospitality Management (CHM)

3. **View Department Details:**
   - Each department shows:
     - Name
     - Code (e.g., CCS, COE, CBA)
     - Description
     - Status (Active/Inactive badge)
     - Created Date
     - Actions (Activate/Deactivate button)

#### Create New Department

4. **Create Department:**
   - Click "Create New Department" button
   - Fill in form:
     - Name: `College of Communication`
     - Code: `CCOM`
     - Description: `Mass communication and media programs`
   - Click "Create Department"
   - ✅ **Expected:** Success message
   - ✅ **Expected:** New department appears in list
   - ✅ **Expected:** Status is "Active" (green badge)

5. **Verify Unique Code Constraint:**
   - Try to create another department with code "CCS"
   - ✅ **Expected:** Error message: "A department with this code already exists"

#### Deactivate/Activate Department

6. **Deactivate Department:**
   - Find the newly created "College of Communication"
   - Click "Deactivate" button
   - Confirm action
   - ✅ **Expected:** Success message
   - ✅ **Expected:** Status changed to "Inactive" (gray badge)
   - ✅ **Expected:** Button changed to "Activate"

7. **Verify Inactive Department:**
   - Logout and login as Admin
   - Go to "Pending Registrations"
   - Check department dropdown
   - ✅ **Expected:** Inactive departments NOT shown in dropdown
   - ✅ **Expected:** Only active departments available for selection

8. **Reactivate Department:**
   - Go back to "Manage Departments"
   - Click "Activate" on College of Communication
   - ✅ **Expected:** Status changed back to "Active"
   - ✅ **Expected:** Now appears in department selection dropdowns

### Part C: Student ID Auto-Generation

#### Test ID Format

1. **Register Multiple Students:**
   - Register 3 new students (following Part A registration steps)
   - Approve each with different departments:
     - Student 1: CCS (Computer Studies)
     - Student 2: COE (Engineering)
     - Student 3: CCS (Computer Studies)

2. **Verify Student ID Format:**
   - ✅ **Expected Student IDs:**
     - Student 1: `2025-CCS-0003` (year-dept-sequence)
     - Student 2: `2025-COE-0002` (different department, different sequence)
     - Student 3: `2025-CCS-0004` (same department, incremented)

3. **Verify ID Components:**
   - ✅ **Year:** Current year (2025)
   - ✅ **Department Code:** 3-4 letter code (CCS, COE, etc.)
   - ✅ **Sequence:** 4-digit number, zero-padded (0001, 0002, etc.)
   - ✅ **Format:** YYYY-DEPT-####

#### Test Department-Specific Sequences

4. **Check Sequence Independence:**
   - View all students in "Manage Students"
   - ✅ **Expected:** Each department has independent sequence:
     - CCS: 0001, 0002, 0003, 0004...
     - COE: 0001, 0002...
     - CBA: 0001, 0002...
   - ✅ **Expected:** Sequences don't interfere with each other

5. **Verify Existing Students:**
   - Check pre-seeded students have correct IDs:
     - John Doe: `2025-CCS-0001`
     - Jane Smith: `2025-COE-0001`
     - Mike Johnson: `2025-CBA-0001`
     - Alex Wilson: `2025-CCS-0002`

### Part D: Student Rejection Workflow

1. **Register Another Student:**
   - Follow registration steps to create account
   - Email: `rejected@student.edu`

2. **Reject Registration:**
   - Login as Admin
   - Go to "Pending Registrations"
   - Find the new student
   - Click "Reject" button
   - Enter rejection reason: `Incomplete documentation. Please submit proof of high school graduation.`
   - Click "Reject Student"
   - ✅ **Expected:** Success message
   - ✅ **Expected:** Student removed from pending list

3. **Verify Rejection as Student:**
   - Logout and login with `rejected@student.edu`
   - ✅ **Expected:** Redirected to "Registration Rejected" page
   - ✅ **Expected:** Page shows:
     - Red X icon
     - "Registration Not Approved" title
     - Rejection message
     - Rejection reason displayed
     - Contact information (email, phone, office hours)
     - Logout button

4. **Verify Limited Access:**
   - Try to navigate to other pages
   - ✅ **Expected:** Always redirected back to rejected page
   - ✅ **Expected:** Cannot access any system features

### Part E: Philippine Address Testing

1. **Login as Any Student:**
   - Use approved student account

2. **Edit Profile - Province Dropdown:**
   - Go to "My Profile" → "Edit Profile"
   - Find Province field
   - ✅ **Expected:** Dropdown with Philippine provinces:
     - Metro Manila
     - Cavite
     - Laguna
     - Bulacan
     - Rizal
     - Pampanga
     - Batangas
     - (and more...)

3. **Test Required Fields:**
   - Try to save profile without Barangay
   - ✅ **Expected:** Validation error
   - Try to save without Province
   - ✅ **Expected:** Validation error
   - ✅ **Expected:** Barangay and Province are required fields (marked with *)

4. **Test Postal Code Format:**
   - Enter postal code: `1234`
   - ✅ **Expected:** Accepted (4-digit Philippine postal code)
   - Try to enter more than 4 digits
   - ✅ **Expected:** Limited to 4 characters (maxlength validation)

5. **View All Students - Philippine Addresses:**
   - Login as Admin
   - Go to "Manage Students"
   - View different students
   - ✅ **Expected:** All students show Philippine addresses:
     - John Doe: Barangay Tatalon, Quezon City, Metro Manila, 1113
     - Jane Smith: Barangay Ermita, Manila, Metro Manila, 1000
     - Mike Johnson: Barangay Poblacion, Makati, Metro Manila, 1200
     - Alex Wilson: Barangay San Antonio, Cavite City, Cavite, 4100

---

## Complete Test Scenarios

### Scenario 1: Complete Happy Path
**Goal:** Test a student from registration through course completion

1. Register new student account
2. Login and complete profile
3. Browse courses and enroll in CS101
4. As each approver (Dean → Accounting → SAO → Library → Records), approve enrollment
5. Verify enrollment status changed to "Enrolled"
6. As Admin, assign grade "A" and mark "Completed"
7. Login as student and verify:
   - CS101 in completed courses with grade
   - Credits earned increased
   - Prospectus updated
   - CS201 now available (prerequisite met)

**✅ Success Criteria:** Student can see completed course with grade, progress updated, next course unlocked

---

### Scenario 2: Rejection Workflow
**Goal:** Test enrollment rejection at different stages

1. As student, submit enrollment request
2. As Dean, reject with comment "Missing prerequisites documentation"
3. Verify as student:
   - Status shows "Rejected"
   - Dean's comment visible
   - No further approvals happened
4. Re-submit enrollment
5. Dean approves, but Accounting rejects
6. ✅ **Success Criteria:** Workflow stops at rejection, comments preserved, student can re-submit

---

### Scenario 3: Prerequisite Enforcement
**Goal:** Verify prerequisite system prevents invalid enrollments

1. As Freshman student with no completed courses
2. Try to enroll in CS201
3. ✅ **Success Criteria:** System blocks enrollment, shows "CS101 required"
4. Enroll and complete CS101 (admin mark as completed)
5. Try CS201 again
6. ✅ **Success Criteria:** Now allowed to enroll

---

### Scenario 4: Year-Level Progression
**Goal:** Test year-level restrictions and updates

1. As Freshman, verify cannot access Junior courses
2. As Dean, update student year level to Junior
3. Login as student
4. ✅ **Success Criteria:** Can now enroll in Junior courses (CS301)

---

### Scenario 5: Failed Course Recovery
**Goal:** Test failure handling and re-enrollment

1. Student enrolled in course
2. Admin marks as Failed with grade "F"
3. Verify as student:
   - Shows in Failed Courses
   - Appears in Prospectus with "Re-enroll" button
   - Does not count toward degree
4. Re-enroll from Prospectus
5. Complete successfully this time
6. ✅ **Success Criteria:** Second attempt successful, progress updated

---

### Scenario 6: Dean Student Management
**Goal:** Test academic information management

1. As Dean, search for student
2. Edit academic information (year level, GPA, major)
3. Verify as student:
   - Profile shows updated info
   - Academic fields read-only
   - Access to courses adjusted based on year level
4. ✅ **Success Criteria:** Dean changes reflected, student cannot edit

---

### Scenario 7: Prospectus Tracking
**Goal:** Test complete curriculum progress tracking

1. As student with major assigned
2. View prospectus showing all program courses
3. Complete some courses (admin marks completed)
4. Submit enrollments for others (pending)
5. Have some in-progress (enrolled status)
6. Verify prospectus shows:
   - Completed with grades
   - In progress
   - Pending approval with status
   - Available (prerequisites met)
   - Locked (prerequisites not met)
7. ✅ **Success Criteria:** All courses categorized correctly, progress bar accurate

---

### Scenario 8: Philippine Registration & Approval Workflow - NEW
**Goal:** Test complete Philippine features from registration to system access

1. Register new student account with Philippine name
2. Login - verify redirected to "Pending Approval" page
3. Verify cannot access system features while pending
4. As Admin, view pending registrations
5. Select department and approve student
6. Verify student ID auto-generated (YYYY-DEPT-####)
7. Login as approved student
8. Verify redirected to profile completion
9. Complete profile with Philippine address (Province, Municipality, Barangay, Postal Code)
10. Verify full system access granted
11. ✅ **Success Criteria:** Complete workflow from registration → approval → ID generation → profile completion → system access

---

### Scenario 9: Department Management - NEW
**Goal:** Test department CRUD operations

1. As Admin, view department list
2. Create new Philippine university department
3. Verify appears in active departments
4. Deactivate department
5. Verify not available in pending registration dropdown
6. Reactivate department
7. Verify available again
8. Try to create duplicate department code
9. ✅ **Success Criteria:** Verify unique constraint enforced
10. ✅ **Success Criteria:** Active/inactive status affects availability

---

## Troubleshooting

### Reset Database
If you need to start fresh:
```bash
cd /home/kimo/Documents/Code/StudentEnrollmentSystem
rm app.db
dotnet run
```
Database will be recreated with seed data.

### Common Issues

**Problem:** Can't login
- **Solution:** Check email/password (all passwords end with `123!`)
- Ensure using correct portal (Student vs Staff)

**Problem:** Course shows as locked
- **Solution:** Check prerequisites and year level requirements
- Verify student year level in profile

**Problem:** Changes not appearing
- **Solution:** Refresh browser page (Ctrl+F5)
- Check if logged in as correct user

---

## Summary Checklist

Use this checklist to verify all features tested:

### Core Features
- [ ] Separate student and staff login portals
- [ ] Complete 5-stage enrollment workflow (Dean → Accounting → SAO → Library → Records)
- [ ] Enrollment rejection at any stage
- [ ] Prerequisite checking blocks invalid enrollments
- [ ] Year-level restrictions enforced
- [ ] Student profile with read-only academic info
- [ ] Profile picture upload
- [ ] Prospectus shows all course categories
- [ ] Admin/Dean can assign grades
- [ ] Mark courses as Completed with grades
- [ ] Mark courses as Failed
- [ ] Failed courses show "Re-enroll" option
- [ ] Dean can manage student academic information
- [ ] Students cannot edit academic info
- [ ] Admin can create/edit courses
- [ ] Admin can manage users and roles
- [ ] Dashboard statistics accurate
- [ ] Navigation adapts to user role

### Philippine Features - NEW
- [ ] Student registration creates "Pending" status
- [ ] Pending students see "Pending Approval" page
- [ ] Pending students cannot access system features
- [ ] Admin/Dean can view pending registrations
- [ ] Admin/Dean can approve students with department selection
- [ ] Student ID auto-generated on approval (YYYY-DEPT-####)
- [ ] Auto-generated ID shows in success message
- [ ] Each department has independent ID sequence
- [ ] Approved students redirected to profile completion
- [ ] Profile completion required before system access
- [ ] Philippine address fields (Province, Municipality, Barangay, Postal Code)
- [ ] Province dropdown with Philippine provinces
- [ ] Barangay is required field
- [ ] No State or Zip Code fields visible
- [ ] Admin can create/manage departments
- [ ] Department unique code constraint enforced
- [ ] Departments can be activated/deactivated
- [ ] Inactive departments not shown in dropdowns
- [ ] 8 Philippine university departments seeded
- [ ] Student rejection workflow with reason
- [ ] Rejected students see rejection page with reason
- [ ] Rejected students cannot access system

---

## Next Steps

After testing all features:
1. Report any bugs or unexpected behavior
2. Provide feedback on user experience
3. Suggest additional features or improvements
4. Test with real-world scenarios specific to your institution

---

**System Ready for Testing!** 🎓

For questions or issues during testing, refer to the Quick Start Guide or README.md for additional information.
