using Microsoft.EntityFrameworkCore;
using SchoolManagementProject.Data;
using SchoolManagementProject.Models;
using SchoolManagementProject.Models;

namespace SchoolManagementProject.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllWithIncludesAsync()
        {
            return await _context.Students.Include(s => s.User).Include(s => s.SchoolClass).AsNoTracking().ToListAsync();
        }

        public async Task<Student> GetByFullNameAsync(string fullName)
        {
            return await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => $"{s.User.FirstName} {s.User.LastName}" == fullName);
        }

        public async Task<IEnumerable<Student>> GetStudentsByClassIdAsync(int classId)
        {
            return await _context.Students.Where(s => s.SchoolClassId == classId).Include(s => s.SchoolClass).Include(s => s.User).ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsByStatusAsync(string status)
        {
            if (Enum.TryParse<StudentStatus>(status, out var studentStatus))
            {
                return await _context.Students.Where(s => s.Status == studentStatus).Include(s => s.SchoolClass).Include(s => s.User).ToListAsync();
            }
            else
            {
                return new List<Student>();
            }
        }

        public async Task<Student> GetStudentWithCoursesAsync(int studentId)
        {
            return await _context.Students.Include(s => s.SchoolClass).ThenInclude(c => c.Course).Include(s => s.User).FirstOrDefaultAsync(s => s.Id == studentId);
        }

        public async Task<List<Student>> GetStudentsBySchoolClassIdAsync(int schoolClassId)
        {
            return await _context.Students.Include(s => s.Grades).Where(s => s.SchoolClassId == schoolClassId).ToListAsync();
        }

        public async Task<int?> GetStudentIdByUserIdAsync(string userId)
        {
            
            var student = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.UserId == userId); 

            return student?.Id; 
        }

        public async Task<Student> GetStudentByUserIdAsync(string userId)
        {
            return await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.UserId == userId);
        }

    }
}
