using SchoolManagementProject.Models;
using SchoolManagementProject.ViewModels;
using SchoolManagementProject.Models;

namespace SchoolManagementProject.Repositories
{
    public interface ISchoolClassRepository : IGenericRepository<SchoolClass>
    {
        Task<List<SchoolClass>> GetAvailableSchoolClassesAsync();
        Task<List<SchoolClass>> GetAllAsync();

        Task<List<SchoolClass>> GetSchoolClassesByIdsAsync(List<int> ids);

        Task<List<SchoolClass>> GetAllWithDetailsAsync();
        Task<SchoolClassViewModel> GetClassDetailsViewModelAsync(int id);

    }
}
