using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Services;

public class EnrollmentWorkflowService : IEnrollmentWorkflowService
{
    private readonly ApplicationDbContext _context;
    private readonly IQuarterPaymentService _quarterPaymentService;

    public EnrollmentWorkflowService(ApplicationDbContext _context, IQuarterPaymentService quarterPaymentService)
    {
        this._context = _context;
        _quarterPaymentService = quarterPaymentService;
    }

    public async Task<EnrollmentBatch> CreateEnrollmentBatchAsync(string studentId, int[] courseIds, string semester, string academicYear, string? comments)
    {
        // Validate student exists
        var student = await _context.Users.FindAsync(studentId);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found.");
        }

        if (courseIds == null || courseIds.Length == 0)
        {
            throw new InvalidOperationException("You must select at least one course to enroll.");
        }

        // Check if student already has a pending batch for this semester
        var existingBatch = await _context.EnrollmentBatches
            .Where(eb => eb.StudentId == studentId
                && eb.Semester == semester
                && eb.AcademicYear == academicYear
                && eb.Status != EnrollmentStatus.Rejected
                && eb.Status != EnrollmentStatus.Failed)
            .FirstOrDefaultAsync();

        if (existingBatch != null)
        {
            throw new InvalidOperationException($"You already have a pending or active enrollment batch for {semester} {academicYear}.");
        }

        // Check quarter payment requirement
        var meetsQuarterRequirement = await _quarterPaymentService.MeetsCurrentQuarterRequirementAsync(studentId);
        if (!meetsQuarterRequirement)
        {
            var currentQuarter = await _quarterPaymentService.GetCurrentActiveQuarterPeriodAsync();
            var requirement = currentQuarter != null
                ? await _quarterPaymentService.GetStudentQuarterRequirementAsync(studentId, currentQuarter.Id)
                : null;

            if (requirement != null)
            {
                throw new InvalidOperationException(
                    $"You must pay at least 30% of your total balance (₱{requirement.RequiredPaymentAmount:N2}) " +
                    $"for the current quarter ({currentQuarter?.Quarter}) by {currentQuarter?.PaymentDeadline:MMM dd, yyyy} " +
                    $"before enrolling. Your current balance is ₱{requirement.TotalBalanceAtQuarterStart:N2}. " +
                    $"You can either make a payment or request a promissory note from the Campus Director.");
            }
            else
            {
                throw new InvalidOperationException(
                    "You must meet the current quarter's payment requirement before enrolling. " +
                    "Please pay at least 30% of your total balance or request a promissory note from the Campus Director.");
            }
        }

        // Load all courses with prerequisites
        var courses = await _context.Courses
            .Where(c => courseIds.Contains(c.Id))
            .Include(c => c.PrerequisiteRelations)
            .ThenInclude(pr => pr.PrerequisiteCourse)
            .ToListAsync();

        if (courses.Count != courseIds.Length)
        {
            throw new InvalidOperationException("One or more selected courses not found.");
        }

        // Validate each course
        var validationErrors = new List<string>();
        var totalCredits = 0;

        foreach (var course in courses)
        {
            // Check if course is active
            if (!course.IsActive)
            {
                validationErrors.Add($"{course.CourseCode}: Course is inactive.");
                continue;
            }

            // Check capacity
            if (course.EnrolledCount >= course.Capacity)
            {
                validationErrors.Add($"{course.CourseCode}: Course is at full capacity.");
            }

            // Check year level requirement
            if (!string.IsNullOrEmpty(course.MinimumYearLevel))
            {
                if (!IsYearLevelSufficient(student.YearOfStudy, course.MinimumYearLevel))
                {
                    validationErrors.Add($"{course.CourseCode}: Requires minimum year level of {course.MinimumYearLevel}. Your current year level is {student.YearOfStudy ?? "not set"}.");
                }
            }

            // Check prerequisites
            if (course.PrerequisiteRelations.Any())
            {
                var completedCourseIds = await _context.Enrollments
                    .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Completed)
                    .Select(e => e.CourseId)
                    .ToListAsync();

                var missingPrerequisites = course.PrerequisiteRelations
                    .Where(pr => !completedCourseIds.Contains(pr.PrerequisiteCourseId))
                    .Select(pr => pr.PrerequisiteCourse.CourseCode)
                    .ToList();

                if (missingPrerequisites.Any())
                {
                    validationErrors.Add($"{course.CourseCode}: Missing prerequisites: {string.Join(", ", missingPrerequisites)}");
                }
            }

            // Check for duplicate enrollment in this batch
            var duplicatesInBatch = courseIds.Count(id => id == course.Id);
            if (duplicatesInBatch > 1)
            {
                validationErrors.Add($"{course.CourseCode}: Cannot enroll in the same course multiple times.");
            }

