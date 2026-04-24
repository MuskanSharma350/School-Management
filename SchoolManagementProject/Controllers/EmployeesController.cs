using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementProject.Models;
using SchoolManagementProject.Repositories;
using SchoolManagementProject.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementProject.ViewModels;

namespace SchoolManagementProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeRepository employeeRepository, IWebHostEnvironment webHostEnvironment, IConverterHelper converterHelper, IUserHelper userHelper, ILogger<EmployeesController> logger)
        {
            _employeeRepository = employeeRepository;
            _webHostEnvironment = webHostEnvironment;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeRepository.GetAllWithIncludesAsync();
            return View(employees);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return new NotFoundViewResult("EmployeeNotFound");

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null) return new NotFoundViewResult("EmployeeNotFound");

            var model = _converterHelper.ToEmployeeViewModel(employee);
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");

            var model = new EmployeeViewModel
            {
                PendingUsers = pendingUsers 
            };

            ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = Guid.Empty;

                    
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = Guid.NewGuid();
                        await SaveImageAsync(model.ImageFile, imageId);
                    }

                    
                    var employee = await _converterHelper.ToEmployeeAsync(model, imageId, true);

                   
                    await _employeeRepository.CreateAsync(employee);

                    if (!string.IsNullOrEmpty(model.UserId))
                    {
                        var user = await _userHelper.GetUserByIdAsync(model.UserId);
                        if (user != null)
                        {
                            await _userHelper.RemoveUserFromRoleAsync(user, "Pending");
                            await _userHelper.AddUserToRoleAsync(user, "Employee");
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating employee");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
            ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return new NotFoundViewResult("EmployeeNotFound");

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null) return new NotFoundViewResult("EmployeeNotFound");

            var model = _converterHelper.ToEmployeeViewModel(employee);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel model)
        {
            if (id != model.Id) return new NotFoundViewResult("EmployeeNotFound");

            if (ModelState.IsValid)
            {
                try
                {
                    Guid imageId = model.ImageId; 

 
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = Guid.NewGuid();
                        await SaveImageAsync(model.ImageFile, imageId);
                    }

                    var employee = await _converterHelper.ToEmployeeAsync(model, imageId, false);
                    await _employeeRepository.UpdateAsync(employee);

                    var user = await _userHelper.GetUserByIdAsync(model.UserId);
                    if (user != null)
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        await _userHelper.UpdateUserAsync(user);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EmployeeExists(model.Id)) return new NotFoundViewResult("EmployeeNotFound");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating employee");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return new NotFoundViewResult("EmployeeNotFound");

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null) return new NotFoundViewResult("EmployeeNotFound");

            return View(_converterHelper.ToEmployeeViewModel(employee));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return new NotFoundViewResult("EmployeeNotFound");

            try
            {
                if (employee.ImageId != Guid.Empty)
                {
                    DeleteImage(employee.ImageId);
                }
                await _employeeRepository.DeleteAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{employee.Id} is being used!";
                    ViewBag.ErrorMessage = "This employee cannot be deleted. Please delete associations first.";
                }

                return View("Error");
            }
        }

        private async Task<bool> EmployeeExists(int id)
        {
            return await _employeeRepository.ExistAsync(id);
        }

        public IActionResult EmployeeNotFound()
        {
            return View();
        }
        private async Task SaveImageAsync(Microsoft.AspNetCore.Http.IFormFile imageFile, Guid imageId)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "employees", $"{imageId}.jpg");

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
        }
        private void DeleteImage(Guid imageId)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "employees", $"{imageId}.jpg");
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}
