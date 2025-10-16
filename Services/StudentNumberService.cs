using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;

namespace StudentEnrollmentSystem.Services;

public class StudentNumberService : IStudentNumberService
{
    private readonly ApplicationDbContext _context;

    public StudentNumberService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateStudentNumberAsync(int departmentId)
    {
        // Get department code
        var department = await _context.Departments.FindAsync(departmentId);
        if (department == null)
        {
            throw new ArgumentException("Department not found", nameof(departmentId));
        }

        var currentYear = DateTime.Now.Year;
        var deptCode = department.Code;

        // Find the highest student number for this year and department
        var prefix = $"{currentYear}-{deptCode}-";
        var existingNumbers = await _context.Users
            .Where(u => u.StudentNumber != null && u.StudentNumber.StartsWith(prefix))
            .Select(u => u.StudentNumber)
            .ToListAsync();

        int maxNumber = 0;
        foreach (var studentNumber in existingNumbers)
        {
            // Extract the sequential number (last part)
            var parts = studentNumber.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int number))
            {
                if (number > maxNumber)
                {
                    maxNumber = number;
                }
            }
        }

        // Generate new number
        var nextNumber = maxNumber + 1;
        return $"{currentYear}-{deptCode}-{nextNumber:D4}";
    }
}
