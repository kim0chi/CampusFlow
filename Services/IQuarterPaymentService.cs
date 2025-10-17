using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Services
{
    public interface IQuarterPaymentService
    {
        /// <summary>
        /// Generate quarter payment requirements for all enrolled students for a specific quarter period
        /// </summary>
        Task GenerateQuarterRequirementsAsync(int quarterPeriodId);

        /// <summary>
        /// Get or create a quarter payment requirement for a specific student and quarter
        /// </summary>
        Task<QuarterPaymentRequirement> GetOrCreateQuarterRequirementAsync(string studentId, int quarterPeriodId);

        /// <summary>
        /// Process a payment towards a quarter requirement
        /// </summary>
        Task<bool> ProcessQuarterPaymentAsync(int requirementId, decimal paymentAmount, int paymentId);

        /// <summary>
        /// Check if a student meets the current quarter's payment requirement
        /// </summary>
        Task<bool> MeetsCurrentQuarterRequirementAsync(string studentId);

        /// <summary>
        /// Get the active quarter period for a given semester and academic year
        /// </summary>
        Task<QuarterPeriod?> GetActiveQuarterPeriodAsync(string semester, string academicYear);

        /// <summary>
        /// Get the active quarter period (current date-based)
        /// </summary>
        Task<QuarterPeriod?> GetCurrentActiveQuarterPeriodAsync();

        /// <summary>
        /// Get student's quarter payment requirement for a specific quarter
        /// </summary>
        Task<QuarterPaymentRequirement?> GetStudentQuarterRequirementAsync(string studentId, int quarterPeriodId);

        /// <summary>
        /// Get all quarter payment requirements for a student
        /// </summary>
        Task<List<QuarterPaymentRequirement>> GetStudentQuarterRequirementsAsync(string studentId);

        /// <summary>
        /// Calculate the 30% payment requirement based on total balance
        /// </summary>
        decimal CalculateRequiredPayment(decimal totalBalance);

        /// <summary>
        /// Link a promissory note to a quarter payment requirement
        /// </summary>
        Task LinkPromissoryNoteAsync(int requirementId, int promissoryNoteId);

        /// <summary>
        /// Approve a quarter payment requirement based on promissory note approval
        /// </summary>
        Task ApproveRequirementViaPromissoryNoteAsync(int requirementId);

        /// <summary>
        /// Get students with overdue quarter payments
        /// </summary>
        Task<List<QuarterPaymentRequirement>> GetOverdueRequirementsAsync();
    }
}
