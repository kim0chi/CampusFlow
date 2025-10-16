namespace StudentEnrollmentSystem.Models.Enums;

public enum EnrollmentStatus
{
    Submitted,
    DeanReview,
    AwaitingPayment,           // Waiting for student to pay or submit promissory note
    PromissoryNoteSubmitted,   // Student submitted promissory note
    CampusDirectorReview,      // Campus Director reviewing promissory note
    AccountingReview,
    SAOReview,
    LibraryReview,
    RecordsReview,
    Enrolled,      // Currently taking the course (in progress)
    Completed,     // Passed the course
    Failed,        // Failed the course
    Rejected       // Enrollment request was rejected
}
