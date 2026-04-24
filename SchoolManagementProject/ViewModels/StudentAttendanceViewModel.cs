using SchoolManagementProject.Models;

namespace SchoolManagementProject.ViewModels
{
    public class StudentAttendanceViewModel
    {
        public Student Student { get; set; } 
        public List<Attendance> Attendances { get; set; } 

        public int TotalClasses { get; set; } 

        public int TotalAbsences => Attendances?.Count(a => a.SubjectId != 0) ?? 0; 

        public string OverallAttendanceStatus()
        {
            double allowedAbsences = TotalClasses * 0.3; 
            return TotalAbsences >= allowedAbsences ? "Failed" : "Passed"; 
        }
    }

}
