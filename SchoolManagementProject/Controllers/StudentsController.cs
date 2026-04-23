using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using SchoolManagementSystem.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Employee,Admin")]

    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConverterHelper _converterHelper;
        private readonly ISchoolClassRepository _schoolClassRepository;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(IStudentRepository studentRepository, IWebHostEnvironment webHostEnvironment, IConverterHelper converterHelper, ISchoolClassRepository schoolClassRepository, IUserHelper userHelper, ILogger<StudentsController> logger)
        {
            _studentRepository = studentRepository;
            _webHostEnvironment = webHostEnvironment;
            _converterHelper = converterHelper;
            _schoolClassRepository = schoolClassRepository;
            _userHelper = userHelper;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _studentRepository.GetAllWithIncludesAsync();
            return View(students);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return new NotFoundViewResult("StudentNotFound");

            var student = await _studentRepository.GetStudentWithCoursesAsync(id.Value);
            if (student == null)
                return new NotFoundViewResult("StudentNotFound");

            return View(_converterHelper.ToStudentViewModel(student));
        }

        public async Task<IActionResult> Create()
        {
            var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");

            var model = new StudentViewModel
            {
                PendingUsers = pendingUsers 
            };

            ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");
            await LoadDropdownData();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SchoolClassId.HasValue)
                {
                    var schoolClass = await _schoolClassRepository.GetByIdAsync(model.SchoolClassId.Value);
                    if (schoolClass == null || schoolClass.CourseId == null)
                    {
                        TempData["ErrorMessage"] = "The selected school class does not have an associated course.";
                        ModelState.AddModelError("SchoolClassId", "The selected school class does not have an associated course.");
                        await LoadDropdownData();
                        var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
                        ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");
                        return View(model);  
                    }
                }

                try
                {
                    Guid imageId = Guid.Empty;
                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = Guid.NewGuid();
                        await SaveImageAsync(model.ImageFile, imageId);
                    }

                    var student = await _converterHelper.ToStudentAsync(model, imageId, true);
                    await _studentRepository.CreateAsync(student);

                    var user = await _userHelper.GetUserByIdAsync(model.UserId);
                    await _userHelper.RemoveUserFromRoleAsync(user, "Pending");
                    await _userHelper.AddUserToRoleAsync(user, "Student");

                    TempData["SuccessMessage"] = "Student created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating student");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            var updatedPendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
            ViewBag.PendingUsers = new SelectList(updatedPendingUsers, "Id", "Email");
            await LoadDropdownData();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return new NotFoundViewResult("StudentNotFound");

            var student = await _studentRepository.GetByIdAsync(id.Value);
            if (student == null) return new NotFoundViewResult("StudentNotFound");

            var model = _converterHelper.ToStudentViewModel(student);

            await LoadDropdownData();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StudentViewModel model)
        {
            if (id != model.Id) return new NotFoundViewResult("StudentNotFound");

            if (ModelState.IsValid)
            {
                if (model.SchoolClassId.HasValue)
                {
                    var schoolClass = await _schoolClassRepository.GetByIdAsync(model.SchoolClassId.Value);
                    if (schoolClass == null || schoolClass.CourseId == null)
                    {
                        TempData["ErrorMessage"] = "The selected school class does not have an associated course.";
                        ModelState.AddModelError("SchoolClassId", "The selected school class does not have an associated course.");
                        await LoadDropdownData();
                        var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
                        ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");
                        return View(model); 
                    }
                }

                try
                {
                    Guid imageId = model.ImageId; 

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        imageId = Guid.NewGuid();
                        await SaveImageAsync(model.ImageFile, imageId);
                    }

                    var student = await _converterHelper.ToStudentAsync(model, imageId, false);
                    await _studentRepository.UpdateAsync(student);

                    var user = await _userHelper.GetUserByIdAsync(model.UserId);
                    if (user != null)
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        await _userHelper.UpdateUserAsync(user);
                    }

                    TempData["SuccessMessage"] = "Student updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await StudentExists(model.Id)) return new NotFoundViewResult("StudentNotFound");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating student");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            var updatedPendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
            ViewBag.PendingUsers = new SelectList(updatedPendingUsers, "Id", "Email");
            await LoadDropdownData();
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return new NotFoundViewResult("StudentNotFound");

            var student = await _studentRepository.GetStudentWithCoursesAsync(id.Value); 

            if (student == null) return new NotFoundViewResult("StudentNotFound");

            return View(_converterHelper.ToStudentViewModel(student));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);

            if (student == null) return new NotFoundViewResult("StudentNotFound");

            try
            {
                await _studentRepository.DeleteAsync(student);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{student.Id} está a ser usado!";
                    ViewBag.ErrorMessage = $"Este estudante não pode ser apagado. Tente apagar primeiro as associações.";
                }

                return View("Error");
            }
        }

        private async Task<bool> StudentExists(int id)
        {
            return await _studentRepository.ExistAsync(id);
        }

        private async Task LoadDropdownData()
        {
            var schoolClasses = await _schoolClassRepository.GetAllAsync();
            var classList = schoolClasses.ToList();

            ViewBag.SchoolClassId = new SelectList(classList, "Id", "ClassName");
        }

        public IActionResult StudentNotFound()
        {
            return View();
        }
        private async Task SaveImageAsync(Microsoft.AspNetCore.Http.IFormFile imageFile, Guid imageId)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "students", $"{imageId}.jpg");

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
        }
        private void DeleteImage(Guid imageId)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "students", $"{imageId}.jpg");
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}
