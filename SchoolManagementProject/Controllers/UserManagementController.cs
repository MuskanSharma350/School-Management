using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Models;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly IUserHelper _userHelper;

        public UserManagementController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userHelper.GetAllUsersInRoleAsync("Pending");
            return View(users);
        }

        public async Task<IActionResult> AssignRole(string userId)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new AssignRoleViewModel
            {
                UserId = user.Id,
                Roles = new List<string> { "Admin", "Employee", "Student", "Anonymous" } // Available roles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            var user = await _userHelper.GetUserByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }

            await _userHelper.RemoveUserFromRoleAsync(user, "Pending");
            await _userHelper.AddUserToRoleAsync(user, model.SelectedRole);

            return RedirectToAction("Index");
        }
    }
}
