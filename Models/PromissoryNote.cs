using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models;

public class PromissoryNote
{
    public int Id { get; set; }

    [Required]
    public int EnrollmentBatchId { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal AmountCovered { get; set; }

    [Required]
    [StringLength(1000)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    public DateTime RepaymentDeadline { get; set; }

    [Required]
    public PromissoryNoteStatus Status { get; set; } = PromissoryNoteStatus.Pending;

    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

    public string? ReviewedByCampusDirectorId { get; set; }

    public DateTime? ReviewedDate { get; set; }

    [StringLength(1000)]
    public string? DirectorComments { get; set; }

    // Optional: Link to quarter payment requirement if this promissory note is for a quarter payment
    public int? QuarterPaymentRequirementId { get; set; }

    // Navigation properties
    public EnrollmentBatch EnrollmentBatch { get; set; } = null!;
    public ApplicationUser Student { get; set; } = null!;
    public ApplicationUser? ReviewedByCampusDirector { get; set; }
    public QuarterPaymentRequirement? QuarterPaymentRequirement { get; set; }
}
