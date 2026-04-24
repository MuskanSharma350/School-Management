using SchoolManagementProject.Models;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagementProject.ViewModels
{
    public class StudentSubjectAttendanceViewModel
    {
        public Subject Subject { get; set; } 
        public Attendance Attendance { get; set; } 
        public int StudentId { get; set; }
        public string StudentName { get; set; } 

        public string Status => Attendance != null ? "Absent" : "Present";

        public List<Attendance> AllAttendances { get; set; }

        public int TotalAbsences => AllAttendances?.Count() ?? 0; 

        public int TotalClasses => Subject.TotalClasses; 

        public bool CanAddAttendance { get; set; } 

        public string SubjectAttendanceStatus()
        {
            if (TotalClasses == 0) return "No classes available";

            double allowedAbsences = TotalClasses * 0.2; 
            return TotalAbsences > allowedAbsences ? "Failed" : "Passed"; 
        }
    }
}
