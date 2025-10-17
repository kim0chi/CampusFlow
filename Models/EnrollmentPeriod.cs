using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models
{
    public class EnrollmentPeriod
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Semester { get; set; } = string.Empty; // e.g., "First Semester", "Second Semester", "Summer"

        [Required]
        [MaxLength(50)]
        public string AcademicYear { get; set; } = string.Empty; // e.g., "2024-2025"

        [Required]
        public DateTime OpenDate { get; set; }

        [Required]
        public DateTime CloseDate { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(450)]
        public string? CreatedBy { get; set; }

        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
