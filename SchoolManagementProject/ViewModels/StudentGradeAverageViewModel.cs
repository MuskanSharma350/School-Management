using SchoolManagementProject.Models;
using System.Linq;

namespace SchoolManagementProject.ViewModels
{
    public class StudentGradeAverageViewModel
    {
        public Student Student { get; set; }

        public double AverageGrade => Student?.Grades != null && Student.Grades.Any()
            ? Student.Grades.Average(g => g.Value)
            : 0;

        public string Status => AverageGrade >= 9.5 ? "Passed" : "Failed";
    }
}
