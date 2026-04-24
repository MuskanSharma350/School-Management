using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementProject.Data;
using SchoolManagementProject.Models;
using SchoolManagementProject.ViewModels;
using SchoolManagementProject.Models;

namespace SchoolManagementProject.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        private readonly ApplicationDbContext _context;

        public SubjectRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<List<Subject>> GetSubjectsByIdsAsync(List<int> ids)
        {
            return await _context.Subjects
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();
        }

        public async Task<List<Subject>> GetSubjectsByStudentIdAsync(int studentId)
        {
            var schoolClassId = await _context.Students.Where(s => s.Id == studentId).Select(s => s.SchoolClassId).FirstOrDefaultAsync();

            if (schoolClassId == null)  return new List<Subject>(); 

            return await _context.CourseSubjects.Where(cs => cs.Course.SchoolClasses.Any(sc => sc.Id == schoolClassId)).Select(cs => cs.Subject).ToListAsync();
        }
        public async Task<SubjectViewModel> GetSubjectDetailsViewModelAsync(int id)
        {
            var subject = await _context.Subjects.FirstOrDefaultAsync(c => c.Id == id);

            if (subject == null)
            {
                return null;
            }

            return new SubjectViewModel
            {
                Id = subject.Id,
                SubjectName = subject.Name,
                Description = subject.Description,
                Credits = subject.Credits,
                TotalClasses = subject.TotalClasses
                
            };
        }

    }

}
