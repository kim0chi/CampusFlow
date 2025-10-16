using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Services;

public class StudentAccountService : IStudentAccountService
{
    private readonly ApplicationDbContext _context;
    private const decimal MINIMUM_PAYMENT_PERCENTAGE = 30.0m; // 30% required

    public StudentAccountService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StudentAccount> GetOrCreateStudentAccountAsync(string studentId, string semester, string academicYear)
    {
        var account = await _context.StudentAccounts
            .Include(sa => sa.Fees)
            .Include(sa => sa.Payments)
            .FirstOrDefaultAsync(sa => sa.StudentId == studentId
                && sa.Semester == semester
                && sa.AcademicYear == academicYear);

        if (account == null)
        {
            account = new StudentAccount
            {
                StudentId = studentId,
                Semester = semester,
                AcademicYear = academicYear,
                TotalBilled = 0,
                TotalPaid = 0,
                CreatedDate = DateTime.UtcNow
            };

            _context.StudentAccounts.Add(account);
            await _context.SaveChangesAsync();
        }

        return account;
    }

    public async Task<decimal> GetStudentBalanceAsync(string studentId, string semester, string academicYear)
    {
        var account = await GetOrCreateStudentAccountAsync(studentId, semester, academicYear);

        // Recalculate to ensure accuracy
        await UpdateAccountBalancesAsync(studentId, semester, academicYear);

        // Reload account to get updated values
        await _context.Entry(account).ReloadAsync();

        return account.CurrentBalance;
    }

    public async Task<decimal> CalculatePaymentPercentageAsync(decimal amountPaid, decimal balance)
    {
        if (balance <= 0)
            return 100m; // No balance means 100% paid

        var percentage = (amountPaid / balance) * 100;
        return Math.Round(percentage, 2);
    }

    public async Task<bool> MeetsPaymentRequirementAsync(decimal percentage)
    {
        return await Task.FromResult(percentage >= MINIMUM_PAYMENT_PERCENTAGE);
    }

    public async Task UpdateAccountBalancesAsync(string studentId, string semester, string academicYear)
    {
        var account = await GetOrCreateStudentAccountAsync(studentId, semester, academicYear);

        // Calculate total fees
        var totalFees = await _context.Fees
            .Where(f => f.StudentId == studentId
                && f.Semester == semester
                && f.AcademicYear == academicYear)
            .SumAsync(f => f.Amount);

        // Calculate total payments
        var totalPayments = await _context.Payments
            .Where(p => p.StudentId == studentId
                && p.Semester == semester
                && p.AcademicYear == academicYear)
            .SumAsync(p => p.Amount);

        account.TotalBilled = totalFees;
        account.TotalPaid = totalPayments;
        account.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<List<Fee>> GetStudentFeesAsync(string studentId, string semester, string academicYear)
    {
        return await _context.Fees
            .Include(f => f.IssuedBy)
            .Where(f => f.StudentId == studentId
                && f.Semester == semester
                && f.AcademicYear == academicYear)
            .OrderByDescending(f => f.DateIssued)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetStudentPaymentsAsync(string studentId, string semester, string academicYear)
    {
        return await _context.Payments
            .Include(p => p.ProcessedByCashier)
            .Where(p => p.StudentId == studentId
                && p.Semester == semester
                && p.AcademicYear == academicYear)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }
}
