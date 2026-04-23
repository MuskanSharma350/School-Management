using SchoolManagementSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementProject.Data;

namespace SchoolManagementSystem.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(Department department)
        {
            return await _context.Employees.Where(e => e.Department == department).ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByStatusAsync(EmployeeStatus status)
        {
            return await _context.Employees.Where(e => e.Status == status).ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetRecentlyHiredEmployeesAsync()
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            return await _context.Employees.Where(e => e.HireDate >= thirtyDaysAgo).ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetAdministrativeEmployeesAsync()
        {
            return await _context.Employees.Where(e => e.Department == Department.Administration || e.Department == Department.HumanResources).ToListAsync();
        }

        public async Task<bool> CanEmployeeManageUserCreationAsync(int employeeId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
            {
                return false;
            }

            return employee.Status == EmployeeStatus.Active && (employee.Department == Department.Administration || employee.Department == Department.HumanResources);
        }

        public async Task<int> CountEmployeesByDepartmentAsync(Department department)
        {
            return await _context.Employees.CountAsync(e => e.Department == department);
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithCompleteProfileAsync()
        {
            return await _context.Employees.Where(e => !string.IsNullOrEmpty(e.FirstName) && !string.IsNullOrEmpty(e.LastName) && !string.IsNullOrEmpty(e.PhoneNumber) && e.HireDate != null && e.Department != null).ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllWithIncludesAsync()
        {
            return await _context.Employees.Include(e => e.User).ToListAsync();
        }

        public async Task<Employee> GetEmployeeByUserIdAsync(string userId)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        }
    }
}
