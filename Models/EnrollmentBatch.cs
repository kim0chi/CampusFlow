using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models;

public class EnrollmentBatch
{
    public int Id { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Semester { get; set; } = string.Empty; // e.g., "Fall 2024", "Spring 2025"

    [Required]
    [StringLength(20)]
    public string AcademicYear { get; set; } = string.Empty; // e.g., "2024-2025"

    [Required]
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Submitted;

    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedDate { get; set; }

    [StringLength(500)]
    public string? StudentComments { get; set; }

    public int TotalCredits { get; set; } = 0; // Sum of all course credits in batch

    // Navigation properties
    public ApplicationUser Student { get; set; } = null!;
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<ApprovalStep> ApprovalSteps { get; set; } = new List<ApprovalStep>();
    public EnrollmentPayment? EnrollmentPayment { get; set; }
}
