using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementProject.Data;
using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Repositories
{
    public class SchoolClassRepository : GenericRepository<SchoolClass>, ISchoolClassRepository
    {
        private readonly ApplicationDbContext _context;

        public SchoolClassRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<SchoolClass>> GetAvailableSchoolClassesAsync()
        {
            return await _context.SchoolClasses.Where(sc => sc.CourseId == null).ToListAsync();
        }

        public async Task<List<SchoolClass>> GetAllAsync()
        {
            return await _context.SchoolClasses.Include(c => c.Students).ToListAsync();
        }

        public async Task<List<SchoolClass>> GetSchoolClassesByIdsAsync(List<int> ids)
        {
            return await _context.SchoolClasses.Where(sc => ids.Contains(sc.Id)).ToListAsync();
        }

        public async Task<List<SchoolClass>> GetAllWithDetailsAsync()
        {
            return await _context.SchoolClasses.Include(sc => sc.Course).ThenInclude(c => c.CourseSubjects).ThenInclude(cs => cs.Subject).ToListAsync();
        }
        public async Task<SchoolClassViewModel> GetClassDetailsViewModelAsync(int id)
        {
            var schoolClass = await _context.SchoolClasses.FirstOrDefaultAsync(c => c.Id == id);

            if (schoolClass == null)
            {
                return null;
            }

            return new SchoolClassViewModel
            {
                Id = schoolClass.Id,
                ClassName = schoolClass.ClassName,
                CourseId = schoolClass.CourseId,
                StartDate = schoolClass.StartDate,
                EndDate = schoolClass.EndDate,
                StudentIds = schoolClass.Students.Select(s => s.Id).ToList(),
                TeacherIds = schoolClass.TeacherSchoolClasses.Select(t => t.TeacherId).ToList()
            };

        }
    }
}