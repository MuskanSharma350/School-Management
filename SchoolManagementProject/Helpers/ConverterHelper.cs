using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using SchoolManagementProject.Models;
using SchoolManagementProject.ViewModels;
using SchoolManagementProject.Models;
using SchoolManagementProject.Repositories;

namespace SchoolManagementProject.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        private readonly IUserHelper _userHelper;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ISchoolClassRepository _schoolClassRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;        
        private readonly IGradeRepository _gradeRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public ConverterHelper(IUserHelper userHelper, ISchoolClassRepository schoolClassRepository, ISubjectRepository subjectRepository, ICourseRepository courseRepository, ITeacherRepository teacherRepository, IGradeRepository gradeRepository, IAttendanceRepository attendanceRepository)
        {
            _userHelper = userHelper;
            _schoolClassRepository = schoolClassRepository;
            _subjectRepository = subjectRepository;
            _courseRepository = courseRepository;
            _teacherRepository = teacherRepository;            
            _gradeRepository = gradeRepository;
            _attendanceRepository = attendanceRepository;
        }

        public async Task<Student> ToStudentAsync(StudentViewModel model, Guid imageId, bool isNew)
        {
            string? userId = null;
            if (!string.IsNullOrEmpty(model.UserId))
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);
                if (user != null)
                {
                    userId = user.Id;
                }
            }

            return new Student
            {
                Id = isNew ? 0 : model.Id, 
                UserId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EnrollmentDate = model.EnrollmentDate,
                Status = model.Status,
                SchoolClassId = model.SchoolClassId,
                SchoolClass = model.SchoolClass,
                ImageId = imageId
            };
        }

        public StudentViewModel ToStudentViewModel(Student student)
        {
            return new StudentViewModel
            {
                Id = student.Id,
                UserId = student.UserId,
                FirstName = student.FirstName, 
                LastName = student.LastName, 
                EnrollmentDate = student.EnrollmentDate, 
                Status = student.Status,
                SchoolClassId = student.SchoolClassId,
                SchoolClass = student.SchoolClass, 
                ImageId = student.ImageId 
            };
        }
        public async Task<Teacher> ToTeacherAsync(TeacherViewModel model, Guid imageId, bool isNew)
        {
            string? userId = null;
            if (!string.IsNullOrEmpty(model.UserId))
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);
                if (user != null)
                {
                    userId = user.Id;
                }
            }

            return new Teacher
            {
                Id = isNew ? 0 : model.Id,
                UserId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                AcademicDegree = model.AcademicDegree,
                HireDate = model.HireDate,
                Status = model.Status,
                ImageId = imageId,

                TeacherSchoolClasses = model.SchoolClassIds.Select(id => new TeacherSchoolClass
                {
                    SchoolClassId = id
                }).ToList(),

                TeacherSubjects = model.SubjectIds.Select(subjectId => new TeacherSubject
                {
                    TeacherId = isNew ? 0 : model.Id,
                    SubjectId = subjectId
                }).ToList()
            };
        }

        public TeacherViewModel ToTeacherViewModel(Teacher teacher)
        {
            return new TeacherViewModel
            {
                Id = teacher.Id,
                UserId = teacher.UserId,
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                AcademicDegree = teacher.AcademicDegree,
                HireDate = teacher.HireDate,
                ImageId = teacher.ImageId,
                Status = teacher.Status,

                SchoolClassIds = teacher.TeacherSchoolClasses.Select(tsc => tsc.SchoolClassId).ToList(),
                SubjectIds = teacher.TeacherSubjects.Select(ts => ts.SubjectId).ToList(),

                SchoolClasses = teacher.TeacherSchoolClasses.Select(tsc => tsc.SchoolClass).ToList(),
                Subjects = teacher.TeacherSubjects.Select(ts => ts.Subject).ToList()
            };
        }

        public async Task<Employee> ToEmployeeAsync(EmployeeViewModel model, Guid imageId, bool isNew)
        {
            string? userId = null;
            if (!string.IsNullOrEmpty(model.UserId))
            {
                var user = await _userHelper.GetUserByIdAsync(model.UserId);
                if (user != null)
                {
                    userId = user.Id;
                }
            }

            return new Employee
            {
                Id = isNew ? 0 : model.Id,
                UserId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Department = model.Department,
                AcademicDegree = model.AcademicDegree,
                HireDate = model.HireDate,
                PhoneNumber = model.PhoneNumber,
                ImageId = imageId,
                Status = model.Status
            };
        }

        public EmployeeViewModel ToEmployeeViewModel(Employee employee)
        {
            return new EmployeeViewModel
            {
                Id = employee.Id,
                UserId = employee.UserId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Department = employee.Department,
                AcademicDegree = employee.AcademicDegree,
                HireDate = employee.HireDate,
                PhoneNumber = employee.PhoneNumber,
                ImageId = employee.ImageId,
                Status = employee.Status
            };
        }

        public async Task<Course> ToCourseAsync(CourseViewModel model, bool isNew)
        {
            var course = isNew ? new Course() : await _courseRepository.GetCourseWithDetailsAsync(model.Id);

            if (course == null)
            {
                return null;
            }

            course.Name = model.Name;

            course.SchoolClasses = await _schoolClassRepository.GetSchoolClassesByIdsAsync(model.SelectedSchoolClassIds);

            var subjects = await _subjectRepository.GetSubjectsByIdsAsync(model.SelectedSubjectIds);

            course.CourseSubjects = subjects.Select(s => new CourseSubject
            {
                CourseId = course.Id,
                SubjectId = s.Id
            }).ToList();
                        
            course.Description = model.Description; 
            course.Duration = model.Duration; 
            course.IsActive = model.IsActive; 
            course.CreatedAt = model.CreatedAt;
            course.UpdatedAt = model.UpdatedAt;

            return course;
        }

        public CourseViewModel ToCourseViewModel(Course course)
        {
            var schoolClasses = _schoolClassRepository.GetAllAsync().Result; 
            var subjects = _subjectRepository.GetAllSubjectsAsync().Result; 

            return new CourseViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description, 
                Duration = course.Duration, 
                IsActive = course.IsActive, 
                CreatedAt = course.CreatedAt, 
                UpdatedAt = course.UpdatedAt, 
                SelectedSchoolClassIds = course.SchoolClasses.Select(sc => sc.Id).ToList(),
                SelectedSubjectIds = course.CourseSubjects.Select(cs => cs.SubjectId).ToList(),
                SchoolClasses = schoolClasses,
                Subjects = subjects,
                SchoolClassItems = schoolClasses.Select(sc => new SelectListItem
                {
                    Value = sc.Id.ToString(),
                    Text = sc.ClassName,
                    Selected = course.SchoolClasses.Any(s => s.Id == sc.Id)
                }).ToList(),
                SubjectItems = subjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                    Selected = course.CourseSubjects.Any(cs => cs.SubjectId == s.Id)
                }).ToList()
            };
        }

        public async Task<SchoolClass> ToSchoolClassAsync(SchoolClassViewModel model, bool isNew)
        {
            var schoolClass = isNew ? new SchoolClass() : await _schoolClassRepository.GetByIdAsync(model.Id);

            if (schoolClass == null)
            {
                return null;
            }

            schoolClass.ClassName = model.ClassName;
            schoolClass.CourseId = model.CourseId; 
            schoolClass.StartDate = model.StartDate;
            schoolClass.EndDate = model.EndDate;

            return schoolClass;
        }

        public SchoolClassViewModel ToSchoolClassViewModel(SchoolClass schoolClass)
        {
            return new SchoolClassViewModel
            {
                Id = schoolClass.Id,
                ClassName = schoolClass.ClassName,
                CourseId = schoolClass.CourseId,
                StartDate = schoolClass.StartDate ?? DateTime.Now, 
                EndDate = schoolClass.EndDate ?? DateTime.Now, 
            };
        }

        public async Task<Subject> ToSubjectAsync(SubjectViewModel model, bool isNew)
        {
            var subject = isNew ? new Subject() : await _subjectRepository.GetByIdAsync(model.Id);

            if (subject == null)
            {
                return null;
            }

            subject.Name = model.SubjectName;
            subject.Description = model.Description;
            subject.Credits = model.Credits;
            subject.TotalClasses = model.TotalClasses; 

            return subject;
        }


        public SubjectViewModel ToSubjectViewModel(Subject subject)
        {
            return new SubjectViewModel
            {
                Id = subject.Id,
                SubjectName = subject.Name,
                Description = subject.Description, 
                Credits = subject.Credits,
                TotalClasses = subject.TotalClasses
            };
        }

        public async Task<Grade> ToGradeAsync(GradeViewModel model, bool isNew)
        {
            var grade = isNew ? new Grade() : await _gradeRepository.GetByIdAsync(model.Id);

            if (grade == null)
            {
                return null; 
            }

            grade.Value = model.Value;
            grade.StudentId = model.StudentId; 
            grade.SubjectId = model.SubjectId; 
            grade.EvaluationDate = model.EvaluationDate; 

            return grade;
        }


        public GradeViewModel ToGradeViewModel(Grade grade)
        {
            return new GradeViewModel
            {
                Id = grade.Id,
                Value = grade.Value,
                StudentId = grade.StudentId,
                StudentName = $"{grade.Student.FirstName} {grade.Student.LastName}",
                SubjectId = grade.SubjectId,
                SubjectName = grade.Subject.Name,
                EvaluationDate = grade.EvaluationDate,
            };
        }

        public async Task<Attendance> ToAttendanceAsync(AttendanceViewModel model, bool isNew)
        {
            var attendance = isNew ? new Attendance() : await _attendanceRepository.GetByIdAsync(model.Id);

            if (attendance == null)
            {
                return null;
            }

            attendance.StudentId = model.StudentId;
            attendance.SubjectId = model.SubjectId;
            attendance.Description = model.Description;
            attendance.Date = model.Date;

            return attendance;
        }

        public AttendanceViewModel ToAttendanceViewModel(Attendance attendance)
        {
            return new AttendanceViewModel
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                StudentName = $"{attendance.Student.FirstName} {attendance.Student.LastName}",
                SubjectId = attendance.SubjectId,
                SubjectName = attendance.Subject.Name,
                Description = attendance.Description,
                Date = attendance.Date,
            };
        }
    }
}
