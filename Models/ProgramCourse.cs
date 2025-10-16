using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models;

public class ProgramCourse
{
    public int Id { get; set; }

    [Required]
    public int ProgramId { get; set; }

    [Required]
    public int CourseId { get; set; }

    public bool IsRequired { get; set; } = true;

    [StringLength(20)]
    public string? YearRecommended { get; set; } // Freshman, Sophomore, Junior, Senior

    [StringLength(50)]
    public string? SemesterRecommended { get; set; } // First Semester, Second Semester, Summer

    // Navigation properties
    public Program Program { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