            // Check for existing enrollment
            var existingEnrollment = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId
                    && e.CourseId == course.Id
                    && e.Status != EnrollmentStatus.Rejected
                    && e.Status != EnrollmentStatus.Failed);

            if (existingEnrollment)
            {
                validationErrors.Add($"{course.CourseCode}: You already have an active or pending enrollment for this course.");
            }

            totalCredits += course.Credits;
        }

        if (validationErrors.Any())
        {
            throw new InvalidOperationException($"Enrollment validation failed:\n{string.Join("\n", validationErrors)}");
        }

        // Create enrollment batch
        var enrollmentBatch = new EnrollmentBatch
        {
            StudentId = studentId,
            Semester = semester,
            AcademicYear = academicYear,
            Status = EnrollmentStatus.DeanReview,
            SubmittedDate = DateTime.UtcNow,
            StudentComments = comments,
            TotalCredits = totalCredits
        };

        _context.EnrollmentBatches.Add(enrollmentBatch);
        await _context.SaveChangesAsync(); // Save to get the batch ID

        // Create individual enrollments for each course
        foreach (var course in courses)
        {
            var enrollment = new Enrollment
            {
                EnrollmentBatchId = enrollmentBatch.Id,
                StudentId = studentId,
                CourseId = course.Id,
                Status = EnrollmentStatus.DeanReview,
                SubmittedDate = DateTime.UtcNow,
                StudentComments = comments
            };

            _context.Enrollments.Add(enrollment);
        }

        // Create initial approval step for Dean
        var deanStep = new ApprovalStep
        {
            EnrollmentBatchId = enrollmentBatch.Id,
            StepType = ApprovalStepType.Dean,
            Status = ApprovalStatus.Pending,
            CreatedDate = DateTime.UtcNow
        };

        _context.ApprovalSteps.Add(deanStep);

        await _context.SaveChangesAsync();

        return enrollmentBatch;
    }

    public async Task<bool> ProcessApprovalAsync(int enrollmentBatchId, ApprovalStepType stepType, string approverId, bool isApproved, string? comments)
    {
        var batch = await _context.EnrollmentBatches
            .Include(eb => eb.ApprovalSteps)
            .Include(eb => eb.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(eb => eb.Id == enrollmentBatchId);

        if (batch == null)
        {
            throw new InvalidOperationException("Enrollment batch not found.");
        }

        // Get the current approval step
        var currentStep = batch.ApprovalSteps
            .FirstOrDefault(a => a.StepType == stepType && a.Status == ApprovalStatus.Pending);

        if (currentStep == null)
        {
            throw new InvalidOperationException("No pending approval step found for this role.");
        }

        // Update current step
        currentStep.Status = isApproved ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
        currentStep.ApproverId = approverId;
        currentStep.ActionDate = DateTime.UtcNow;
        currentStep.Comments = comments;

        if (!isApproved)
        {
            // Rejection - end workflow
            batch.Status = EnrollmentStatus.Rejected;
            batch.CompletedDate = DateTime.UtcNow;

            // Update all individual enrollments status
            foreach (var enrollment in batch.Enrollments)
            {
                enrollment.Status = EnrollmentStatus.Rejected;
                enrollment.CompletedDate = DateTime.UtcNow;
            }
        }
        else
        {
            // Special handling for Dean approval - go to AwaitingPayment
            if (stepType == ApprovalStepType.Dean)
            {
                batch.Status = EnrollmentStatus.AwaitingPayment;

                // Update all individual enrollments status
                foreach (var enrollment in batch.Enrollments)
                {
                    enrollment.Status = EnrollmentStatus.AwaitingPayment;
                }
            }
            else
            {
                // Move to next step
                var nextStep = GetNextStep(stepType);
                if (nextStep.HasValue)
                {
                    // Create next approval step
                    var nextApprovalStep = new ApprovalStep
                    {
                        EnrollmentBatchId = enrollmentBatchId,
                        StepType = nextStep.Value,
                        Status = ApprovalStatus.Pending,
                        CreatedDate = DateTime.UtcNow
                    };

                    _context.ApprovalSteps.Add(nextApprovalStep);

                    // Update batch status
                    batch.Status = GetEnrollmentStatusForStep(nextStep.Value);

                    // Update all individual enrollments status
                    foreach (var enrollment in batch.Enrollments)
                    {
                        enrollment.Status = batch.Status;
                    }
                }
                else
                {
                    // All approvals complete - enroll student in all courses
                    batch.Status = EnrollmentStatus.Enrolled;
                    batch.CompletedDate = DateTime.UtcNow;

                    // Update all individual enrollments and increment course counts
                    foreach (var enrollment in batch.Enrollments)
                    {
                        enrollment.Status = EnrollmentStatus.Enrolled;
                        enrollment.CompletedDate = DateTime.UtcNow;
                        enrollment.Course.EnrolledCount++;
                    }
                }
            }
        }

        await _context.SaveChangesAsync();

        return isApproved;
    }

    public async Task<List<EnrollmentBatch>> GetPendingEnrollmentBatchesForRoleAsync(string roleName)
    {
        var stepType = GetStepTypeFromRole(roleName);
        if (!stepType.HasValue)
        {
            return new List<EnrollmentBatch>();
        }

        var batches = await _context.EnrollmentBatches
            .Include(eb => eb.Student)
            .ThenInclude(s => s.Department)
            .Include(eb => eb.Enrollments)
            .ThenInclude(e => e.Course)
            .Include(eb => eb.ApprovalSteps)
            .Where(eb => eb.ApprovalSteps.Any(a => a.StepType == stepType.Value && a.Status == ApprovalStatus.Pending))
            .OrderBy(eb => eb.SubmittedDate)
            .ToListAsync();

        return batches;
    }

    public async Task<ApprovalStepType?> GetCurrentApprovalStepAsync(int enrollmentBatchId)
    {
        var batch = await _context.EnrollmentBatches
            .Include(eb => eb.ApprovalSteps)
            .FirstOrDefaultAsync(eb => eb.Id == enrollmentBatchId);

        if (batch == null)
        {
            return null;
        }

        var pendingStep = batch.ApprovalSteps
            .FirstOrDefault(a => a.Status == ApprovalStatus.Pending);

        return pendingStep?.StepType;
    }

    public async Task<bool> CanUserApproveAsync(int enrollmentBatchId, string userId, string userRole)
    {
        var currentStep = await GetCurrentApprovalStepAsync(enrollmentBatchId);
        if (!currentStep.HasValue)
        {
            return false;
        }

        var expectedRole = GetRoleFromStepType(currentStep.Value);
        return expectedRole == userRole;
    }

    public async Task<List<ApprovalStep>> GetBatchHistoryAsync(int enrollmentBatchId)
    {
        return await _context.ApprovalSteps
            .Include(a => a.Approver)
            .Where(a => a.EnrollmentBatchId == enrollmentBatchId)
            .OrderBy(a => a.CreatedDate)
            .ToListAsync();
    }

    private ApprovalStepType? GetNextStep(ApprovalStepType currentStep)
    {
        return currentStep switch
        {
            ApprovalStepType.Dean => null, // After Dean, goes to AwaitingPayment (not an approval step)
            ApprovalStepType.Accounting => ApprovalStepType.SAO,
            ApprovalStepType.SAO => ApprovalStepType.Library,
            ApprovalStepType.Library => ApprovalStepType.Records,
            ApprovalStepType.Records => null,
            _ => null
        };
    }

    private EnrollmentStatus GetEnrollmentStatusForStep(ApprovalStepType stepType)
    {
        return stepType switch
        {
            ApprovalStepType.Dean => EnrollmentStatus.DeanReview,
            ApprovalStepType.Accounting => EnrollmentStatus.AccountingReview,
            ApprovalStepType.SAO => EnrollmentStatus.SAOReview,
            ApprovalStepType.Library => EnrollmentStatus.LibraryReview,
            ApprovalStepType.Records => EnrollmentStatus.RecordsReview,
            _ => EnrollmentStatus.Submitted
        };
    }

    private ApprovalStepType? GetStepTypeFromRole(string roleName)
    {
        return roleName switch
        {
            "Dean" => ApprovalStepType.Dean,
            "Accounting" => ApprovalStepType.Accounting,
            "SAO" => ApprovalStepType.SAO,
            "Library" => ApprovalStepType.Library,
            "Records" => ApprovalStepType.Records,
            _ => null
        };
    }

    private string GetRoleFromStepType(ApprovalStepType stepType)
    {
        return stepType switch
        {
            ApprovalStepType.Dean => "Dean",
            ApprovalStepType.Accounting => "Accounting",
            ApprovalStepType.SAO => "SAO",
            ApprovalStepType.Library => "Library",
            ApprovalStepType.Records => "Records",
            _ => ""
        };
    }

    private bool IsYearLevelSufficient(string? studentYearLevel, string requiredYearLevel)
    {
        if (string.IsNullOrEmpty(studentYearLevel))
        {
            return false;
        }

        var yearLevelHierarchy = new Dictionary<string, int>
        {
            { "Freshman", 1 },
            { "Sophomore", 2 },
            { "Junior", 3 },
            { "Senior", 4 },
            { "Graduate", 5 }
        };

        if (!yearLevelHierarchy.TryGetValue(studentYearLevel, out var studentLevel))
        {
            return false;
        }

        if (!yearLevelHierarchy.TryGetValue(requiredYearLevel, out var requiredLevel))
        {
            return false;
        }

        return studentLevel >= requiredLevel;
    }
}
