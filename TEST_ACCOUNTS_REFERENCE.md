# Test Accounts Reference Card

Quick reference for all test accounts in the Student Enrollment System.

---

## 🎓 Student Accounts

| Name | Email | Password | Year Level | Department | Major | Special Notes |
|------|-------|----------|------------|------------|-------|---------------|
| **John Doe** | john.doe@student.edu | Student123! | Junior | Computer Science | Computer Science | GPA: 3.75, Minor: Mathematics |
| **Jane Smith** | jane.smith@student.edu | Student123! | Senior | Engineering | Mechanical Engineering | GPA: 3.92, Minor: Physics |
| **Mike Johnson** | mike.johnson@student.edu | Student123! | Sophomore | Business Administration | Business Administration | GPA: 3.50, Minor: Marketing |
| **Alex Wilson** | alex.wilson@student.edu | Student123! | Freshman | Computer Science | Computer Science | GPA: 3.85, New student |

---

## 👥 Staff Accounts

### Admin
| Name | Email | Password | Access Level |
|------|-------|----------|--------------|
| **Admin User** | admin@university.edu | Admin123! | Full system access, all features |

### Approval Workflow Staff
| Name | Role | Email | Password | Approval Stage | Special Permissions |
|------|------|-------|----------|----------------|---------------------|
| **Dean Smith** | Dean | dean@university.edu | Dean123! | Stage 1 (First) | Student management, grading |
| **Accounting Officer** | Accounting | accounting@university.edu | Accounting123! | Stage 2 | Financial clearance |
| **Student Affairs Officer** | SAO | sao@university.edu | Sao123! | Stage 3 | Student conduct review |
| **Library Staff** | Library | library@university.edu | Library123! | Stage 4 | Library clearance |
| **Records Manager** | Records | records@university.edu | Records123! | Stage 5 (Final) | Official enrollment |

---

## 🔑 Password Pattern

All test accounts use the same password pattern:
- **Format:** `[Role]123!`
- **Examples:**
  - Students: `Student123!`
  - Admin: `Admin123!`
  - Dean: `Dean123!`

**Security Note:** These are test accounts only. Change passwords in production.

---

## 🎯 What Each Account Can Test

### Student Accounts

**All Students Can:**
- View and browse courses
- Submit enrollment requests
- Track enrollment status
- View approval history
- Update personal profile information
- View prospectus (curriculum progress)
- Upload profile picture
- View completed courses with grades
- Re-enroll in failed courses

**Students CANNOT:**
- Edit academic information (GPA, major, year level, student number)
- Approve enrollments
- Create or edit courses
- Manage other users

### Staff Accounts

**Dean Can:**
- Approve/reject enrollments (Stage 1)
- View approval queue and history
- Manage student academic information
- Assign grades to students
- Mark courses as completed/failed
- View all students
- Search and filter students

**Accounting, SAO, Library, Records Can:**
- Approve/reject enrollments at their stage
- View approval queue
- View approval history
- Add comments to approvals

**Admin Can:**
- Everything Dean can do, PLUS:
- Create/edit/deactivate courses
- Manage course prerequisites
- Manage users and roles
- View system-wide statistics
- Full system access

---

## 📚 Sample Courses

| Code | Name | Department | Credits | Min Year | Prerequisites |
|------|------|------------|---------|----------|---------------|
| CS101 | Intro to Computer Science | Computer Science | 3 | Freshman | None |
| CS201 | Data Structures & Algorithms | Computer Science | 4 | Sophomore | CS101 |
| CS301 | Database Systems | Computer Science | 3 | Junior | CS201 |
| ENG101 | Intro to Engineering | Engineering | 3 | Freshman | None |
| ENG201 | Thermodynamics | Engineering | 3 | Sophomore | ENG101 |
| BUS101 | Principles of Business | Business Admin | 3 | Freshman | None |
| BUS201 | Marketing Management | Business Admin | 3 | Sophomore | BUS101 |
| MATH201 | Calculus I | Mathematics | 4 | Freshman | None |

---

## 🎨 Login Portals

### Student Portal
- **URL:** Click "Student Login" on home page
- **Theme:** Blue
- **Access:** Student role only
- **Features:** Course browsing, enrollment, prospectus

### Staff Portal
- **URL:** Click "Staff Login" on home page
- **Theme:** Green
- **Access:** All staff roles (Admin, Dean, Accounting, SAO, Library, Records)
- **Features:** Approvals, management, grading

---

## 🔄 Enrollment Workflow Stages

1. **Submitted** → Student submits request
2. **Dean Review** → Dean approves/rejects
3. **Accounting Review** → Accounting approves/rejects
4. **SAO Review** → SAO approves/rejects
5. **Library Review** → Library approves/rejects
6. **Records Review** → Records approves/rejects
7. **Enrolled** → Student officially enrolled
8. **Rejected** → Denied at any stage (workflow ends)

---

## ✅ Quick Test Paths

### Test Complete Approval (Happy Path)
1. Login as **Alex Wilson** (Freshman)
2. Enroll in **CS101**
3. Login as **Dean** → Approve
4. Login as **Accounting** → Approve
5. Login as **SAO** → Approve
6. Login as **Library** → Approve
7. Login as **Records** → Approve
8. Login back as **Alex Wilson** → Verify enrolled

### Test Prerequisite Blocking
1. Login as **Alex Wilson** (no completed courses)
2. Try to enroll in **CS201**
3. ✅ Expected: Blocked with "CS101 required" message

### Test Year-Level Restriction
1. Login as **Alex Wilson** (Freshman)
2. Try to enroll in **CS301** (Junior level)
3. ✅ Expected: Blocked with year level warning

### Test Grading
1. Create/use enrolled course
2. Login as **Admin**
3. Go to Admin → Grade Students
4. Assign grade and mark as Completed
5. Login as student → Verify grade appears

### Test Student Management
1. Login as **Dean**
2. Go to Admin → Manage Students
3. Find **Alex Wilson**
4. Edit academic information (change year level)
5. Login as **Alex** → Verify changes reflected

---

## 🔗 Quick Links (when app is running)

- **Home:** https://localhost:5001
- **Student Login:** https://localhost:5001/Account/StudentLogin
- **Staff Login:** https://localhost:5001/Account/StaffLogin
- **Dashboard:** https://localhost:5001/Dashboard
- **Browse Courses:** https://localhost:5001/Course/Browse
- **My Enrollments:** https://localhost:5001/Enrollment
- **My Profile:** https://localhost:5001/Profile
- **My Prospectus:** https://localhost:5001/Prospectus
- **Pending Approvals:** https://localhost:5001/Dashboard/PendingApprovals
- **Admin - Manage Students:** https://localhost:5001/Admin/ManageStudents
- **Admin - Grade Students:** https://localhost:5001/Admin/ManageCourses

---

## 🆘 Troubleshooting Tips

**Can't Login?**
- Check you're using the correct portal (Student vs Staff)
- Verify email/password (all passwords: `[Role]123!`)
- Try different browser or incognito mode

**Course Shows as Locked?**
- Check prerequisites are met
- Verify student year level is sufficient
- Login as Dean to update year level if needed

**Changes Not Showing?**
- Hard refresh browser (Ctrl+F5 or Cmd+Shift+R)
- Clear browser cache
- Logout and login again

**Reset Everything?**
```bash
rm app.db
dotnet run
```

---

**Print this reference card for quick access during testing!** 📋
