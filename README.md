# Student Enrollment System

A comprehensive ASP.NET Core 9.0 MVC application for managing student course enrollments with a multi-stage approval workflow.

## Features

### Core Workflow
- **Multi-Stage Approval Workflow**: Enrollments go through 5 approval stages:
  1. Dean
  2. Accounting
  3. SAO (Student Affairs Office)
  4. Library
  5. Records

- **Dual Login Portals**: Separate login interfaces for students and staff with role validation

### Academic Features
- **Prerequisites System**: Courses can require completion of other courses before enrollment
- **Year-Level Restrictions**: Courses restricted by student year level (Freshman, Sophomore, Junior, Senior, Graduate)
- **Course Grading**: Admin/Dean can assign grades and mark courses as Completed or Failed
- **Academic Programs**: Define degree programs with required and elective courses
- **Student Prospectus**: Track curriculum progress with visual categorization:
  - ✅ Completed courses (with grades)
  - 📚 Currently taking
  - ⏳ Pending approval
  - ❌ Failed courses (with re-enrollment option)
  - 📖 Available courses
  - 🔒 Locked courses (prerequisites not met)

### Student Management
- **Student Profiles**: Comprehensive profiles with personal, academic, and emergency contact information
- **Profile Picture Upload**: Students can upload and manage profile pictures
- **Academic Information Management**: Dean/Admin can update student academic information (GPA, major, year level, student number)
- **Read-Only Academic Fields**: Students cannot edit their own academic information

### Administration
- **Role-Based Access Control**:
  - **Student**: Browse courses, submit enrollment requests, track status, view prospectus, manage personal profile
  - **Dean**: All approval functions, student management, grade assignment
  - **Accounting/SAO/Library/Records**: Review and approve/reject enrollments at their stage
  - **Admin**: Full system access, user management, course management, grading

- **Course Management**: Create, edit, and manage courses with capacity tracking, prerequisites, and year-level requirements

- **User Management**: Admin can manage user accounts and assign/remove roles

- **Audit Trail**: Complete history of all approval actions with comments

- **Dashboard Analytics**: Role-specific dashboards with statistics and pending items

### User Experience
- **Responsive UI**: Bootstrap 5 based responsive design
- **Icon-Based Navigation**: Intuitive navigation with Bootstrap Icons
- **Progress Tracking**: Visual progress bars for degree completion
- **Status Badges**: Color-coded badges for enrollment and course status

## Technology Stack

- ASP.NET Core 9.0 MVC
- Entity Framework Core 9.0
- SQLite Database (bundled via NuGet - no separate installation required)
- ASP.NET Core Identity
- Bootstrap 5

## Database Setup

**No SQLite installation required!** The SQLite engine is included in the `Microsoft.EntityFrameworkCore.Sqlite` NuGet package.

The application uses SQLite and Entity Framework Core migrations. On first run, the database will be automatically created and seeded with sample data:
- Database file: `app.db` (created in project root)
- Tables are created via EF Core migrations
- Sample data is seeded automatically (7 users + 8 courses)

### Initial Migration

The initial migration has already been created. To apply it manually:

```bash
export PATH="$PATH:/home/kimo/.dotnet/tools"
dotnet ef database update
```

## Running the Application

```bash
dotnet run
```

The application will be available at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

## Default Users

The system is seeded with the following test users:

### Admin User
- **Email**: admin@university.edu
- **Password**: Admin123!
- **Role**: Admin

### Approval Users
- **Dean**: dean@university.edu / Dean123!
- **Accounting**: accounting@university.edu / Accounting123!
- **SAO**: sao@university.edu / SAO123!
- **Library**: library@university.edu / Library123!
- **Records**: records@university.edu / Records123!

### Student Users
- **John Doe** (Junior): john.doe@student.edu / Student123!
  - Student Number: 2024001
  - Department: Computer Science
  - GPA: 3.75, Major: Computer Science, Minor: Mathematics

- **Jane Smith** (Senior): jane.smith@student.edu / Student123!
  - Student Number: 2024002
  - Department: Engineering
  - GPA: 3.92, Major: Mechanical Engineering, Minor: Physics

- **Mike Johnson** (Sophomore): mike.johnson@student.edu / Student123!
  - Student Number: 2024003
  - Department: Business Administration
  - GPA: 3.50, Major: Business Administration, Minor: Marketing

- **Alex Wilson** (Freshman): alex.wilson@student.edu / Student123!
  - Student Number: 2024004
  - Department: Computer Science
  - GPA: 3.85, Major: Computer Science

## Sample Courses

The system includes 8 sample courses with prerequisites and year-level restrictions:

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

### Sample Programs
- **Computer Science** (120 credits): CS101, CS201, CS301, MATH201
- **Mechanical Engineering** (128 credits): ENG101, ENG201, MATH201
- **Business Administration** (120 credits): BUS101, BUS201, MATH201

## Enrollment Workflow

1. **Student** submits an enrollment request for a course
2. Request enters **Dean Review** stage
3. Upon Dean approval, moves to **Accounting Review**
4. Upon Accounting approval, moves to **SAO Review**
5. Upon SAO approval, moves to **Library Review**
6. Upon Library approval, moves to **Records Review**
7. Upon Records approval, student is **Enrolled** in the course

