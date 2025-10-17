using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Services
{
    public class QuarterPaymentService : IQuarterPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IStudentAccountService _studentAccountService;
        private const decimal REQUIRED_PAYMENT_PERCENTAGE = 0.30m; // 30%

        public QuarterPaymentService(ApplicationDbContext context, IStudentAccountService studentAccountService)
        {
            _context = context;
            _studentAccountService = studentAccountService;
        }

        public async Task GenerateQuarterRequirementsAsync(int quarterPeriodId)
        {
            var quarterPeriod = await _context.QuarterPeriods.FindAsync(quarterPeriodId);
            if (quarterPeriod == null)
            {
                throw new ArgumentException("Quarter period not found");
            }

            // Get all students who have a balance (current or accumulated)
            var studentsWithBalance = await _context.StudentAccounts
                .Where(sa => sa.CurrentBalance > 0)
                .Select(sa => sa.StudentId)
                .Distinct()
                .ToListAsync();

            foreach (var studentId in studentsWithBalance)
            {
                // Check if requirement already exists
                var existingRequirement = await _context.QuarterPaymentRequirements
                    .FirstOrDefaultAsync(qpr => qpr.StudentId == studentId && qpr.QuarterPeriodId == quarterPeriodId);

                if (existingRequirement == null)
                {
                    // Calculate total balance across all semesters
                    var totalBalance = await _studentAccountService.GetStudentTotalBalanceAsync(studentId);

                    if (totalBalance > 0)
                    {
                        var requirement = new QuarterPaymentRequirement
                        {
                            StudentId = studentId,
                            QuarterPeriodId = quarterPeriodId,
                            TotalBalanceAtQuarterStart = totalBalance,
                            RequiredPaymentAmount = CalculateRequiredPayment(totalBalance),
                            ActualPaymentAmount = 0,
                            MeetsRequirement = false,
                            HasPromissoryNote = false,
                            Status = "Pending",
                            CreatedDate = DateTime.UtcNow
                        };

                        _context.QuarterPaymentRequirements.Add(requirement);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<QuarterPaymentRequirement> GetOrCreateQuarterRequirementAsync(string studentId, int quarterPeriodId)
        {
            var requirement = await _context.QuarterPaymentRequirements
                .Include(qpr => qpr.QuarterPeriod)
                .Include(qpr => qpr.PromissoryNote)
                .FirstOrDefaultAsync(qpr => qpr.StudentId == studentId && qpr.QuarterPeriodId == quarterPeriodId);

            if (requirement == null)
            {
                // Create new requirement
                var totalBalance = await _studentAccountService.GetStudentTotalBalanceAsync(studentId);

                requirement = new QuarterPaymentRequirement
                {
                    StudentId = studentId,
                    QuarterPeriodId = quarterPeriodId,
                    TotalBalanceAtQuarterStart = totalBalance,
                    RequiredPaymentAmount = CalculateRequiredPayment(totalBalance),
                    ActualPaymentAmount = 0,
                    MeetsRequirement = false,
                    HasPromissoryNote = false,
                    Status = "Pending",
                    CreatedDate = DateTime.UtcNow
                };

                _context.QuarterPaymentRequirements.Add(requirement);
                await _context.SaveChangesAsync();

                // Reload to get navigation properties
                requirement = await _context.QuarterPaymentRequirements
                    .Include(qpr => qpr.QuarterPeriod)
                    .Include(qpr => qpr.PromissoryNote)
                    .FirstAsync(qpr => qpr.Id == requirement.Id);
            }

            return requirement;
        }

        public async Task<bool> ProcessQuarterPaymentAsync(int requirementId, decimal paymentAmount, int paymentId)
        {
            var requirement = await _context.QuarterPaymentRequirements.FindAsync(requirementId);
            if (requirement == null)
            {
                return false;
            }

            requirement.ActualPaymentAmount += paymentAmount;

            // Check if payment meets 30% requirement
            if (requirement.ActualPaymentAmount >= requirement.RequiredPaymentAmount)
            {
                requirement.MeetsRequirement = true;
                requirement.Status = "Paid";
                requirement.PaidDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MeetsCurrentQuarterRequirementAsync(string studentId)
        {
            var currentQuarter = await GetCurrentActiveQuarterPeriodAsync();
            if (currentQuarter == null)
            {
                // If no active quarter period is set, allow enrollment
                return true;
            }

            var requirement = await GetStudentQuarterRequirementAsync(studentId, currentQuarter.Id);
            if (requirement == null)
            {
                // No requirement exists - check if student has a balance
                var totalBalance = await _studentAccountService.GetStudentTotalBalanceAsync(studentId);
                if (totalBalance <= 0)
                {
                    // No balance, no payment required
                    return true;
                }

                // Create requirement for this student
                requirement = await GetOrCreateQuarterRequirementAsync(studentId, currentQuarter.Id);
            }

            return requirement.MeetsRequirement;
        }

        public async Task<QuarterPeriod?> GetActiveQuarterPeriodAsync(string semester, string academicYear)
        {
            return await _context.QuarterPeriods
                .Where(qp => qp.Semester == semester && qp.AcademicYear == academicYear && qp.IsActive)
                .OrderBy(qp => qp.Quarter)
                .FirstOrDefaultAsync();
        }

        public async Task<QuarterPeriod?> GetCurrentActiveQuarterPeriodAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.QuarterPeriods
                .Where(qp => qp.IsActive && qp.StartDate <= now && qp.EndDate >= now)
                .FirstOrDefaultAsync();
        }

        public async Task<QuarterPaymentRequirement?> GetStudentQuarterRequirementAsync(string studentId, int quarterPeriodId)
        {
            return await _context.QuarterPaymentRequirements
                .Include(qpr => qpr.QuarterPeriod)
                .Include(qpr => qpr.PromissoryNote)
                .FirstOrDefaultAsync(qpr => qpr.StudentId == studentId && qpr.QuarterPeriodId == quarterPeriodId);
        }

        public async Task<List<QuarterPaymentRequirement>> GetStudentQuarterRequirementsAsync(string studentId)
        {
            return await _context.QuarterPaymentRequirements
                .Include(qpr => qpr.QuarterPeriod)
                .Include(qpr => qpr.PromissoryNote)
                .Where(qpr => qpr.StudentId == studentId)
                .OrderByDescending(qpr => qpr.CreatedDate)
                .ToListAsync();
        }

        public decimal CalculateRequiredPayment(decimal totalBalance)
        {
            return Math.Round(totalBalance * REQUIRED_PAYMENT_PERCENTAGE, 2);
        }

        public async Task LinkPromissoryNoteAsync(int requirementId, int promissoryNoteId)
        {
            var requirement = await _context.QuarterPaymentRequirements.FindAsync(requirementId);
            if (requirement == null)
            {
                throw new ArgumentException("Requirement not found");
            }

            requirement.HasPromissoryNote = true;
            requirement.PromissoryNoteId = promissoryNoteId;
            requirement.Status = "PromissoryNoteSubmitted";

            await _context.SaveChangesAsync();
        }

        public async Task ApproveRequirementViaPromissoryNoteAsync(int requirementId)
        {
            var requirement = await _context.QuarterPaymentRequirements.FindAsync(requirementId);
            if (requirement == null)
            {
                throw new ArgumentException("Requirement not found");
            }

            requirement.MeetsRequirement = true;
            requirement.Status = "PromissoryApproved";

            await _context.SaveChangesAsync();
        }

        public async Task<List<QuarterPaymentRequirement>> GetOverdueRequirementsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.QuarterPaymentRequirements
                .Include(qpr => qpr.QuarterPeriod)
                .Include(qpr => qpr.Student)
                .Where(qpr => !qpr.MeetsRequirement && qpr.QuarterPeriod.PaymentDeadline < now)
                .ToListAsync();
        }
    }
}
