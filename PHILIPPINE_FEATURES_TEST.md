# Philippine Features Testing Guide
## Student Enrollment System - Philippine-Specific Features

This focused guide covers testing of all Philippine-specific features implemented in the Student Enrollment System.

---

## Overview of Philippine Features

The system has been adapted for Philippine universities with these key features:

1. **Student Registration Approval Workflow**
   - New students register → Pending status
   - Admin/Dean approves with department selection
   - Automatic student ID generation (YYYY-DEPT-####)
   - Profile completion requirement

2. **Department Management**
   - Philippine university departments (CCS, COE, CBA, etc.)
   - CRUD operations for departments
   - Active/inactive status management

3. **Philippine Address Format**
   - Province (dropdown with Philippine provinces)
   - Municipality/City
   - Barangay (required)
   - Postal Code (4-digit Philippine format)
   - Removed: State and Zip Code (US format)

4. **Automatic Student ID Generation**
   - Format: YYYY-DEPT-####
   - Year-based (2025-)
   - Department-specific sequences
   - Zero-padded 4-digit number (0001, 0002, etc.)

---

## Quick Start for Philippine Features Testing

### Prerequisites
- Application running on http://localhost:5000
- Admin account: `admin@university.edu` / `Admin123!`
- Dean account: `dean@university.edu` / `Dean123!`

### Recommended Test Order
1. Test Department Management (15 minutes)
2. Test Student Registration & Approval (20 minutes)
3. Test Student ID Auto-Generation (10 minutes)
4. Test Philippine Address Format (15 minutes)
5. Test Rejection Workflow (10 minutes)

**Total estimated testing time: 70 minutes**

---

## Test 1: Department Management

### Objective
Verify that admins can create, view, activate, and deactivate Philippine university departments.

### Test Data
- Test Department Name: `College of Communication`
- Test Department Code: `CCOM`
- Test Description: `Mass communication and media programs`

### Steps

#### 1.1 View Existing Departments

1. Login as Admin: `admin@university.edu` / `Admin123!`
2. Navigate to: **Admin → Manage Departments**
3. **Verify:** List shows 8 Philippine departments:
   - ✅ College of Arts and Sciences (CAS)
   - ✅ College of Engineering (COE)
   - ✅ College of Business Administration (CBA)
   - ✅ College of Education (COED)
   - ✅ College of Nursing (CON)
   - ✅ College of Computer Studies (CCS)
   - ✅ College of Criminal Justice (CCJ)
   - ✅ College of Hospitality Management (CHM)
4. **Verify:** Each department shows:
   - ✅ Name, Code, Description
   - ✅ Status badge (Active/Inactive)
   - ✅ Created Date
   - ✅ Actions (Activate/Deactivate button)

#### 1.2 Create New Department

1. Click **"Create New Department"** button
2. Fill in form:
   - Name: `College of Communication`
   - Code: `CCOM`
   - Description: `Mass communication and media programs`
3. Click **"Create Department"**
4. **Verify:** Success message appears
5. **Verify:** New department appears in list
6. **Verify:** Status shows "Active" (green badge)
7. **Verify:** Created Date is today

#### 1.3 Test Unique Code Constraint

1. Click **"Create New Department"** again
2. Fill in form:
   - Name: `Another College`
   - Code: `CCOM` (same as before)
   - Description: `Test duplicate`
3. Click **"Create Department"**
4. **Verify:** ❌ Error message: "A department with this code already exists."
5. **Verify:** Department NOT created

#### 1.4 Deactivate Department

1. Find "College of Communication" in department list
2. Click **"Deactivate"** button
3. Confirm the action
4. **Verify:** Success message
5. **Verify:** Status changed to "Inactive" (gray badge)
6. **Verify:** Button changed to "Activate"

#### 1.5 Verify Inactive Department Not Available

1. Navigate to: **Admin → Pending Registrations**
2. Look at department dropdown
3. **Verify:** "College of Communication" NOT in dropdown
4. **Verify:** Only active departments available

#### 1.6 Reactivate Department

1. Return to: **Admin → Manage Departments**
2. Find "College of Communication"
3. Click **"Activate"** button
4. **Verify:** Status changed to "Active"
5. Go back to **Pending Registrations**
6. **Verify:** "College of Communication" NOW appears in dropdown

### Expected Results
- ✅ 8 Philippine departments pre-seeded
- ✅ Can create new departments
- ✅ Unique code constraint enforced
- ✅ Can activate/deactivate departments
- ✅ Inactive departments excluded from selection dropdowns

---

## Test 2: Student Registration & Approval Workflow

### Objective
Test the complete workflow from student registration through approval to profile completion.

### Test Data
- Email: `maria.santos@student.edu`
- Password: `Test123!`
- First Name: `Maria`
- Last Name: `Santos`
- Department: `College of Computer Studies (CCS)`

### Steps

#### 2.1 Register New Student

1. **Logout** (if logged in)
2. Go to home page
3. Click **"Student Login"**
4. Click **"Register"** link
5. Fill in registration form:
   - Email: `maria.santos@student.edu`
   - Password: `Test123!`
   - Confirm Password: `Test123!`
   - First Name: `Maria`
   - Last Name: `Santos`
6. Click **"Register"**
7. **Verify:** ✅ Success message
8. **Verify:** Automatically logged in

#### 2.2 Verify Pending Status

1. After registration, check current page
2. **Verify:** Redirected to **"Registration Pending Approval"** page
3. **Verify:** Page displays:
   - ✅ Yellow hourglass icon
   - ✅ "Registration Pending Approval" title
   - ✅ Welcome message: "Welcome, Maria Santos!"
   - ✅ Message: "Your registration has been successfully submitted and is currently under review"
   - ✅ Information about what happens next
   - ✅ Expected timeline (1-2 business days)
   - ✅ Logout button

#### 2.3 Verify Limited Access (Pending Student)

1. Try to navigate to: **Browse Courses** (via URL or nav)
2. **Verify:** Redirected back to "Pending Approval" page
3. Try to navigate to: **Dashboard**
4. **Verify:** Redirected back to "Pending Approval" page
5. Try to navigate to: **My Enrollments**
6. **Verify:** Redirected back to "Pending Approval" page
7. **Verify:** ✅ Student cannot access ANY system features while pending

#### 2.4 Admin Views Pending Registrations

1. **Logout**
2. Login as Admin: `admin@university.edu` / `Admin123!`
3. Navigate to: **Admin → Pending Registrations**
4. **Verify:** Maria Santos appears in pending list
5. **Verify:** Shows:
   - ✅ Email: maria.santos@student.edu
   - ✅ Name: Maria Santos
   - ✅ Registration date (today)
   - ✅ Department dropdown (for selection)
   - ✅ Approve button
   - ✅ Reject button

#### 2.5 Approve Student with Department

1. In pending list, find Maria Santos
2. Select Department from dropdown: **College of Computer Studies (CCS)**
3. Click **"Approve"** button
4. **Verify:** ✅ Success message appears
5. **Verify:** Success message shows: "Student Maria Santos approved successfully. Student Number: 2025-CCS-XXXX"
6. **Verify:** Student ID displayed in format: 2025-CCS-0003 (or next sequential)
7. **Verify:** Maria Santos removed from pending list

#### 2.6 Verify Student in Approved List

1. Navigate to: **Admin → Manage Students**
2. Search for: `Maria Santos`
3. **Verify:** Student appears with:
   - ✅ Student ID: 2025-CCS-XXXX
   - ✅ Department: College of Computer Studies
   - ✅ Name: Maria Santos
   - ✅ Email: maria.santos@student.edu

#### 2.7 Profile Completion Requirement

1. **Logout**
2. Login as approved student: `maria.santos@student.edu` / `Test123!`
3. **Verify:** Redirected to **Profile Edit** page
4. **Verify:** Page displays:
   - ✅ Alert: "Please complete your profile to access the system"
   - ✅ Required fields marked with red asterisk (*)
   - ✅ Student ID field shows auto-generated ID (read-only)
   - ✅ Department field shows assigned department (read-only)

#### 2.8 Complete Profile with Philippine Address

1. Fill in all required fields:
   - Phone Number: `+63 917 555 1234`
   - Date of Birth: `01/15/2003`
   - Address: `123 Rizal Street`
   - City/Municipality: `Quezon City`
   - **Barangay:** `Barangay Commonwealth` ⭐ REQUIRED
   - **Province:** Select **Metro Manila** (dropdown) ⭐ REQUIRED
   - Postal Code: `1121`
   - Major: `Computer Science`
   - Emergency Contact Name: `Juan Santos`
   - Emergency Contact Phone: `+63 918 555 5678`
   - Emergency Contact Relationship: `Father`
2. Click **"Save Profile"**
3. **Verify:** ✅ Success message
4. **Verify:** Redirected to Dashboard
5. **Verify:** Navigation menu now accessible
6. **Verify:** Can access all system features

#### 2.9 Verify Profile Completion

1. Navigate to: **My Profile**
2. **Verify:** All information saved correctly
3. **Verify:** Philippine address displayed:
   - ✅ Address: 123 Rizal Street
   - ✅ Barangay: Barangay Commonwealth
   - ✅ City/Municipality: Quezon City
   - ✅ Province: Metro Manila
   - ✅ Postal Code: 1121
4. **Verify:** ✅ NO "State" or "Zip Code" fields visible

### Expected Results
- ✅ New students start with "Pending" status
- ✅ Pending students cannot access system features
- ✅ Admin can view and approve pending registrations
- ✅ Student ID auto-generated on approval
- ✅ Approved students must complete profile
- ✅ Profile completion uses Philippine address format
- ✅ After profile completion, full system access granted

---

## Test 3: Student ID Auto-Generation

### Objective
Verify automatic student ID generation follows correct format and sequence.

### Test Data
- 3 new student registrations with different departments

### Steps

#### 3.1 Register Multiple Students

1. Register and approve **Student 1:**
   - Email: `student1@test.edu`
   - Name: Pedro Cruz
   - Department: **College of Computer Studies (CCS)**
   - **Note the generated ID**

2. Register and approve **Student 2:**
   - Email: `student2@test.edu`
   - Name: Ana Reyes
   - Department: **College of Engineering (COE)**
   - **Note the generated ID**

3. Register and approve **Student 3:**
   - Email: `student3@test.edu`
   - Name: Carlos Garcia
   - Department: **College of Computer Studies (CCS)** (same as Student 1)
   - **Note the generated ID**

#### 3.2 Verify ID Format

1. Check each generated ID
2. **Verify format:** YYYY-DEPT-####
   - ✅ YYYY = Current year (2025)
   - ✅ DEPT = Department code (CCS, COE, CBA, etc.)
   - ✅ #### = 4-digit sequential number (0001, 0002, etc.)

#### 3.3 Verify Expected IDs

**Expected Results:**
- Student 1 (CCS): `2025-CCS-0003` (or next CCS sequence)
- Student 2 (COE): `2025-COE-0002` (or next COE sequence)
- Student 3 (CCS): `2025-CCS-0004` (one more than Student 1)

**Verify:**
- ✅ Each department has independent sequence
- ✅ CCS sequence: ...0003, 0004... (increments)
- ✅ COE sequence: ...0002... (different from CCS)
- ✅ Sequences are zero-padded (0001, not 1)

#### 3.4 Verify Existing Students

1. Navigate to: **Admin → Manage Students**
2. View all students
3. **Verify pre-seeded students have correct IDs:**
   - ✅ John Doe: 2025-CCS-0001
   - ✅ Jane Smith: 2025-COE-0001
   - ✅ Mike Johnson: 2025-CBA-0001
   - ✅ Alex Wilson: 2025-CCS-0002

#### 3.5 Test Sequence Independence

1. Count students by department:
   - CCS students: Check sequence is 0001, 0002, 0003, 0004...
   - COE students: Check sequence is 0001, 0002...
   - CBA students: Check sequence is 0001...
2. **Verify:** Each department maintains its own sequence
3. **Verify:** No gaps in sequences

### Expected Results
- ✅ ID format: YYYY-DEPT-####
- ✅ Year is current year
- ✅ Department code matches selected department
- ✅ Sequential number is 4 digits, zero-padded
- ✅ Each department has independent sequence
- ✅ Sequences increment correctly
- ✅ No duplicate IDs generated

---

## Test 4: Philippine Address Format

### Objective
Verify that all address fields use Philippine format (Province, Municipality, Barangay, Postal Code).

### Steps

#### 4.1 Profile Edit - Philippine Fields

1. Login as any approved student
2. Navigate to: **My Profile → Edit Profile**
3. Scroll to Address section
4. **Verify fields present:**
   - ✅ Address (Street address)
   - ✅ City/Municipality
   - ✅ Barangay (REQUIRED - red asterisk)
   - ✅ Province (REQUIRED dropdown - red asterisk)
   - ✅ Postal Code (optional, 4-digit max)
5. **Verify fields NOT present:**
   - ❌ State (US format - removed)
   - ❌ Zip Code (US format - removed)

#### 4.2 Province Dropdown

1. Click Province dropdown
2. **Verify contains Philippine provinces:**
   - ✅ Metro Manila
   - ✅ Cavite
   - ✅ Laguna
   - ✅ Bulacan
   - ✅ Rizal
   - ✅ Pampanga
   - ✅ Batangas
   - ✅ Cebu
   - ✅ Davao
   - ✅ (and more...)

#### 4.3 Required Field Validation

1. Try to save profile with Barangay empty
2. **Verify:** ❌ Validation error: "Barangay is required"
3. Try to save profile with Province empty
4. **Verify:** ❌ Validation error: "Province is required"
5. Fill Barangay: `Barangay Tondo`
6. Select Province: `Metro Manila`
7. **Verify:** ✅ Saves successfully

#### 4.4 Postal Code Format

1. Enter postal code: `12345` (5 digits)
2. **Verify:** Only 4 digits accepted (maxlength=4)
3. Enter postal code: `1234` (4 digits)
4. **Verify:** ✅ Accepted
5. Leave postal code empty
6. **Verify:** ✅ Saves successfully (postal code is optional)

#### 4.5 View All Students - Address Display

1. Login as Admin
2. Navigate to: **Admin → Manage Students**
3. View different students
4. **Verify each shows Philippine address:**
   - John Doe: Barangay Tatalon, Quezon City, Metro Manila, 1113
   - Jane Smith: Barangay Ermita, Manila, Metro Manila, 1000
   - Mike Johnson: Barangay Poblacion, Makati, Metro Manila, 1200
   - Alex Wilson: Barangay San Antonio, Cavite City, Cavite, 4100

#### 4.6 Edit Student - Admin View

1. Click **"Edit Academics"** on any student
2. View student information panel
3. **Verify:** Address displays in Philippine format
4. **Verify:** No US format fields (State/Zip Code)

### Expected Results
- ✅ Address uses Philippine format
- ✅ Province is dropdown with Philippine provinces
- ✅ Barangay is required field
- ✅ Province is required field
- ✅ Postal Code is 4-digit format (optional)
- ✅ No State or Zip Code fields
- ✅ All students display Philippine addresses

---

## Test 5: Student Rejection Workflow

### Objective
Verify that students can be rejected and see appropriate message.

### Test Data
- Email: `rejected.student@test.edu`
- Rejection Reason: `Incomplete documentation. Please submit proof of high school graduation and birth certificate.`

### Steps

#### 5.1 Register Student for Rejection

1. Logout
2. Register new student account:
   - Email: `rejected.student@test.edu`
   - Password: `Test123!`
   - First Name: `Jose`
   - Last Name: `Dela Cruz`
3. Login and verify pending status

#### 5.2 Reject Student Registration

1. Logout
2. Login as Admin: `admin@university.edu` / `Admin123!`
3. Navigate to: **Admin → Pending Registrations**
4. Find Jose Dela Cruz
5. Click **"Reject"** button
6. Enter rejection reason:
   ```
   Incomplete documentation. Please submit proof of high school graduation and birth certificate.
   ```
7. Click **"Reject Student"**
8. **Verify:** ✅ Success message
9. **Verify:** Student removed from pending list

#### 5.3 Verify Rejection as Student

1. Logout
2. Login as rejected student: `rejected.student@test.edu` / `Test123!`
3. **Verify:** Redirected to **"Registration Rejected"** page
4. **Verify:** Page displays:
   - ✅ Red X icon
   - ✅ "Registration Not Approved" title
   - ✅ Message: "Dear Jose Dela Cruz, We regret to inform you that your registration could not be approved at this time."
   - ✅ Rejection reason displayed:
     - "Incomplete documentation. Please submit proof of high school graduation and birth certificate."
   - ✅ Contact information section:
     - Email: admissions@university.edu
     - Phone: +63 2 1234 5678
     - Office hours
   - ✅ Logout button

#### 5.4 Verify Limited Access (Rejected Student)

1. Try to navigate to other pages
2. **Verify:** Always redirected back to rejection page
3. **Verify:** Cannot access any system features
4. Logout

### Expected Results
- ✅ Admin can reject student registrations
- ✅ Rejection reason is required and saved
- ✅ Rejected students see rejection page
- ✅ Rejection reason displayed to student
- ✅ Contact information provided
- ✅ Rejected students cannot access system

---

## Complete Philippine Features Test Checklist

Use this checklist to track your testing progress:

### Department Management
- [ ] View 8 pre-seeded Philippine departments
- [ ] Create new department
- [ ] Verify unique code constraint
- [ ] Deactivate department
- [ ] Verify inactive department not in dropdowns
- [ ] Reactivate department
- [ ] Verify reactivated department in dropdowns

### Student Registration & Approval
- [ ] Register new student account
- [ ] Verify pending status after registration
- [ ] Verify pending page displays correctly
- [ ] Verify pending student cannot access system
- [ ] Admin views pending registrations
- [ ] Admin approves student with department selection
- [ ] Verify student ID auto-generated and displayed
- [ ] Verify student appears in approved list

### Profile Completion
- [ ] Approved student redirected to profile edit
- [ ] Profile edit shows required fields
- [ ] Complete profile with Philippine address
- [ ] Verify all Philippine address fields present
- [ ] Verify no US address fields (State/Zip Code)
- [ ] After profile completion, verify system access granted

### Student ID Auto-Generation
- [ ] Verify ID format: YYYY-DEPT-####
- [ ] Verify year component is current year
- [ ] Verify department code matches selected department
- [ ] Verify sequential number is 4-digit, zero-padded
- [ ] Verify each department has independent sequence
- [ ] Register multiple students, verify sequences increment
- [ ] Verify no duplicate IDs

### Philippine Address Format
- [ ] Province dropdown with Philippine provinces
- [ ] Barangay is required field
- [ ] Province is required field
- [ ] Postal Code is 4-digit format (optional)
- [ ] Validation errors for missing required fields
- [ ] Address displays correctly in Philippine format
- [ ] All students show Philippine addresses

### Student Rejection
- [ ] Reject student registration with reason
- [ ] Rejected student sees rejection page
- [ ] Rejection reason displayed to student
- [ ] Contact information displayed
- [ ] Rejected student cannot access system

---

## Summary

### Key Philippine Features Implemented

1. **Department Management System**
   - 8 Philippine university departments
   - CRUD operations
   - Active/inactive status

2. **Student Approval Workflow**
   - Registration → Pending → Admin Approval → Student ID → Profile → Access
   - Rejection workflow with reason

3. **Automatic Student ID Generation**
   - Format: 2025-CCS-0001
   - Department-specific sequences
   - Auto-generated on approval

4. **Philippine Address Format**
   - Province, Municipality, Barangay, Postal Code
   - Province dropdown
   - Required Barangay field

### Testing Complete
✅ All Philippine features tested and verified!

---

## Support

For issues or questions during testing:
- See ACCOUNTS.txt for test account credentials
- See CLIENT_TESTING_GUIDE.md for comprehensive feature testing
- See README_FOR_CLIENT.txt for getting started
- Contact development team for support

---

**Happy Testing!** 🇵🇭 🎓
