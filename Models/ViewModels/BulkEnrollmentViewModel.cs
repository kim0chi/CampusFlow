using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models.ViewModels;

public class BulkEnrollmentViewModel
{
    [Required(ErrorMessage = "You must select at least one course")]
    public int[] CourseIds { get; set; } = Array.Empty<int>();

    [Required(ErrorMessage = "Semester is required")]
    [StringLength(50)]
    public string Semester { get; set; } = string.Empty;

    [Required(ErrorMessage = "Academic year is required")]
    [StringLength(20)]
    public string AcademicYear { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Comments { get; set; }
}
