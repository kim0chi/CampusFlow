using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Services;

public interface IEnrollmentWorkflowService
{
    Task<EnrollmentBatch> CreateEnrollmentBatchAsync(string studentId, int[] courseIds, string semester, string academicYear, string? comments);
    Task<bool> ProcessApprovalAsync(int enrollmentBatchId, ApprovalStepType stepType, string approverId, bool isApproved, string? comments);
    Task<List<EnrollmentBatch>> GetPendingEnrollmentBatchesForRoleAsync(string roleName);
    Task<ApprovalStepType?> GetCurrentApprovalStepAsync(int enrollmentBatchId);
    Task<bool> CanUserApproveAsync(int enrollmentBatchId, string userId, string userRole);
    Task<List<ApprovalStep>> GetBatchHistoryAsync(int enrollmentBatchId);
}
