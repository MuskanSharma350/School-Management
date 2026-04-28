using Microsoft.AspNetCore.Identity;
using SchoolManagementProject.Models;
using SchoolManagementProject.ViewModels;
using SchoolManagementProject.Models;

namespace SchoolManagementProject.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<User> GetUserByIdAsync(string userId);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        Task RemoveUserFromRoleAsync(User user, string roleName);

        Task<List<User>> GetAllUsersInRoleAsync(string roleName);

        Task NotifyAdministrativeEmployeesPendingUserAsync(User user);

        Task<IdentityResult> ResetPasswordWithoutTokenAsync(User user, string password);

        Task<string> GetRoleAsync(User user);

        Task UpdateUserDataByRoleAsync(User user);

        Task<Employee> GetEmployeeByUserAsync(string userEmail);
        Task<IdentityResult> CreateUserWithProfileAsync(User user, string password, string role);
    }
}
