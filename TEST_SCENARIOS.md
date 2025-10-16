# Test Scenarios
## Predefined Test Cases for Student Enrollment System

This document provides specific, reproducible test scenarios to verify all features of the Student Enrollment System.

---

## Test Scenario Index

1. [Scenario 1: Complete Happy Path Enrollment](#scenario-1-complete-happy-path-enrollment)
2. [Scenario 2: Multi-Stage Rejection Testing](#scenario-2-multi-stage-rejection-testing)
3. [Scenario 3: Prerequisite Enforcement](#scenario-3-prerequisite-enforcement)
4. [Scenario 4: Year-Level Progression](#scenario-4-year-level-progression)
5. [Scenario 5: Course Completion and Grading](#scenario-5-course-completion-and-grading)
6. [Scenario 6: Failed Course Recovery](#scenario-6-failed-course-recovery)
7. [Scenario 7: Dean Student Management](#scenario-7-dean-student-management)
8. [Scenario 8: Complete Prospectus Journey](#scenario-8-complete-prospectus-journey)
9. [Scenario 9: Login Portal Validation](#scenario-9-login-portal-validation)
10. [Scenario 10: Concurrent Enrollments](#scenario-10-concurrent-enrollments)

---

## Scenario 1: Complete Happy Path Enrollment

**Objective:** Verify complete enrollment workflow from submission through all 5 approval stages to final enrollment.

**Prerequisites:** Fresh database with seeded data.

### Steps

1. **Start as Student**
   - Login: `alex.wilson@student.edu` / `Student123!`
   - Navigate to "Browse Courses"
   - Click "Enroll" on **CS101 - Intro to Computer Science**
   - Verify requirements show: "✓ All requirements met"
   - Add comment: "My first course!"
   - Click "Submit Enrollment Request"

2. **Verify Initial Status**
   - Go to "My Enrollments"
   - **Expected:** CS101 shows status "DeanReview" (yellow badge)
   - Click "View Details"
   - **Expected:** Shows approval workflow with Dean pending

3. **Dean Approval (Stage 1)**
   - Logout → Login as: `dean@university.edu` / `Dean123!`
   - Dashboard shows: "1" pending approval
   - Click "Approvals" → "Pending Approvals"
   - **Expected:** See Alex Wilson's CS101 request
   - Click "Review"
   - **Expected:** See student details, course info
   - Add comment: "Approved - strong academic record"
   - Click "Approve"
   - **Expected:** Success message "Enrollment approved successfully!"
   - **Expected:** Enrollment no longer in Dean's pending list

4. **Accounting Approval (Stage 2)**
   - Logout → Login as: `accounting@university.edu` / `Accounting123!`
   - Dashboard shows: "1" pending approval
   - Navigate to "Pending Approvals"
   - **Expected:** See Alex Wilson's CS101 request
   - Click "Review" → Add comment: "Financial clearance verified"
   - Click "Approve"
   - **Expected:** Success message

5. **SAO Approval (Stage 3)**
   - Logout → Login as: `sao@university.edu` / `Sao123!`
   - Go to "Pending Approvals"
   - Review and approve with comment: "Student conduct check passed"
   - **Expected:** Success message

6. **Library Approval (Stage 4)**
   - Logout → Login as: `library@university.edu` / `Library123!`
   - Go to "Pending Approvals"
   - Review and approve with comment: "Library access granted"
   - **Expected:** Success message

7. **Records Final Approval (Stage 5)**
   - Logout → Login as: `records@university.edu` / `Records123!`
   - Go to "Pending Approvals"
   - Review and approve with comment: "Officially enrolled in system"
   - **Expected:** Success message

8. **Verify Final Enrollment**
   - Logout → Login as: `alex.wilson@student.edu` / `Student123!`
   - Go to "My Enrollments"
   - **Expected:** CS101 shows status "Enrolled" (green badge)
   - Click "View Details"
   - **Expected:** See complete approval history:
     - All 5 stages completed
     - All approver names shown
     - All comments visible
     - Timestamps for each approval
   - Go to "Dashboard"
   - **Expected:** CS101 appears in recent enrollments

### Success Criteria
- ✅ Enrollment progresses through all 5 stages
- ✅ Each approver sees only their pending items
- ✅ Comments preserved throughout workflow
- ✅ Final status shows "Enrolled"
- ✅ Complete audit trail visible to student

---

## Scenario 2: Multi-Stage Rejection Testing

**Objective:** Verify enrollment can be rejected at any stage and workflow stops immediately.

**Prerequisites:** Fresh database.

### Test 2A: Dean Rejection

1. Login as: `john.doe@student.edu` / `Student123!`
2. Enroll in **MATH201 - Calculus I**
3. Verify status: "DeanReview"
4. Logout → Login as Dean
5. Review enrollment
6. **Select "Reject"**
7. Add comment: "Course full - try next semester"
8. Click "Submit"
9. **Expected:** Success message "Enrollment rejected"
10. Logout → Login as John Doe
11. Go to "My Enrollments"
12. **Expected:** MATH201 shows status "Rejected" (red badge)
13. Click "View Details"
14. **Expected:**
    - Only Dean step shows (no other stages)
    - Dean's rejection comment visible
    - Status: Rejected

### Test 2B: Mid-Workflow Rejection

1. Login as: `jane.smith@student.edu` / `Student123!`
2. Enroll in **ENG101**
3. As Dean: Approve with comment "Approved"
4. As Accounting: Approve with comment "Approved"
5. **As SAO: Reject** with comment "Missing immunization records"
6. Logout → Login as Jane Smith
7. **Expected:** ENG101 status "Rejected"
8. Click "View Details"
9. **Expected:**
    - Dean: Approved ✓
    - Accounting: Approved ✓
    - SAO: Rejected ✗ (with comment)
    - Library: Not processed
    - Records: Not processed

### Test 2C: Re-submission After Rejection

1. From Jane Smith's account (still logged in)
2. Click "Browse Courses"
3. Enroll in ENG101 again
4. Add comment: "Immunization records now uploaded"
5. Submit enrollment
6. **Expected:** New enrollment created with status "DeanReview"
7. **Expected:** Previous rejected enrollment still visible in history

### Success Criteria
- ✅ Rejection stops workflow immediately
- ✅ No further approvals processed
- ✅ Rejection reason visible to student
- ✅ Student can re-submit new request
- ✅ Previous rejections preserved in history

---

## Scenario 3: Prerequisite Enforcement

**Objective:** Verify system prevents enrollment in courses when prerequisites not met.

**Prerequisites:** Fresh database, Alex Wilson (Freshman, no completed courses).

### Test 3A: Direct Prerequisite Blocking

1. Login as: `alex.wilson@student.edu` / `Student123!`
2. Navigate to "Browse Courses"
3. Find **CS201 - Data Structures and Algorithms**
4. Note course card shows: "Prerequisites: CS101"
5. Click "Enroll" on CS201
6. **Expected:** Requirements section shows:
   - "✗ Prerequisites not met: CS101"
   - Red warning badge
7. **Expected:** "Submit Enrollment Request" button is **disabled**

### Test 3B: Chained Prerequisites

1. Still logged in as Alex Wilson
2. Try to enroll in **CS301 - Database Systems**
3. **Expected:** Shows "✗ Prerequisites not met: CS201"
4. **Note:** CS301 requires CS201, which requires CS101
5. Verify two-level prerequisite chain prevents enrollment

### Test 3C: Prerequisite Unlocking

1. Still as Alex Wilson
2. Enroll in **CS101** (no prerequisites)
3. Complete full approval workflow (all 5 stages)
4. Verify status: "Enrolled"
5. Logout → Login as: `admin@university.edu` / `Admin123!`
6. Go to "Admin" → "Grade Students"
7. Click "Manage Grades" on CS101
8. Find Alex Wilson
9. Click "Grade"
10. Select Grade: **A**
11. Select Status: **Completed (Passed)**
12. Click "Save Grade"
13. Logout → Login as Alex Wilson
14. Go to "Browse Courses"
15. Try to enroll in **CS201**
16. **Expected:** Now shows "✓ All requirements met"
17. **Expected:** "Submit Enrollment Request" button is **enabled**
18. Successfully submit enrollment for CS201

### Test 3D: Multiple Prerequisites

1. Create a test course with multiple prerequisites (if needed)
2. **Expected:** All prerequisites must be completed
3. **Expected:** System checks each prerequisite individually

### Success Criteria
- ✅ Cannot enroll in course without prerequisites
- ✅ Enrollment button disabled when prerequisites not met
- ✅ Clear error message shows which courses required
- ✅ Chained prerequisites enforced
- ✅ Prerequisites unlocked immediately after completion
- ✅ Multiple prerequisites all checked

---

## Scenario 4: Year-Level Progression

**Objective:** Test year-level restrictions and student progression.

**Prerequisites:** Fresh database.

### Test 4A: Freshman Restrictions

1. Login as: `alex.wilson@student.edu` / `Student123!` (Freshman)
2. Go to "Browse Courses"
3. View **CS301 - Database Systems** (Junior-level)
4. **Expected:** Shows warning: "Year Level: Junior or higher required"
5. Click "Enroll"
6. **Expected:** Requirements show: "✗ Year level insufficient: Junior required"
7. **Expected:** Button disabled

### Test 4B: Appropriate Level Access

1. Still as Alex Wilson (Freshman)
2. View **CS101** (Freshman-level)
3. **Expected:** No year-level warning
4. **Expected:** Can enroll (button enabled)
5. View **MATH201** (Freshman-level)
6. **Expected:** Accessible to Freshman

### Test 4C: Year-Level Update

1. Logout → Login as: `dean@university.edu` / `Dean123!`
2. Go to "Admin" → "Manage Students"
3. Search for "Alex Wilson"
4. Click "Edit Academics"
5. Change "Year of Study" from **Freshman** to **Junior**
6. Click "Save Changes"
7. **Expected:** Success message
8. Logout → Login as Alex Wilson
9. Go to "My Profile"
10. **Expected:** Year of Study shows "Junior"
11. Go to "Browse Courses"
12. Try to enroll in **CS301**
13. **Expected:** Now accessible (no year-level warning)
14. **Expected:** Can submit enrollment (if prerequisites met)

### Test 4D: Hierarchy Enforcement

1. Create test with all year levels:
   - Freshman can access: Freshman courses only
   - Sophomore can access: Freshman, Sophomore
   - Junior can access: Freshman, Sophomore, Junior
   - Senior can access: All courses
   - Graduate can access: All courses
2. Verify hierarchy works correctly

### Success Criteria
- ✅ Year-level restrictions enforced
- ✅ Clear warning messages
- ✅ Dean can update student year level
- ✅ Changes take effect immediately
- ✅ Year hierarchy properly enforced

---

## Scenario 5: Course Completion and Grading

**Objective:** Test complete grading workflow from enrollment to completion.

**Prerequisites:** John Doe enrolled in CS101 (status: Enrolled).

### Test 5A: Assign Passing Grade

1. Login as: `admin@university.edu` / `Admin123!`
2. Navigate to "Admin" → "Grade Students"
3. **Expected:** See list of active courses
4. Click "Manage Grades" on **CS101**
5. **Expected:** See list of enrolled students
6. Find **John Doe** with status "Enrolled"
7. Click "Grade" button
8. Modal opens
9. Select Grade: **A**
10. Select Status: **Completed (Passed)**
11. Click "Save Grade"
12. **Expected:** Success message "Grade 'A' assigned successfully"
13. **Expected:** John's record now shows:
    - Status: Completed (green badge)
    - Grade: A
    - Graded By: Admin User
    - Graded Date: (today's date)

### Test 5B: Verify in Student Views

1. Logout → Login as: `john.doe@student.edu` / `Student123!`
2. Go to "My Enrollments"
3. **Expected:** CS101 shows:
   - Status: "Completed" (green badge)
   - Grade: "A"
4. Go to "My Profile"
5. Scroll to "Completed Courses" section
6. **Expected:** CS101 appears with grade "A"
7. **Expected:** "Total Credits Earned" increased by 3
8. Go to "My Prospectus"
9. **Expected:** CS101 in "Completed Courses" section (green)
10. **Expected:** Shows grade "A"
11. **Expected:** Progress bar updated

### Test 5C: Prerequisites Unlocked

1. Still in prospectus as John Doe
2. Find **CS201 - Data Structures and Algorithms**
3. **Expected:** Now in "Available Courses" section (was locked before)
4. **Expected:** Can click "Enroll" button

### Success Criteria
- ✅ Admin can assign grades
- ✅ Grade visible in all student views
- ✅ Credits counted toward total
- ✅ Progress bar updated
- ✅ Dependent courses unlocked

---

## Scenario 6: Failed Course Recovery

**Objective:** Test failure handling, re-enrollment, and recovery process.

**Prerequisites:** Jane Smith enrolled in ENG201.

### Test 6A: Assign Failing Grade

1. Login as: `admin@university.edu` / `Admin123!`
2. Go to "Admin" → "Grade Students" → ENG201
3. Find **Jane Smith**
4. Click "Grade"
5. Select Grade: **F**
6. Select Status: **Failed**
7. Click "Save Grade"
8. **Expected:** Success message

### Test 6B: Verify Failed Course Display

1. Logout → Login as: `jane.smith@student.edu` / `Student123!`
2. Go to "My Enrollments"
3. **Expected:** ENG201 shows:
   - Status: "Failed" (red badge)
   - Grade: "F"
4. Go to "My Profile"
5. Scroll to "Failed Courses" section
6. **Expected:** ENG201 appears with grade "F"
7. **Expected:** Total Credits Earned does NOT include ENG201
8. Go to "My Prospectus"
9. **Expected:** ENG201 in "Failed Courses" section (red)
10. **Expected:** Shows grade "F"
11. **Expected:** Shows "Re-enroll" button

### Test 6C: Verify Dependent Courses Stay Locked

1. Still in prospectus
2. If ENG201 has dependent courses, verify they remain locked
3. **Expected:** Failed course does NOT satisfy prerequisites

### Test 6D: Re-enrollment Process

1. Still in prospectus
2. In "Failed Courses" section, find ENG201
3. Click "Re-enroll" button
4. **Expected:** Redirected to enrollment page
5. **Expected:** ENG201 pre-selected
6. Add comment: "Retaking after additional study"
7. Submit enrollment request
8. **Expected:** New enrollment created with status "DeanReview"
9. Go to "My Enrollments"
10. **Expected:** See TWO ENG201 entries:
    - One "Failed" (old)
    - One "DeanReview" (new)

### Test 6E: Successful Completion Second Time

1. Complete approval workflow for new ENG201 enrollment
2. As Admin, assign grade "B" and mark "Completed"
3. Login as Jane Smith
4. Verify prospectus:
   - ENG201 now in "Completed Courses"
   - Shows grade "B"
   - Old failed attempt still in history
   - Credits now counted

### Success Criteria
- ✅ Failed courses clearly marked
- ✅ Do not count toward degree progress
- ✅ Do not unlock dependent courses
- ✅ Re-enrollment option available
- ✅ Can successfully complete on second attempt
- ✅ History preserved

---

## Scenario 7: Dean Student Management

**Objective:** Test Dean's ability to manage student academic information.

**Prerequisites:** Fresh database.

### Test 7A: Search and Filter Students

1. Login as: `dean@university.edu` / `Dean123!`
2. Navigate to "Admin" → "Manage Students"
3. **Expected:** See all 4 students
4. Use search box, type: "Alex"
5. **Expected:** Only Alex Wilson shown
6. Clear search
7. Filter by Department: "Computer Science"
8. **Expected:** Alex Wilson and John Doe shown
9. Filter by Year Level: "Freshman"
10. **Expected:** Only Alex Wilson shown

### Test 7B: View Student Details

1. Click "Edit Academics" for Alex Wilson
2. **Expected:** Left side shows edit form
3. **Expected:** Right side shows:
   - Enrollment History (if any)
   - Academic Summary (completed courses, credits, GPA)
4. Verify current values displayed in form

### Test 7C: Update Academic Information

1. Change values:
   - Student Number: 2025001 (new number)
   - Department: Mathematics
   - Year of Study: Sophomore
   - Major: Computer Science (select from dropdown)
   - Minor: Mathematics (select from dropdown)
   - GPA: 3.95
2. Click "Save Changes"
3. **Expected:** Success message
4. **Expected:** Redirected to student list
5. Search for Alex Wilson again
6. **Expected:** New values displayed in list

### Test 7D: Verify Student Cannot Edit

1. Logout → Login as: `alex.wilson@student.edu` / `Student123!`
2. Go to "My Profile"
3. **Expected:** All updated values displayed
4. Click "Edit Profile"
5. Scroll to "Academic Information" section
6. **Expected:** All academic fields are READ-ONLY (disabled)
7. **Expected:** Info message: "Academic information is managed by your academic advisor"
8. Try to edit personal info (phone, address)
9. **Expected:** Personal fields ARE editable
10. Change phone number
11. Click "Save Changes"
12. **Expected:** Phone updated, academic info unchanged

### Test 7E: Year-Level Change Effects

1. Verify as Alex (now Sophomore):
2. Go to "Browse Courses"
3. **Expected:** Can now enroll in Sophomore courses (CS201, ENG201, BUS201)
4. **Expected:** Still cannot enroll in Junior courses (CS301)

### Success Criteria
- ✅ Dean can search and filter students
- ✅ Dean can update all academic fields
- ✅ Changes take effect immediately
- ✅ Students can see but not edit academic info
- ✅ Year-level changes affect course access

---

## Scenario 8: Complete Prospectus Journey

**Objective:** Test comprehensive prospectus tracking through complete student journey.

**Prerequisites:** John Doe with Computer Science major.

### Test 8A: Initial Prospectus View

1. Login as: `john.doe@student.edu` / `Student123!`
2. Navigate to "My Prospectus"
3. **Expected:** Header shows:
   - Program: Computer Science
   - Department: Computer Science
   - Credits: 0 / 120 (or current)
   - Progress bar with percentage
4. **Expected:** Legend shows 6 categories
5. **Expected:** Summary at bottom shows counts

### Test 8B: Enroll and Track Pending

1. From prospectus, find CS101 in "Available Courses"
2. Click "Enroll" button (direct from prospectus)
3. Complete enrollment submission
4. Return to prospectus
5. **Expected:** CS101 moved to "Pending Approval" section (yellow)
6. **Expected:** Shows current status (e.g., "DeanReview")
7. **Expected:** "View Status" button available

### Test 8C: Progress Through Stages

1. As approvers, approve enrollment through all 5 stages
2. After each approval, login as John and check prospectus
3. **Expected:** Status updates in "Pending Approval" section
4. After final approval (Records):
5. **Expected:** CS101 moves to "Currently Taking" section (blue)
6. **Expected:** Shows as "Enrolled" status

### Test 8D: Complete Course

1. As Admin, assign grade "A" and mark "Completed"
2. Login as John Doe
3. View prospectus
4. **Expected:** CS101 moved to "Completed Courses" section (green)
5. **Expected:** Shows grade "A"
6. **Expected:** Progress bar increased
7. **Expected:** Credits earned updated
8. **Expected:** CS201 moved from "Locked" to "Available" (prerequisite met)

### Test 8E: Submit Multiple Enrollments

1. Enroll in CS201, MATH201, and BUS101 (if allowed)
2. View prospectus
3. **Expected:** All three in "Pending Approval"
4. Approve CS201 and MATH201, reject BUS101
5. View prospectus
6. **Expected:**
   - CS201 and MATH201 in "Pending" or "Currently Taking"
   - BUS101 removed from prospectus (not in program)

### Test 8F: Fail a Course

1. As Admin, mark CS201 as "Failed" with grade "F"
2. Login as John Doe
3. View prospectus
4. **Expected:** CS201 in "Failed Courses" section (red)
5. **Expected:** Shows grade "F"
6. **Expected:** "Re-enroll" button available
7. **Expected:** CS301 still locked (CS201 failed, prerequisite not met)

### Test 8G: Complete Multiple Courses

1. Complete MATH201 with grade "B"
2. Re-enroll and complete CS201 with grade "A"
3. View prospectus
4. **Expected:** Both in "Completed Courses"
5. **Expected:** Progress bar significantly increased
6. **Expected:** Multiple locked courses now available
7. **Expected:** Summary shows accurate counts

### Success Criteria
- ✅ All course states tracked correctly
- ✅ Real-time status updates
- ✅ Progress bar accurate
- ✅ Prerequisites affect availability
- ✅ Failed courses handled properly
- ✅ Can enroll directly from prospectus
- ✅ Summary statistics accurate

---

## Scenario 9: Login Portal Validation

**Objective:** Test dual login portal system and role validation.

**Prerequisites:** Fresh database.

### Test 9A: Student Portal - Correct Role

1. Open browser to: https://localhost:5001
2. Click "Student Login"
3. **Expected:** Blue-themed login page
4. **Expected:** "Student Login" header
5. **Expected:** Test student accounts listed
6. Login with: `john.doe@student.edu` / `Student123!`
7. **Expected:** Successfully logged in
8. **Expected:** Redirected to Dashboard
9. **Expected:** Dashboard shows student features

### Test 9B: Student Portal - Wrong Role

1. Logout
2. Go to home page
3. Click "Student Login"
4. Try to login with: `dean@university.edu` / `Dean123!`
5. **Expected:** Login succeeds initially (credentials valid)
6. **Expected:** Immediately logged out
7. **Expected:** Error message: "This login portal is for students only. Please use the Staff Login."
8. **Expected:** Returned to Student Login page

### Test 9C: Staff Portal - Correct Role

1. From home page, click "Staff Login"
2. **Expected:** Green-themed login page
3. **Expected:** "Staff Login" header
4. **Expected:** Test staff accounts listed
5. Login with: `dean@university.edu` / `Dean123!`
6. **Expected:** Successfully logged in
7. **Expected:** Redirected to Dashboard
8. **Expected:** Dashboard shows Dean-specific features

### Test 9D: Staff Portal - Wrong Role

1. Logout
2. Go to home page
3. Click "Staff Login"
4. Try to login with: `alex.wilson@student.edu` / `Student123!`
5. **Expected:** Login succeeds initially
6. **Expected:** Immediately logged out
7. **Expected:** Error message: "This login portal is for staff only. Please use the Student Login."
8. **Expected:** Returned to Staff Login page

### Test 9E: Portal Switching

1. On Student Login page
2. **Expected:** Footer link: "Staff Login"
3. Click "Staff Login" link
4. **Expected:** Taken to Staff Login page
5. **Expected:** Footer link: "Student Login"
6. Click "Student Login" link
7. **Expected:** Back to Student Login page

### Test 9F: Original Login Page

1. Navigate directly to: https://localhost:5001/Account/Login
2. **Expected:** Still works (original login page)
3. Login with any valid credentials
4. **Expected:** No role validation (accepts any role)
5. **Expected:** Redirected to Dashboard

### Success Criteria
- ✅ Two separate login portals
- ✅ Different themes (blue/green)
- ✅ Role validation enforced
- ✅ Appropriate error messages
- ✅ Can switch between portals
- ✅ Original login still functional

---

## Scenario 10: Concurrent Enrollments

**Objective:** Test multiple students enrolling in same course and approver queue management.

**Prerequisites:** Fresh database.

### Test 10A: Multiple Student Enrollments

1. Login as: `alex.wilson@student.edu`
2. Enroll in **CS101**
3. Logout → Login as: `john.doe@student.edu`
4. Enroll in **CS101**
5. Logout → Login as: `jane.smith@student.edu`
6. Enroll in **CS101**
7. Logout → Login as: `mike.johnson@student.edu`
8. Enroll in **CS101**

### Test 10B: Dean Queue Management

1. Login as: `dean@university.edu`
2. Navigate to "Pending Approvals"
3. **Expected:** See 4 CS101 enrollment requests
4. **Expected:** All show student names and student numbers
5. **Expected:** All show status "DeanReview"
6. Approve Alex Wilson's request
7. **Expected:** Still see 3 requests remaining
8. Reject John Doe's request with comment: "Please see advisor first"
9. **Expected:** Only 2 requests remaining
10. Approve Jane Smith and Mike Johnson

### Test 10C: Next Stage Queue

1. Logout → Login as: `accounting@university.edu`
2. Navigate to "Pending Approvals"
3. **Expected:** See 2 requests (Alex and Jane or Mike)
4. **Expected:** Do NOT see John's rejected request
5. Approve both

### Test 10D: Verify Individual Student Views

1. Login as Alex Wilson
2. **Expected:** CS101 status "SAOReview" (or later)
3. Login as John Doe
4. **Expected:** CS101 status "Rejected"
5. **Expected:** Can see Dean's rejection comment
6. Login as Jane Smith
7. **Expected:** CS101 status "SAOReview" (or later)

### Test 10E: Capacity Tracking

1. Login as Admin
2. Go to "Admin" → "Manage Courses"
3. View CS101
4. **Expected:** Shows "Enrolled: X / 30" (capacity)
5. Complete all approvals for all students
6. **Expected:** Enrolled count increases as each enrollment is completed

### Success Criteria
- ✅ Multiple students can enroll in same course
- ✅ Each approver sees all pending for their stage
- ✅ Rejections don't appear in subsequent stages
- ✅ Each student sees only their enrollment status
- ✅ Course capacity tracked correctly

---

## Quick Regression Test Checklist

Use this for quick system verification:

### Core Features (15 min)
- [ ] Student can login via Student portal
- [ ] Staff can login via Staff portal
- [ ] Student can browse courses
- [ ] Student can submit enrollment
- [ ] Dean can approve enrollment
- [ ] Final Records approval creates "Enrolled" status
- [ ] Student can view approval history

### Academic Features (10 min)
- [ ] Prerequisite blocks enrollment
- [ ] Year-level restricts access
- [ ] Admin can assign grade
- [ ] Completed course appears in profile
- [ ] Failed course shows re-enroll option
- [ ] Prospectus shows all categories

### Admin Features (10 min)
- [ ] Dean can search students
- [ ] Dean can update academic info
- [ ] Admin can create course
- [ ] Admin can manage user roles
- [ ] Dashboard shows accurate statistics

### Edge Cases (5 min)
- [ ] Rejection stops workflow
- [ ] Wrong portal blocks login
- [ ] Cannot edit own academic info
- [ ] Progress bar updates correctly

---

## Reporting Issues

When reporting bugs found during testing, include:
1. Scenario number and step
2. Expected result
3. Actual result
4. Screenshots if applicable
5. Browser and version
6. Any error messages

---

**All scenarios tested = System ready for production!** ✅
