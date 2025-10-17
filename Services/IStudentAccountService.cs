using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Services;

public interface IStudentAccountService
{
    Task<StudentAccount> GetOrCreateStudentAccountAsync(string studentId, string semester, string academicYear);
    Task<decimal> GetStudentBalanceAsync(string studentId, string semester, string academicYear);
    Task<decimal> GetStudentTotalBalanceAsync(string studentId);
    Task<decimal> CalculatePaymentPercentageAsync(decimal amountPaid, decimal balance);
    Task<bool> MeetsPaymentRequirementAsync(decimal percentage);
    Task UpdateAccountBalancesAsync(string studentId, string semester, string academicYear);
    Task<List<Fee>> GetStudentFeesAsync(string studentId, string semester, string academicYear);
    Task<List<Payment>> GetStudentPaymentsAsync(string studentId, string semester, string academicYear);
}
