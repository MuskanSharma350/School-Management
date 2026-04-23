using SchoolManagementSystem.Models;

namespace SchoolManagementSystem.Repositories
{
    public interface ISubjectRepository : IGenericRepository<Subject>
    {
        Task<List<Subject>> GetAllSubjectsAsync();

        Task<List<Subject>> GetSubjectsByIdsAsync(List<int> ids);

        Task<List<Subject>> GetSubjectsByStudentIdAsync(int studentId);
        Task<SubjectViewModel> GetSubjectDetailsViewModelAsync(int id);
    }
}
