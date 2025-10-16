using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models;

public class Payment
{
    public int Id { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    public int? StudentAccountId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [Required]
    [StringLength(50)]
    public string ReferenceNumber { get; set; } = string.Empty;

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(50)]
    public string Semester { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string AcademicYear { get; set; } = string.Empty;

    public string? ProcessedByCashierId { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    // Navigation properties
    public ApplicationUser Student { get; set; } = null!;
    public StudentAccount? StudentAccount { get; set; }
    public ApplicationUser? ProcessedByCashier { get; set; }
    public ICollection<EnrollmentPayment> EnrollmentPayments { get; set; } = new List<EnrollmentPayment>();
}
