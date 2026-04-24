using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementProject.Data;
using SchoolManagementProject.Models;
using SchoolManagementProject.Models;

namespace SchoolManagementProject.Repositories
{
    public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
    {
        private readonly ApplicationDbContext _context;

        public TeacherRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersWithSubjectsAsync()
        {
            return await _context.Teachers.Include(t => t.TeacherSubjects).ThenInclude(ts => ts.Subject).ToListAsync();
        }

        public async Task<IEnumerable<Teacher>> GetTeachersByDisciplineAsync(int subjectId)
        {
            return await _context.Teachers.Where(t => t.TeacherSubjects.Any(ts => ts.SubjectId == subjectId)).ToListAsync();
        }

        public async Task<Teacher> GetTeacherByFullNameAsync(string fullName)
        {
            var names = fullName.Split(' ');
            return await _context.Teachers.FirstOrDefaultAsync(t => t.FirstName == names[0] && t.LastName == names[1]);
        }

        public async Task<Teacher> GetTeacherWithSubjectsAsync(int teacherId)
        {
            return await _context.Teachers.Include(t => t.TeacherSubjects).ThenInclude(ts => ts.Subject).FirstOrDefaultAsync(t => t.Id == teacherId);
        }

        public async Task UpdateTeacherSubjectsAsync(int teacherId, IEnumerable<int> subjectIds)
        {
            var teacher = await _context.Teachers.Include(t => t.TeacherSubjects).FirstOrDefaultAsync(t => t.Id == teacherId);

            if (teacher != null)
            {
                teacher.TeacherSubjects.Clear();

                foreach (var subjectId in subjectIds)
                {
                    teacher.TeacherSubjects.Add(new TeacherSubject { TeacherId = teacherId, SubjectId = subjectId });
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTeacherClassesAsync(int teacherId, IEnumerable<int> schoolClassIds)
        {
            var teacher = await _context.Teachers.Include(t => t.TeacherSchoolClasses).FirstOrDefaultAsync(t => t.Id == teacherId);

            if (teacher != null)
            {
                teacher.TeacherSchoolClasses.Clear();

                foreach (var schoolClassId in schoolClassIds)
                {
                    teacher.TeacherSchoolClasses.Add(new TeacherSchoolClass { TeacherId = teacherId, SchoolClassId = schoolClassId });
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Teacher>> GetTeachersByStatusAsync(TeacherStatus status)
        {
            return await _context.Teachers.Where(t => t.Status == status).ToListAsync();
        }

        public async Task<int> CountTeachersByDisciplineAsync(int subjectId)
        {
            return await _context.Teachers.CountAsync(t => t.TeacherSubjects.Any(ts => ts.SubjectId == subjectId));
        }

        public async Task<IEnumerable<Teacher>> GetAllWithIncludesAsync()
        {
            return await _context.Teachers.Include(t => t.TeacherSubjects).ThenInclude(ts => ts.Subject).Include(t => t.TeacherSchoolClasses).ThenInclude(tsc => tsc.SchoolClass).ToListAsync();
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            return await _context.Teachers.ToListAsync();
        }

        public async Task<Teacher> GetTeacherWithDetailsAsync(int teacherId)
        {
            return await _context.Teachers.Include(t => t.TeacherSchoolClasses).ThenInclude(tsc => tsc.SchoolClass).Include(t => t.TeacherSubjects).ThenInclude(ts => ts.Subject).FirstOrDefaultAsync(t => t.Id == teacherId);
        }

        public async Task<Teacher> GetTeacherByUserIdAsync(string userId)
        {
            return await _context.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId == userId);
        }
    }
}