At any stage, the approver can:
- **Approve**: Move to next stage
- **Reject**: End workflow (student must resubmit)
- **Add Comments**: Provide feedback or reasons for decision

## Project Structure

```
StudentEnrollmentSystem/
├── Controllers/
│   ├── AccountController.cs         # Authentication (student/staff login portals)
│   ├── AdminController.cs           # User/role management, student management, grading
│   ├── CourseController.cs          # Course CRUD operations, prerequisites
│   ├── DashboardController.cs       # Role-based dashboards with analytics
│   ├── EnrollmentController.cs      # Student enrollment with validation
│   ├── HomeController.cs            # Landing page with dual portals
│   ├── ProfileController.cs         # Student profile management, picture upload
│   └── ProspectusController.cs      # Curriculum tracking and progress
├── Models/
│   ├── ApplicationUser.cs           # Extended Identity user with academic fields
│   ├── Course.cs                    # Course with prerequisites and year-level
│   ├── CoursePrerequisite.cs        # Many-to-many prerequisites
│   ├── Enrollment.cs                # Enrollment with grade and status
│   ├── ApprovalStep.cs              # Approval history with comments
│   ├── Program.cs                   # Academic program (degree)
│   ├── ProgramCourse.cs             # Program curriculum courses
│   ├── Enums/
│   │   ├── EnrollmentStatus.cs      # Submitted → Enrolled → Completed/Failed
│   │   └── ApprovalStatus.cs        # Approval step statuses
│   └── ViewModels/
│       ├── DashboardViewModel.cs
│       ├── EnrollmentViewModel.cs
│       └── ApprovalViewModel.cs
├── Services/
│   ├── IEnrollmentWorkflowService.cs
│   └── EnrollmentWorkflowService.cs  # Workflow with validation logic
├── Data/
│   ├── ApplicationDbContext.cs      # EF Core context with all relationships
│   └── DbInitializer.cs             # Comprehensive data seeding
└── Views/
    ├── Home/                        # Landing page with dual login cards
    ├── Account/                     # StudentLogin, StaffLogin, Register
    ├── Dashboard/                   # Role-specific dashboards
    ├── Profile/                     # View, Edit, ChangePassword
    ├── Prospectus/                  # Curriculum progress tracker
    ├── Enrollment/                  # Browse, Create, Details, Index
    ├── Course/                      # Index, Create, Edit, Browse
    └── Admin/
        ├── Users.cshtml             # User management
        ├── ManageRoles.cshtml       # Role assignment
        ├── ManageCourses.cshtml     # Course list for grading
        ├── ManageCourseEnrollments.cshtml  # Grade assignment
        ├── ManageStudents.cshtml    # Student list with search
        └── EditStudentAcademics.cshtml  # Edit student academic info
```

## Key Features Implementation

### Workflow Service
The `EnrollmentWorkflowService` manages the sequential approval process:
- Creates enrollment with initial Dean approval step
- Processes approvals and moves to next stage
- Handles rejections
- Tracks complete audit history

### Role-Based Authorization
- Controllers use `[Authorize(Roles = "...")]` attributes
- Views conditionally render based on user roles
- Navigation menu adapts to user's role

### Dashboard
- **Students**: View their enrollments and status
- **Approvers**: See pending approvals requiring their action
- **Admin**: System-wide statistics and management tools

## Development

### Adding a New Approval Stage

To add a new approval stage:

1. Add to `ApprovalStepType` enum in `Models/Enums/ApprovalStepType.cs`
2. Add to `EnrollmentStatus` enum in `Models/Enums/EnrollmentStatus.cs`
3. Update `GetNextStep()` and `GetEnrollmentStatusForStep()` in `EnrollmentWorkflowService.cs`
4. Update role mappings in `GetStepTypeFromRole()` and `GetRoleFromStepType()`
5. Add new role to `DbInitializer.cs`
6. Create and apply database migration

### Database Migrations

Create a new migration:
```bash
dotnet ef migrations add MigrationName
```

Apply migrations:
```bash
dotnet ef database update
```

## Security

- Passwords must meet complexity requirements (uppercase, lowercase, digit, min 6 chars)
- All sensitive operations require authentication
- Role-based authorization prevents unauthorized access
- Anti-forgery tokens protect against CSRF attacks

## Testing Documentation

Comprehensive testing guides are available:
- **CLIENT_TESTING_GUIDE.md** - Step-by-step feature testing guide with scenarios
- **TEST_ACCOUNTS_REFERENCE.md** - Quick reference card for all test accounts
- **TEST_SCENARIOS.md** - Pre-defined test cases
- **QUICK_START.md** - Quick start and basic workflow testing

## Future Enhancements

Potential future features:
- Email notifications for approval status changes and grade assignments
- Bulk enrollment operations for advisors
- Enrollment deadlines and registration periods
- Academic calendar integration
- Advanced reporting and analytics dashboard
- Export enrollment data to CSV/Excel
- Course waitlist management
- Student transcript generation
- Automated prerequisite chain visualization
- Mobile app for notifications

## License

This is a sample educational project.
