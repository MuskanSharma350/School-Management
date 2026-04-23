using SchoolManagementSystem.Models;
using System;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Helpers
{
    public interface IConverterHelper
    {
        Task<Student> ToStudentAsync(StudentViewModel model, Guid imageId, bool isNew);

        StudentViewModel ToStudentViewModel(Student student);

        Task<Teacher> ToTeacherAsync(TeacherViewModel model, Guid imageId, bool isNew);

        TeacherViewModel ToTeacherViewModel(Teacher teacher);

        Task<Employee> ToEmployeeAsync(EmployeeViewModel model, Guid imageId, bool isNew);

        EmployeeViewModel ToEmployeeViewModel(Employee employee);

        Task<Course> ToCourseAsync(CourseViewModel model, bool isNew);

        CourseViewModel ToCourseViewModel(Course course);

        Task<SchoolClass> ToSchoolClassAsync(SchoolClassViewModel model, bool isNew);

        SchoolClassViewModel ToSchoolClassViewModel(SchoolClass schoolClass);

        Task<Subject> ToSubjectAsync(SubjectViewModel model, bool isNew);

        SubjectViewModel ToSubjectViewModel(Subject subject);

        Task<Grade> ToGradeAsync(GradeViewModel model, bool isNew);
        GradeViewModel ToGradeViewModel(Grade grade);

        Task<Attendance> ToAttendanceAsync(AttendanceViewModel model, bool isNew);
        AttendanceViewModel ToAttendanceViewModel(Attendance attendance);

    }
}
