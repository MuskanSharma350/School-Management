using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolManagementProject.Data;
using SchoolManagementProject.Models;
using SchoolManagementProject.ViewModels;
using System.Threading.Tasks;

namespace SchoolManagementProject.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Course> GetCourseWithDetailsAsync(int id)
        {
            return await _context.Courses.Include(c => c.SchoolClasses).Include(c => c.CourseSubjects).ThenInclude(cs => cs.Subject).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Course>> GetAllWithDetailsAsync()
        {
            return await _context.Courses.Include(c => c.SchoolClasses).Include(c => c.CourseSubjects).ThenInclude(cs => cs.Subject).ToListAsync();
        }

        public async Task<List<CourseViewModel>> GetAllCourseViewModelsAsync()
        {
            var courses = await GetAllWithDetailsAsync();

            return courses.Select(c => new CourseViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description, 
                Duration = c.Duration, 
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                SchoolClassItems = c.SchoolClasses.Select(sc => new SelectListItem
                {
                    Value = sc.Id.ToString(),
                    Text = sc.ClassName
                }).ToList(),
                SubjectItems = c.CourseSubjects.Select(cs => new SelectListItem
                {
                    Value = cs.Subject.Id.ToString(),
                    Text = cs.Subject.Name
                }).ToList()
            }).ToList();
        }


        public async Task<CourseViewModel> GetCourseDetailsViewModelAsync(int id)
        {
            var course = await _context.Courses.Include(c => c.SchoolClasses).Include(c => c.CourseSubjects).ThenInclude(cs => cs.Subject).FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return null; 
            }

            return new CourseViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description, 
                Duration = course.Duration, 
                IsActive = course.IsActive, 
                CreatedAt = course.CreatedAt, 
                UpdatedAt = course.UpdatedAt, 
                SchoolClassItems = course.SchoolClasses.Select(sc => new SelectListItem
                {
                    Value = sc.Id.ToString(),
                    Text = sc.ClassName
                }).ToList(),
                SubjectItems = course.CourseSubjects.Select(cs => new SelectListItem
                {
                    Value = cs.Subject.Id.ToString(),
                    Text = cs.Subject.Name
                }).ToList()
            };
        }


        public async Task<List<Course>> GetCoursesByIdsAsync(List<int> ids)
        {
            return await _context.Courses.Where(c => ids.Contains(c.Id)).Include(c => c.SchoolClasses).Include(c => c.CourseSubjects).ThenInclude(cs => cs.Subject).ToListAsync();
        }

    }

}

