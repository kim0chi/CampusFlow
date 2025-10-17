using System.ComponentModel.DataAnnotations;
using StudentEnrollmentSystem.Models.Enums;

namespace StudentEnrollmentSystem.Models
{
    public class QuarterPeriod
    {
        public int Id { get; set; }

        [Required]
        public Quarter Quarter { get; set; }

        [Required]
        [MaxLength(50)]
        public string Semester { get; set; } = string.Empty; // e.g., "First Semester", "Second Semester"

        [Required]
        [MaxLength(50)]
        public string AcademicYear { get; set; } = string.Empty; // e.g., "2024-2025"

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime PaymentDeadline { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(450)]
        public string? CreatedBy { get; set; }

        public ApplicationUser? CreatedByUser { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<QuarterPaymentRequirement> PaymentRequirements { get; set; } = new List<QuarterPaymentRequirement>();
    }
}
