using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Range(1, 10)]
    public int Credits { get; set; }

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Semester { get; set; } = string.Empty;

    [Required]
    [Range(1, 500)]
    public int Capacity { get; set; }

    public int EnrolledCount { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(20)]
    public string? MinimumYearLevel { get; set; } // Freshman, Sophomore, Junior, Senior, Graduate

    // Navigation properties
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<CoursePrerequisite> PrerequisiteRelations { get; set; } = new List<CoursePrerequisite>();
    public ICollection<CoursePrerequisite> DependentCourseRelations { get; set; } = new List<CoursePrerequisite>();
}
