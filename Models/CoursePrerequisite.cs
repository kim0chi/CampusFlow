using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models;

public class CoursePrerequisite
{
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required]
    public int PrerequisiteCourseId { get; set; }

    // Navigation properties
    public Course Course { get; set; } = null!;
    public Course PrerequisiteCourse { get; set; } = null!;
}
