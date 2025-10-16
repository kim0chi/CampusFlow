namespace StudentEnrollmentSystem.Models.ViewModels;

public class ApprovalViewModel
{
    public int EnrollmentBatchId { get; set; }
    public bool IsApproved { get; set; }
    public string? Comments { get; set; }
}
