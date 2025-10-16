using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Data;

public static class DbInitializer
{
    public static async Task Initialize(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Ensure database is created
        await context.Database.MigrateAsync();

        // Seed Roles
        string[] roleNames = { "Admin", "Dean", "Accounting", "SAO", "Library", "Records", "Student", "Cashier", "CampusDirector" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Seed Departments (Philippine Universities)
        if (!await context.Departments.AnyAsync())
        {
            var departments = new List<Models.Department>
            {
                new Models.Department { Name = "College of Arts and Sciences", Code = "CAS", Description = "Liberal arts, sciences, and humanities programs", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Models.Department { Name = "College of Engineering", Code = "COE", Description = "Engineering and technology programs", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Models.Department { Name = "College of Business Administration", Code = "CBA", Description = "Business, management, and entrepreneurship programs", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Models.Department { Name = "College of Education", Code = "COED", Description = "Teacher education and pedagogy programs", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Models.Department { Name = "College of Nursing", Code = "CON", Description = "Nursing and healthcare programs", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Models.Department { Name = "College of Computer Studies", Code = "CCS", Description = "Computer science and information technology programs", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Models.Department { Name = "College of Criminal Justice", Code = "CCJ", Description = "Criminology and law enforcement programs", IsActive = true, CreatedDate = DateTime.UtcNow },
                new Models.Department { Name = "College of Hospitality Management", Code = "CHM", Description = "Hotel, restaurant, and tourism management programs", IsActive = true, CreatedDate = DateTime.UtcNow }
            };

            await context.Departments.AddRangeAsync(departments);
            await context.SaveChangesAsync();
        }

        // Seed Admin User
        var adminEmail = "admin@university.edu";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Seed Dean User
        var deanEmail = "dean@university.edu";
        if (await userManager.FindByEmailAsync(deanEmail) == null)
        {
            var deanUser = new ApplicationUser
            {
                UserName = deanEmail,
                Email = deanEmail,
                FirstName = "Dean",
                LastName = "Smith",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(deanUser, "Dean123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(deanUser, "Dean");
            }
        }

        // Seed Accounting User
        var accountingEmail = "accounting@university.edu";
        if (await userManager.FindByEmailAsync(accountingEmail) == null)
        {
            var accountingUser = new ApplicationUser
            {
                UserName = accountingEmail,
                Email = accountingEmail,
                FirstName = "Accounting",
                LastName = "Officer",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(accountingUser, "Accounting123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(accountingUser, "Accounting");
            }
        }

        // Seed SAO User
        var saoEmail = "sao@university.edu";
        if (await userManager.FindByEmailAsync(saoEmail) == null)
        {
            var saoUser = new ApplicationUser
            {
                UserName = saoEmail,
                Email = saoEmail,
                FirstName = "Student Affairs",
                LastName = "Officer",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(saoUser, "Sao123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(saoUser, "SAO");
            }
        }

        // Seed Library User
        var libraryEmail = "library@university.edu";
        if (await userManager.FindByEmailAsync(libraryEmail) == null)
        {
            var libraryUser = new ApplicationUser
            {
                UserName = libraryEmail,
                Email = libraryEmail,
                FirstName = "Library",
                LastName = "Staff",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(libraryUser, "Library123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(libraryUser, "Library");
            }
        }

        // Seed Records User
        var recordsEmail = "records@university.edu";
        if (await userManager.FindByEmailAsync(recordsEmail) == null)
        {
            var recordsUser = new ApplicationUser
            {
                UserName = recordsEmail,
                Email = recordsEmail,
                FirstName = "Records",
                LastName = "Manager",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(recordsUser, "Records123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(recordsUser, "Records");
            }
        }

        // Seed Cashier User
        var cashierEmail = "cashier@university.edu";
        if (await userManager.FindByEmailAsync(cashierEmail) == null)
        {
            var cashierUser = new ApplicationUser
            {
                UserName = cashierEmail,
                Email = cashierEmail,
                FirstName = "Maria",
                LastName = "Santos",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(cashierUser, "Cashier123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(cashierUser, "Cashier");
            }
        }

        // Seed Campus Director User
        var directorEmail = "campusdirector@university.edu";
        if (await userManager.FindByEmailAsync(directorEmail) == null)
        {
            var directorUser = new ApplicationUser
            {
                UserName = directorEmail,
                Email = directorEmail,
                FirstName = "Dr. Roberto",
                LastName = "Cruz",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(directorUser, "Director123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(directorUser, "CampusDirector");
            }
        }

        // Seed Student Users
        // Get department IDs for students
        var ccsDept = await context.Departments.FirstOrDefaultAsync(d => d.Code == "CCS");
        var coeDept = await context.Departments.FirstOrDefaultAsync(d => d.Code == "COE");
        var cbaDept = await context.Departments.FirstOrDefaultAsync(d => d.Code == "CBA");

        var studentEmail1 = "john.doe@student.edu";
        if (await userManager.FindByEmailAsync(studentEmail1) == null)
        {
            var studentUser = new ApplicationUser
            {
                UserName = studentEmail1,
                Email = studentEmail1,
                FirstName = "John",
                LastName = "Doe",
                StudentNumber = "2025-CCS-0001",
                DepartmentId = ccsDept?.Id,
                PhoneNumber = "+63 917 123 4567",
                DateOfBirth = new DateTime(2002, 5, 15),
                Address = "123 Mabini Street",
                City = "Quezon City",
                Province = "Metro Manila",
                Barangay = "Barangay Tatalon",
                PostalCode = "1113",
                Bio = "Computer Science student passionate about software development and artificial intelligence.",
                YearOfStudy = "Junior",
                GPA = 3.75m,
                Major = "Computer Science",
                Minor = "Mathematics",
                EmergencyContactName = "Jane Doe",
                EmergencyContactPhone = "+63 917 987 6543",
                EmergencyContactRelationship = "Mother",
                EmailConfirmed = true,
                ApprovalStatus = "Approved",
                IsProfileCompleted = true
            };

            var result = await userManager.CreateAsync(studentUser, "Student123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(studentUser, "Student");
            }
        }

        var studentEmail2 = "jane.smith@student.edu";
        if (await userManager.FindByEmailAsync(studentEmail2) == null)
        {
            var studentUser = new ApplicationUser
            {
                UserName = studentEmail2,
                Email = studentEmail2,
                FirstName = "Jane",
                LastName = "Smith",
                StudentNumber = "2025-COE-0001",
                DepartmentId = coeDept?.Id,
                PhoneNumber = "+63 918 234 5678",
                DateOfBirth = new DateTime(2001, 8, 22),
                Address = "456 Rizal Avenue",
                City = "Manila",
                Province = "Metro Manila",
                Barangay = "Barangay Ermita",
                PostalCode = "1000",
                Bio = "Mechanical Engineering student interested in robotics and renewable energy.",
                YearOfStudy = "Senior",
                GPA = 3.92m,
                Major = "Mechanical Engineering",
                Minor = "Physics",
                EmergencyContactName = "Robert Smith",
                EmergencyContactPhone = "+63 918 876 5432",
                EmergencyContactRelationship = "Father",
                EmailConfirmed = true,
                ApprovalStatus = "Approved",
                IsProfileCompleted = true
            };

            var result = await userManager.CreateAsync(studentUser, "Student123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(studentUser, "Student");
            }
        }

        var studentEmail3 = "mike.johnson@student.edu";
        if (await userManager.FindByEmailAsync(studentEmail3) == null)
        {
            var studentUser = new ApplicationUser
            {
                UserName = studentEmail3,
                Email = studentEmail3,
                FirstName = "Mike",
                LastName = "Johnson",
                StudentNumber = "2025-CBA-0001",
                DepartmentId = cbaDept?.Id,
                PhoneNumber = "+63 919 345 6789",
                DateOfBirth = new DateTime(2003, 3, 10),
                Address = "789 Bonifacio Street",
                City = "Makati",
                Province = "Metro Manila",
                Barangay = "Barangay Poblacion",
                PostalCode = "1200",
                Bio = "Business Administration student focusing on entrepreneurship and marketing strategies.",
                YearOfStudy = "Sophomore",
                GPA = 3.50m,
                Major = "Business Administration",
                Minor = "Marketing",
                EmergencyContactName = "Sarah Johnson",
                EmergencyContactPhone = "+63 919 765 4321",
                EmergencyContactRelationship = "Mother",
                EmailConfirmed = true,
                ApprovalStatus = "Approved",
                IsProfileCompleted = true
            };

            var result = await userManager.CreateAsync(studentUser, "Student123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(studentUser, "Student");
            }
        }

        // Seed Courses
        if (!context.Courses.Any())
        {
            var courses = new[]
            {
                new Course
                {
                    CourseCode = "CS101",
                    Name = "Introduction to Computer Science",
                    Description = "Foundational concepts of computer science and programming",
                    Credits = 3,
                    Department = "Computer Science",
                    Semester = "First Semester 2025",
                    Capacity = 30,
                    MinimumYearLevel = "Freshman",
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "CS201",
                    Name = "Data Structures and Algorithms",
                    Description = "Advanced programming concepts, data structures, and algorithms",
                    Credits = 4,
                    Department = "Computer Science",
                    Semester = "First Semester 2025",
                    Capacity = 25,
                    MinimumYearLevel = "Sophomore",
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "ENG101",
                    Name = "Introduction to Engineering",
                    Description = "Basic engineering principles and problem-solving techniques",
                    Credits = 3,
                    Department = "Engineering",
                    Semester = "First Semester 2025",
                    Capacity = 40,
                    MinimumYearLevel = "Freshman",
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "BUS101",
                    Name = "Principles of Business",
                    Description = "Introduction to business concepts and practices",
                    Credits = 3,
                    Department = "Business Administration",
                    Semester = "First Semester 2025",
                    Capacity = 35,
                    MinimumYearLevel = "Freshman",
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "MATH201",
                    Name = "Calculus I",
                    Description = "Limits, derivatives, and integration",
                    Credits = 4,
                    Department = "Mathematics",
                    Semester = "First Semester 2025",
                    Capacity = 30,
                    MinimumYearLevel = "Freshman",
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "ENG201",
                    Name = "Thermodynamics",
                    Description = "Principles of thermodynamics and heat transfer",
                    Credits = 3,
                    Department = "Engineering",
                    Semester = "First Semester 2025",
                    Capacity = 20,
                    MinimumYearLevel = "Sophomore",
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "BUS201",
                    Name = "Marketing Management",
                    Description = "Marketing strategies and consumer behavior",
                    Credits = 3,
                    Department = "Business Administration",
                    Semester = "First Semester 2025",
                    Capacity = 30,
                    MinimumYearLevel = "Sophomore",
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "CS301",
                    Name = "Database Systems",
                    Description = "Database design, SQL, and database management",
                    Credits = 3,
                    Department = "Computer Science",
                    Semester = "First Semester 2025",
                    Capacity = 25,
                    MinimumYearLevel = "Junior",
                    IsActive = true
                }
            };

            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();

            // Seed Course Prerequisites
            var cs101 = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "CS101");
            var cs201 = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "CS201");
            var cs301 = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "CS301");
            var eng101 = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "ENG101");
            var eng201 = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "ENG201");
            var bus101 = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "BUS101");
            var bus201 = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "BUS201");

            var prerequisites = new List<CoursePrerequisite>();

            if (cs101 != null && cs201 != null)
            {
                prerequisites.Add(new CoursePrerequisite
                {
                    CourseId = cs201.Id,
                    PrerequisiteCourseId = cs101.Id
                });
            }

            if (cs201 != null && cs301 != null)
            {
                prerequisites.Add(new CoursePrerequisite
                {
                    CourseId = cs301.Id,
                    PrerequisiteCourseId = cs201.Id
                });
            }

            if (eng101 != null && eng201 != null)
            {
                prerequisites.Add(new CoursePrerequisite
                {
                    CourseId = eng201.Id,
                    PrerequisiteCourseId = eng101.Id
                });
            }

            if (bus101 != null && bus201 != null)
            {
                prerequisites.Add(new CoursePrerequisite
                {
                    CourseId = bus201.Id,
                    PrerequisiteCourseId = bus101.Id
                });
            }

            context.CoursePrerequisites.AddRange(prerequisites);
            await context.SaveChangesAsync();
        }

        // Seed Programs
        if (!context.Programs.Any())
        {
            // Get departments for program assignment
            var ccsDeptForProgram = await context.Departments.FirstOrDefaultAsync(d => d.Code == "CCS");
            var coeDeptForProgram = await context.Departments.FirstOrDefaultAsync(d => d.Code == "COE");
            var cbaDeptForProgram = await context.Departments.FirstOrDefaultAsync(d => d.Code == "CBA");

            var csProgram = new Models.Program
            {
                Name = "Computer Science",
                DepartmentId = ccsDeptForProgram?.Id,
                Description = "Bachelor of Science in Computer Science - comprehensive program covering programming, algorithms, and software development",
                TotalCreditsRequired = 120,
                IsActive = true
            };

            var engProgram = new Models.Program
            {
                Name = "Mechanical Engineering",
                DepartmentId = coeDeptForProgram?.Id,
                Description = "Bachelor of Science in Mechanical Engineering - focus on design, analysis, and manufacturing",
                TotalCreditsRequired = 128,
                IsActive = true
            };

            var busProgram = new Models.Program
            {
                Name = "Business Administration",
                DepartmentId = cbaDeptForProgram?.Id,
                Description = "Bachelor of Business Administration - comprehensive business education with focus on management and entrepreneurship",
                TotalCreditsRequired = 120,
                IsActive = true
            };

            context.Programs.AddRange(csProgram, engProgram, busProgram);
            await context.SaveChangesAsync();

            // Seed Program Courses
            var cs101Course = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "CS101");
            var cs201Course = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "CS201");
            var cs301Course = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "CS301");
            var math201Course = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "MATH201");
            var eng101Course = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "ENG101");
            var eng201Course = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "ENG201");
            var bus101Course = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "BUS101");
            var bus201Course = await context.Courses.FirstOrDefaultAsync(c => c.CourseCode == "BUS201");

            var programCourses = new List<ProgramCourse>();

            // Computer Science Program Courses
            if (cs101Course != null)
            {
                programCourses.Add(new ProgramCourse
                {
                    ProgramId = csProgram.Id,
                    CourseId = cs101Course.Id,
                    IsRequired = true,
                    YearRecommended = "Freshman",
                    SemesterRecommended = "First Semester"
                });
            }

            if (math201Course != null)
            {
                programCourses.Add(new ProgramCourse
                {
                    ProgramId = csProgram.Id,
                    CourseId = math201Course.Id,
                    IsRequired = true,
                    YearRecommended = "Freshman",
                    SemesterRecommended = "First Semester"
                });
            }

            if (cs201Course != null)
            {
                programCourses.Add(new ProgramCourse
                {
                    ProgramId = csProgram.Id,
                    CourseId = cs201Course.Id,
                    IsRequired = true,
                    YearRecommended = "Sophomore",
                    SemesterRecommended = "First Semester"
                });
            }

            if (cs301Course != null)
            {
                programCourses.Add(new ProgramCourse
                {
                    ProgramId = csProgram.Id,
                    CourseId = cs301Course.Id,
                    IsRequired = true,
                    YearRecommended = "Junior",
                    SemesterRecommended = "First Semester"
                });
            }

            // Engineering Program Courses
            if (eng101Course != null)
            {
                programCourses.Add(new ProgramCourse
                {
                    ProgramId = engProgram.Id,
                    CourseId = eng101Course.Id,
                    IsRequired = true,
                    YearRecommended = "Freshman",
                    SemesterRecommended = "First Semester"
                });
            }

            if (math201Course != null)
            {
                programCourses.Add(new ProgramCourse
                {
                    ProgramId = engProgram.Id,
                    CourseId = math201Course.Id,
                    IsRequired = true,
                    YearRecommended = "Freshman",
                    SemesterRecommended = "First Semester"
                });
            }

            if (eng201Course != null)
            {
                programCourses.Add(new ProgramCourse
                {
                    ProgramId = engProgram.Id,
                    CourseId = eng201Course.Id,
                    IsRequired = true,
                    YearRecommended = "Sophomore",
                    SemesterRecommended = "Second Semester"
                });
            }

            // Business Program Courses
            if (bus101Course != null)
            {
                programCourses.Add(new ProgramCourse
                {
                    ProgramId = busProgram.Id,
                    CourseId = bus101Course.Id,
                    IsRequired = true,
                    YearRecommended = "Freshman",
                    SemesterRecommended = "First Semester"
                });
            }

            if (bus201Course != null)
            {
                programCourses.Add(new ProgramCourse
                {
                    ProgramId = busProgram.Id,
                    CourseId = bus201Course.Id,
                    IsRequired = true,
                    YearRecommended = "Sophomore",
                    SemesterRecommended = "Second Semester"
                });
            }

            if (math201Course != null)
            {
                programCourses.Add(new ProgramCourse
                {
                    ProgramId = busProgram.Id,
                    CourseId = math201Course.Id,
                    IsRequired = false,
                    YearRecommended = "Freshman",
                    SemesterRecommended = "First Semester"
                });
            }

            context.ProgramCourses.AddRange(programCourses);
            await context.SaveChangesAsync();
        }

        // Seed Freshman Student
        var freshmanEmail = "alex.wilson@student.edu";
        if (await userManager.FindByEmailAsync(freshmanEmail) == null)
        {
            var studentUser = new ApplicationUser
            {
                UserName = freshmanEmail,
                Email = freshmanEmail,
                FirstName = "Alex",
                LastName = "Wilson",
                StudentNumber = "2025-CCS-0002",
                DepartmentId = ccsDept?.Id,
                PhoneNumber = "+63 920 456 7890",
                DateOfBirth = new DateTime(2006, 11, 5),
                Address = "321 Aguinaldo Highway",
                City = "Cavite City",
                Province = "Cavite",
                Barangay = "Barangay San Antonio",
                PostalCode = "4100",
                Bio = "First-year student excited to start computer science journey.",
                YearOfStudy = "Freshman",
                GPA = 3.85m,
                Major = "Computer Science",
                EmergencyContactName = "Patricia Wilson",
                EmergencyContactPhone = "+63 920 654 3210",
                EmergencyContactRelationship = "Mother",
                EmailConfirmed = true,
                ApprovalStatus = "Approved",
                IsProfileCompleted = true
            };

            var result = await userManager.CreateAsync(studentUser, "Student123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(studentUser, "Student");
            }
        }

        // Seed Student Fees
        if (!context.Fees.Any())
        {
            var currentSemester = "1st Semester";
            var currentAcademicYear = "2024-2025";

            // Get all students
            var john = await userManager.FindByEmailAsync("john.doe@student.edu");
            var jane = await userManager.FindByEmailAsync("jane.smith@student.edu");
            var mike = await userManager.FindByEmailAsync("mike.johnson@student.edu");
            var alex = await userManager.FindByEmailAsync("alex.wilson@student.edu");

            var fees = new List<Models.Fee>();

            if (john != null)
            {
                fees.AddRange(new[]
                {
                    new Models.Fee { StudentId = john.Id, FeeType = Models.Enums.FeeType.Tuition, Amount = 28000, Description = "Tuition Fee - 1st Semester", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = john.Id, FeeType = Models.Enums.FeeType.Miscellaneous, Amount = 5000, Description = "Miscellaneous Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = john.Id, FeeType = Models.Enums.FeeType.Laboratory, Amount = 3000, Description = "Laboratory Fees - Computer Lab", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = john.Id, FeeType = Models.Enums.FeeType.Library, Amount = 1500, Description = "Library Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = john.Id, FeeType = Models.Enums.FeeType.Athletic, Amount = 1000, Description = "Athletic Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = john.Id, FeeType = Models.Enums.FeeType.Insurance, Amount = 800, Description = "Student Insurance", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = john.Id, FeeType = Models.Enums.FeeType.IDCard, Amount = 200, Description = "ID Card Fee", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow }
                });
            }

            if (jane != null)
            {
                fees.AddRange(new[]
                {
                    new Models.Fee { StudentId = jane.Id, FeeType = Models.Enums.FeeType.Tuition, Amount = 35000, Description = "Tuition Fee - 1st Semester", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = jane.Id, FeeType = Models.Enums.FeeType.Miscellaneous, Amount = 6000, Description = "Miscellaneous Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = jane.Id, FeeType = Models.Enums.FeeType.Laboratory, Amount = 5000, Description = "Laboratory Fees - Engineering Lab", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = jane.Id, FeeType = Models.Enums.FeeType.Library, Amount = 1500, Description = "Library Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = jane.Id, FeeType = Models.Enums.FeeType.Athletic, Amount = 1000, Description = "Athletic Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow }
                });
            }

            if (mike != null)
            {
                fees.AddRange(new[]
                {
                    new Models.Fee { StudentId = mike.Id, FeeType = Models.Enums.FeeType.Tuition, Amount = 25000, Description = "Tuition Fee - 1st Semester", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = mike.Id, FeeType = Models.Enums.FeeType.Miscellaneous, Amount = 4500, Description = "Miscellaneous Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = mike.Id, FeeType = Models.Enums.FeeType.Library, Amount = 1500, Description = "Library Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = mike.Id, FeeType = Models.Enums.FeeType.Athletic, Amount = 1000, Description = "Athletic Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = mike.Id, FeeType = Models.Enums.FeeType.Registration, Amount = 500, Description = "Registration Fee", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow }
                });
            }

            if (alex != null)
            {
                fees.AddRange(new[]
                {
                    new Models.Fee { StudentId = alex.Id, FeeType = Models.Enums.FeeType.Tuition, Amount = 26000, Description = "Tuition Fee - 1st Semester", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = alex.Id, FeeType = Models.Enums.FeeType.Miscellaneous, Amount = 5000, Description = "Miscellaneous Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = alex.Id, FeeType = Models.Enums.FeeType.Laboratory, Amount = 3000, Description = "Laboratory Fees - Computer Lab", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = alex.Id, FeeType = Models.Enums.FeeType.Library, Amount = 1500, Description = "Library Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = alex.Id, FeeType = Models.Enums.FeeType.Athletic, Amount = 1000, Description = "Athletic Fees", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow },
                    new Models.Fee { StudentId = alex.Id, FeeType = Models.Enums.FeeType.IDCard, Amount = 200, Description = "ID Card Fee", Semester = currentSemester, AcademicYear = currentAcademicYear, DateIssued = DateTime.UtcNow }
                });
            }

            context.Fees.AddRange(fees);
            await context.SaveChangesAsync();

            // Create Student Accounts and update balances
            foreach (var studentEmail in new[] { "john.doe@student.edu", "jane.smith@student.edu", "mike.johnson@student.edu", "alex.wilson@student.edu" })
            {
                var student = await userManager.FindByEmailAsync(studentEmail);
                if (student != null)
                {
                    var account = new StudentAccount
                    {
                        StudentId = student.Id,
                        Semester = currentSemester,
                        AcademicYear = currentAcademicYear,
                        TotalBilled = await context.Fees.Where(f => f.StudentId == student.Id && f.Semester == currentSemester && f.AcademicYear == currentAcademicYear).SumAsync(f => f.Amount),
                        TotalPaid = 0,
                        CreatedDate = DateTime.UtcNow
                    };

                    context.StudentAccounts.Add(account);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
