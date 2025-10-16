namespace StudentEnrollmentSystem.Services;

public interface IStudentNumberService
{
    Task<string> GenerateStudentNumberAsync(int departmentId);
}
