using SchoolManagementProject.Models;
using SchoolManagementProject.Models;

namespace SchoolManagementProject.ViewModels
{
    public class StudentSubjectGradeViewModel
    {
        public Subject Subject { get; set; }
        public Grade Grade { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } 

    }
}
