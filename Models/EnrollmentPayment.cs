using System.ComponentModel.DataAnnotations;

namespace StudentEnrollmentSystem.Models;

public class EnrollmentPayment
{
    public int Id { get; set; }

    [Required]
    public int EnrollmentBatchId { get; set; }

    public int? PaymentId { get; set; } // Null if using promissory note

    public int? PromissoryNoteId { get; set; } // Null if paid

    public decimal AmountPaid { get; set; } = 0;

    public decimal StudentBalance { get; set; } = 0; // Balance at time of enrollment

    public decimal PaymentPercentage { get; set; } = 0; // Percentage paid

    public bool MeetsRequirement { get; set; } = false; // True if >= 30% OR approved promissory note

    public bool UsingPromissoryNote { get; set; } = false;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public EnrollmentBatch EnrollmentBatch { get; set; } = null!;
    public Payment? Payment { get; set; }
    public PromissoryNote? PromissoryNote { get; set; }
}
