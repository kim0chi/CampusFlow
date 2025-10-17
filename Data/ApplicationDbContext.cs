using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Models;

namespace StudentEnrollmentSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<EnrollmentBatch> EnrollmentBatches { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<ApprovalStep> ApprovalSteps { get; set; }
    public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
    public DbSet<Models.Program> Programs { get; set; }
    public DbSet<ProgramCourse> ProgramCourses { get; set; }
    public DbSet<Models.Department> Departments { get; set; }
    public DbSet<StudentAccount> StudentAccounts { get; set; }
    public DbSet<Fee> Fees { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PromissoryNote> PromissoryNotes { get; set; }
    public DbSet<EnrollmentPayment> EnrollmentPayments { get; set; }
    public DbSet<AcademicYear> AcademicYears { get; set; }
    public DbSet<EnrollmentPeriod> EnrollmentPeriods { get; set; }
    public DbSet<QuarterPeriod> QuarterPeriods { get; set; }
    public DbSet<QuarterPaymentRequirement> QuarterPaymentRequirements { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Course
        builder.Entity<Course>()
            .HasIndex(c => c.CourseCode)
            .IsUnique();

        // Configure AcademicYear
        builder.Entity<AcademicYear>()
            .HasIndex(ay => ay.Year)
            .IsUnique();

        // Configure EnrollmentBatch
        builder.Entity<EnrollmentBatch>()
            .HasOne(eb => eb.Student)
            .WithMany()
            .HasForeignKey(eb => eb.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<EnrollmentBatch>()
            .HasIndex(eb => new { eb.StudentId, eb.Semester, eb.AcademicYear, eb.SubmittedDate });

        // Configure Enrollment
        builder.Entity<Enrollment>()
            .HasOne(e => e.EnrollmentBatch)
            .WithMany(eb => eb.Enrollments)
            .HasForeignKey(e => e.EnrollmentBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Enrollment
        builder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Enrollment>()
            .HasOne(e => e.GradedBy)
            .WithMany()
            .HasForeignKey(e => e.GradedById)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Enrollment>()
            .HasIndex(e => new { e.StudentId, e.CourseId });

        // Configure ApprovalStep
        builder.Entity<ApprovalStep>()
            .HasOne(a => a.EnrollmentBatch)
            .WithMany(eb => eb.ApprovalSteps)
            .HasForeignKey(a => a.EnrollmentBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApprovalStep>()
            .HasOne(a => a.Approver)
            .WithMany(u => u.ApprovalActions)
            .HasForeignKey(a => a.ApproverId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ApprovalStep>()
            .HasIndex(a => new { a.EnrollmentBatchId, a.StepType })
            .IsUnique();

        // Configure CoursePrerequisite
        builder.Entity<CoursePrerequisite>()
            .HasOne(cp => cp.Course)
            .WithMany(c => c.PrerequisiteRelations)
            .HasForeignKey(cp => cp.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CoursePrerequisite>()
            .HasOne(cp => cp.PrerequisiteCourse)
            .WithMany(c => c.DependentCourseRelations)
            .HasForeignKey(cp => cp.PrerequisiteCourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<CoursePrerequisite>()
            .HasIndex(cp => new { cp.CourseId, cp.PrerequisiteCourseId })
            .IsUnique();

        // Configure Program
        builder.Entity<Models.Program>()
            .HasIndex(p => p.Name)
            .IsUnique();

        // Configure ProgramCourse
        builder.Entity<ProgramCourse>()
            .HasOne(pc => pc.Program)
            .WithMany(p => p.ProgramCourses)
            .HasForeignKey(pc => pc.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProgramCourse>()
            .HasOne(pc => pc.Course)
            .WithMany()
            .HasForeignKey(pc => pc.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProgramCourse>()
            .HasIndex(pc => new { pc.ProgramId, pc.CourseId })
            .IsUnique();

        // Configure Department
        builder.Entity<Models.Department>()
            .HasIndex(d => d.Code)
            .IsUnique();

        // Configure ApplicationUser - Department relationship
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure ApplicationUser - ApprovedBy relationship (self-referencing)
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.ApprovedBy)
            .WithMany()
            .HasForeignKey(u => u.ApprovedById)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure StudentAccount
        builder.Entity<StudentAccount>()
            .HasOne(sa => sa.Student)
            .WithMany()
            .HasForeignKey(sa => sa.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StudentAccount>()
            .HasIndex(sa => new { sa.StudentId, sa.Semester, sa.AcademicYear })
            .IsUnique();

        // Configure Fee
        builder.Entity<Fee>()
            .HasOne(f => f.Student)
            .WithMany()
            .HasForeignKey(f => f.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Fee>()
            .HasOne(f => f.StudentAccount)
            .WithMany(sa => sa.Fees)
            .HasForeignKey(f => f.StudentAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Fee>()
            .HasOne(f => f.IssuedBy)
            .WithMany()
            .HasForeignKey(f => f.IssuedById)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Payment
        builder.Entity<Payment>()
            .HasOne(p => p.Student)
            .WithMany()
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Payment>()
            .HasOne(p => p.StudentAccount)
            .WithMany(sa => sa.Payments)
            .HasForeignKey(p => p.StudentAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Payment>()
            .HasOne(p => p.ProcessedByCashier)
            .WithMany()
            .HasForeignKey(p => p.ProcessedByCashierId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Payment>()
            .HasIndex(p => p.ReferenceNumber)
            .IsUnique();

        // Configure PromissoryNote
        builder.Entity<PromissoryNote>()
            .HasOne(pn => pn.EnrollmentBatch)
            .WithMany()
            .HasForeignKey(pn => pn.EnrollmentBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PromissoryNote>()
            .HasOne(pn => pn.Student)
            .WithMany()
            .HasForeignKey(pn => pn.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PromissoryNote>()
            .HasOne(pn => pn.ReviewedByCampusDirector)
            .WithMany()
            .HasForeignKey(pn => pn.ReviewedByCampusDirectorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure EnrollmentPayment
        builder.Entity<EnrollmentPayment>()
            .HasOne(ep => ep.EnrollmentBatch)
            .WithOne(eb => eb.EnrollmentPayment)
            .HasForeignKey<EnrollmentPayment>(ep => ep.EnrollmentBatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EnrollmentPayment>()
            .HasOne(ep => ep.Payment)
            .WithMany(p => p.EnrollmentPayments)
            .HasForeignKey(ep => ep.PaymentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<EnrollmentPayment>()
            .HasOne(ep => ep.PromissoryNote)
            .WithMany()
            .HasForeignKey(ep => ep.PromissoryNoteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<EnrollmentPayment>()
            .HasIndex(ep => ep.EnrollmentBatchId)
            .IsUnique();

        // Configure EnrollmentPeriod
        builder.Entity<EnrollmentPeriod>()
            .HasOne(ep => ep.CreatedByUser)
            .WithMany()
            .HasForeignKey(ep => ep.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<EnrollmentPeriod>()
            .HasIndex(ep => new { ep.Semester, ep.AcademicYear, ep.IsActive });

        // Configure QuarterPeriod
        builder.Entity<QuarterPeriod>()
            .HasOne(qp => qp.CreatedByUser)
            .WithMany()
            .HasForeignKey(qp => qp.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<QuarterPeriod>()
            .HasIndex(qp => new { qp.Quarter, qp.Semester, qp.AcademicYear })
            .IsUnique();

        // Configure QuarterPaymentRequirement
        builder.Entity<QuarterPaymentRequirement>()
            .HasOne(qpr => qpr.Student)
            .WithMany()
            .HasForeignKey(qpr => qpr.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<QuarterPaymentRequirement>()
            .HasOne(qpr => qpr.QuarterPeriod)
            .WithMany(qp => qp.PaymentRequirements)
            .HasForeignKey(qpr => qpr.QuarterPeriodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<QuarterPaymentRequirement>()
            .HasOne(qpr => qpr.PromissoryNote)
            .WithOne(pn => pn.QuarterPaymentRequirement)
            .HasForeignKey<QuarterPaymentRequirement>(qpr => qpr.PromissoryNoteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<QuarterPaymentRequirement>()
            .HasIndex(qpr => new { qpr.StudentId, qpr.QuarterPeriodId })
            .IsUnique();
    }
}

