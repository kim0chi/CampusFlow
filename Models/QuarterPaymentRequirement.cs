using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentEnrollmentSystem.Models
{
    public class QuarterPaymentRequirement
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(450)]
        public string StudentId { get; set; } = string.Empty;

        public ApplicationUser? Student { get; set; }

        [Required]
        public int QuarterPeriodId { get; set; }

        public QuarterPeriod? QuarterPeriod { get; set; }

        /// <summary>
        /// Snapshot of the student's total balance (including old debts) when the quarter started
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalBalanceAtQuarterStart { get; set; }

        /// <summary>
        /// Required payment amount (30% of TotalBalanceAtQuarterStart)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal RequiredPaymentAmount { get; set; }

        /// <summary>
        /// Actual amount paid by student for this quarter
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualPaymentAmount { get; set; }

        /// <summary>
        /// True if student paid >= 30% OR has an approved promissory note
        /// </summary>
        public bool MeetsRequirement { get; set; }

        /// <summary>
        /// True if student is using a promissory note instead of paying
        /// </summary>
        public bool HasPromissoryNote { get; set; }

        /// <summary>
        /// Link to the promissory note if applicable
        /// </summary>
        public int? PromissoryNoteId { get; set; }

        public PromissoryNote? PromissoryNote { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Paid, PromissoryApproved, Overdue

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? PaidDate { get; set; }
    }
}
