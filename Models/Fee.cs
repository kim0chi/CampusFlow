using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models;

public class Fee
{
    public int Id { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    public int? StudentAccountId { get; set; }

    [Required]
    public FeeType FeeType { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(50)]
    public string Semester { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string AcademicYear { get; set; } = string.Empty;

    public DateTime DateIssued { get; set; } = DateTime.UtcNow;

    public string? IssuedById { get; set; } // Admin/Cashier who issued the fee

    // Navigation properties
    public ApplicationUser Student { get; set; } = null!;
    public StudentAccount? StudentAccount { get; set; }
    public ApplicationUser? IssuedBy { get; set; }
}
