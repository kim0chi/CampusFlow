using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models;

public class ApplicationUser : IdentityUser
{
    // Basic Information
    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(20)]
    public string? StudentNumber { get; set; }

    // Department relationship
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    // Extended Profile Information
    public DateTime? DateOfBirth { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Province { get; set; }

    [StringLength(100)]
    public string? Municipality { get; set; }

    [StringLength(100)]
    public string? Barangay { get; set; }

    [StringLength(10)]
    public string? PostalCode { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    [StringLength(20)]
    public string? YearOfStudy { get; set; } // Freshman, Sophomore, Junior, Senior, Graduate

    public decimal? GPA { get; set; }

    [StringLength(100)]
    public string? Major { get; set; }

    [StringLength(100)]
    public string? Minor { get; set; }

    [StringLength(500)]
    public string? ProfilePictureUrl { get; set; }

    // Emergency Contact
    [StringLength(100)]
    public string? EmergencyContactName { get; set; }

    [StringLength(20)]
    public string? EmergencyContactPhone { get; set; }

    [StringLength(50)]
    public string? EmergencyContactRelationship { get; set; }

    // Profile Completion Tracking
    public bool IsProfileCompleted { get; set; } = false;

    // Approval Workflow
    [StringLength(20)]
    public string ApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

    public string? ApprovedById { get; set; }
    public ApplicationUser? ApprovedBy { get; set; }

    public DateTime? ApprovalDate { get; set; }

    [StringLength(500)]
    public string? RejectionReason { get; set; }

    // Computed Properties
    public string FullName => $"{FirstName} {LastName}";

    public int? Age
    {
        get
        {
            if (DateOfBirth.HasValue)
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                return age;
            }
            return null;
        }
    }

    public string Initials => $"{FirstName?.FirstOrDefault()}{LastName?.FirstOrDefault()}".ToUpper();

    public int ProfileCompletionPercentage
    {
        get
        {
            var totalFields = 15;
            var completedFields = 0;

            if (!string.IsNullOrEmpty(FirstName)) completedFields++;
            if (!string.IsNullOrEmpty(LastName)) completedFields++;
            if (!string.IsNullOrEmpty(Email)) completedFields++;
            if (!string.IsNullOrEmpty(PhoneNumber)) completedFields++;
            if (!string.IsNullOrEmpty(StudentNumber)) completedFields++;
            if (DepartmentId.HasValue) completedFields++;
            if (DateOfBirth.HasValue) completedFields++;
            if (!string.IsNullOrEmpty(Address)) completedFields++;
            if (!string.IsNullOrEmpty(City)) completedFields++;
            if (!string.IsNullOrEmpty(Province)) completedFields++;
            if (!string.IsNullOrEmpty(Barangay)) completedFields++;
            if (!string.IsNullOrEmpty(Major)) completedFields++;
            if (!string.IsNullOrEmpty(EmergencyContactName)) completedFields++;
            if (!string.IsNullOrEmpty(EmergencyContactPhone)) completedFields++;
            if (!string.IsNullOrEmpty(ProfilePictureUrl)) completedFields++;

            return (int)((double)completedFields / totalFields * 100);
        }
    }

    public bool HasRequiredProfileFields()
    {
        // Check mandatory fields for new students
        return !string.IsNullOrEmpty(FirstName) &&
               !string.IsNullOrEmpty(LastName) &&
               !string.IsNullOrEmpty(PhoneNumber) &&
               DateOfBirth.HasValue &&
               !string.IsNullOrEmpty(Address) &&
               !string.IsNullOrEmpty(City) &&
               !string.IsNullOrEmpty(Province) &&
               !string.IsNullOrEmpty(Barangay) &&
               !string.IsNullOrEmpty(Major) &&
               !string.IsNullOrEmpty(EmergencyContactName) &&
               !string.IsNullOrEmpty(EmergencyContactPhone);
    }

    // Navigation properties
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<ApprovalStep> ApprovalActions { get; set; } = new List<ApprovalStep>();
}
