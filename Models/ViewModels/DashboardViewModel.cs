namespace StudentEnrollmentSystem.Models.ViewModels;

public class DashboardViewModel
{
    public List<Enrollment> PendingApprovals { get; set; } = new List<Enrollment>();
    public List<Enrollment> MyEnrollments { get; set; } = new List<Enrollment>();
    public int PendingCount { get; set; }
    public string RoleName { get; set; } = string.Empty;
}
