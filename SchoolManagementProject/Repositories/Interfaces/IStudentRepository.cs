using SchoolManagementProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagementProject.Repositories
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<IEnumerable<Student>> GetStudentsByClassIdAsync(int classId);

        Task<IEnumerable<Student>> GetStudentsByStatusAsync(string status);

        Task<Student> GetStudentWithCoursesAsync(int studentId);

        Task<Student> GetByFullNameAsync(string fullName);

        Task<IEnumerable<Student>> GetAllWithIncludesAsync();

        Task<List<Student>> GetStudentsBySchoolClassIdAsync(int schoolClassId);

        Task<int?> GetStudentIdByUserIdAsync(string userId);

        Task<Student> GetStudentByUserIdAsync(string userId);

    }
}
