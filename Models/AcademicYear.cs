using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models;

public class AcademicYear
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Year { get; set; } = string.Empty; // e.g., "2024-2025"

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public bool IsCurrentYear { get; set; } = false; // Only one can be current at a time

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
