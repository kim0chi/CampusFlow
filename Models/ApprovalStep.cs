using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models;

public class ApprovalStep
{
    public int Id { get; set; }

    [Required]
    public int EnrollmentBatchId { get; set; }

    [Required]
    public ApprovalStepType StepType { get; set; }

    [Required]
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    public string? ApproverId { get; set; }

    public DateTime? ActionDate { get; set; }

    [StringLength(1000)]
    public string? Comments { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public EnrollmentBatch EnrollmentBatch { get; set; } = null!;
    public ApplicationUser? Approver { get; set; }
}
