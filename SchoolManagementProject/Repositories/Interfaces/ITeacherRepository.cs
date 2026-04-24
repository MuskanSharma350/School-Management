using SchoolManagementProject.Models;

namespace SchoolManagementProject.Repositories
{
    public interface ITeacherRepository : IGenericRepository<Teacher>
    {
        Task<IEnumerable<Teacher>> GetAllTeachersWithSubjectsAsync();

        Task<IEnumerable<Teacher>> GetTeachersByDisciplineAsync(int subjectId);

        Task<Teacher> GetTeacherByFullNameAsync(string fullName);

        Task<Teacher> GetTeacherWithSubjectsAsync(int teacherId);

        Task UpdateTeacherSubjectsAsync(int teacherId, IEnumerable<int> subjectIds);

        Task<IEnumerable<Teacher>> GetTeachersByStatusAsync(TeacherStatus status);

        Task<int> CountTeachersByDisciplineAsync(int subjectId);

        Task<IEnumerable<Teacher>> GetAllWithIncludesAsync();

        Task<IEnumerable<Teacher>> GetAllAsync();

        Task<Teacher> GetTeacherWithDetailsAsync(int teacherId);

        Task UpdateTeacherClassesAsync(int teacherId, IEnumerable<int> subjectIds);

        Task<Teacher> GetTeacherByUserIdAsync(string userId);
    }
}
