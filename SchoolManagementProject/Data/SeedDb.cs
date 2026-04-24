using SchoolManagementProject.Data;
using SchoolManagementProject.Models;
using SchoolManagementProject.Helpers;

public class SeedDb
{
    private readonly ApplicationDbContext _context;
    private readonly IUserHelper _userHelper;

    public SeedDb(ApplicationDbContext context, IUserHelper userHelper)
    {
        _context = context;
        _userHelper = userHelper;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();

        // Create roles and users
        await _userHelper.CheckRoleAsync("Admin");
        await _userHelper.CheckRoleAsync("Pending");

        // Create users
        var adminUser = await CreateUserAsync("admin@school.com", "Admin", "User", "Admin123!", "Admin");
        var pendingUser1 = await CreateUserAsync("pending1@school.com", "Pending", "User", "Pending123!", "Pending");
        var pendingUser2 = await CreateUserAsync("pending2@school.com", "Pending", "User", "Pending123!", "Pending");
    }

    private async Task<User> CreateUserAsync(string email, string firstName, string lastName, string password, string role)
    {
        var user = await _userHelper.GetUserByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                DateCreated = DateTime.UtcNow
            };

            await _userHelper.AddUserAsync(user, password);
            await _userHelper.AddUserToRoleAsync(user, role);
        }
        return user;
    }
}
