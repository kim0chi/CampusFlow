using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models;

public class StudentAccount
{
    public int Id { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Semester { get; set; } = string.Empty; // e.g., "1st Semester", "2nd Semester"

    [Required]
    [StringLength(20)]
    public string AcademicYear { get; set; } = string.Empty; // e.g., "2024-2025"

    public decimal TotalBilled { get; set; } = 0;
    public decimal TotalPaid { get; set; } = 0;

    public decimal CurrentBalance => TotalBilled - TotalPaid;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdated { get; set; }

    // Navigation properties
    public ApplicationUser Student { get; set; } = null!;
    public ICollection<Fee> Fees { get; set; } = new List<Fee>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
