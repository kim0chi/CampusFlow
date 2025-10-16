using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models;

public class Program
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    [Range(30, 300)]
    public int TotalCreditsRequired { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<ProgramCourse> ProgramCourses { get; set; } = new List<ProgramCourse>();
}
