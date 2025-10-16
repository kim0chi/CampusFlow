using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models;

public class Enrollment
{
    public int Id { get; set; }

    [Required]
    public int EnrollmentBatchId { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    public int CourseId { get; set; }

    [Required]
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Submitted;

    public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedDate { get; set; }

    [StringLength(500)]
    public string? StudentComments { get; set; }

    [StringLength(5)]
    public string? Grade { get; set; } // A, B+, B, C, D, F, etc.

    public string? GradedById { get; set; } // Admin/Dean who assigned the grade

    public DateTime? GradedDate { get; set; }

    // Navigation properties
    public EnrollmentBatch EnrollmentBatch { get; set; } = null!;
    public ApplicationUser Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public ApplicationUser? GradedBy { get; set; }
}
