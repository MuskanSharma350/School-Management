using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using SchoolManagementSystem.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    public class TeachersController : Controller
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly ISchoolClassRepository _schoolClassRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<TeachersController> _logger;

        public TeachersController(ITeacherRepository teacherRepository, ISubjectRepository subjectRepository, ISchoolClassRepository schoolClassRepository, IWebHostEnvironment webHostEnvironment, IConverterHelper converterHelper, IUserHelper userHelper, ILogger<TeachersController> logger)
        {
            _teacherRepository = teacherRepository;
            _subjectRepository = subjectRepository;
            _schoolClassRepository = schoolClassRepository;
            _webHostEnvironment = webHostEnvironment;
            _converterHelper = converterHelper;
            _userHelper = userHelper; 
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _teacherRepository.GetAllWithIncludesAsync();
            return View(teachers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return new NotFoundViewResult("TeacherNotFound");

            var selectedTeacher = await _teacherRepository.GetTeacherWithDetailsAsync(id.Value);

            if (selectedTeacher == null) return new NotFoundViewResult("TeacherNotFound");

            var model = _converterHelper.ToTeacherViewModel(selectedTeacher);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");

            var model = new TeacherViewModel
            {
                PendingUsers = pendingUsers 
            };

            await LoadDropdownData();
            ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email"); 

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownData(); 
                var pendingUsers = await _userHelper.GetAllUsersInRoleAsync("Pending");
                ViewBag.PendingUsers = new SelectList(pendingUsers, "Id", "Email");
                return View(model);
            }

            try
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    imageId = Guid.NewGuid();
                    await SaveImageAsync(model.ImageFile, imageId);
                }

                var teacher = await _converterHelper.ToTeacherAsync(model, imageId, true);

                await _teacherRepository.CreateAsync(teacher);

                var user = await _userHelper.GetUserByIdAsync(model.UserId);
                await _userHelper.RemoveUserFromRoleAsync(user, "Pending");
                await _userHelper.AddUserToRoleAsync(user, "Teacher");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating teacher");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }

            await LoadDropdownData();
            var pendingUsersReload = await _userHelper.GetAllUsersInRoleAsync("Pending");
            ViewBag.PendingUsers = new SelectList(pendingUsersReload, "Id", "Email");
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return new NotFoundViewResult("TeacherNotFound");

            var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(id.Value);
            if (teacher == null) return new NotFoundViewResult("TeacherNotFound");

            var model = _converterHelper.ToTeacherViewModel(teacher);

            await LoadDropdownData();

            model.SchoolClassIds = teacher.TeacherSchoolClasses.Select(tsc => tsc.SchoolClassId).ToList();
            model.SubjectIds = teacher.TeacherSubjects.Select(ts => ts.SubjectId).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherViewModel model)
        {
            if (id != model.Id) return new NotFoundViewResult("TeacherNotFound");

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

                    var teacher = await _converterHelper.ToTeacherAsync(model, imageId, false);

                    await _teacherRepository.UpdateTeacherSubjectsAsync(teacher.Id, model.SubjectIds);
                    await _teacherRepository.UpdateTeacherClassesAsync(teacher.Id, model.SchoolClassIds);

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
                    if (!await TeacherExists(model.Id)) return new NotFoundViewResult("TeacherNotFound");
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating teacher");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            await LoadDropdownData();
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return new NotFoundViewResult("TeacherNotFound");

            var teacher = await _teacherRepository.GetTeacherWithDetailsAsync(id.Value);
            if (teacher == null) return new NotFoundViewResult("TeacherNotFound");

            var model = _converterHelper.ToTeacherViewModel(teacher);

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher == null) return new NotFoundViewResult("TeacherNotFound");

            try
            {
                await _teacherRepository.DeleteAsync(teacher);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{teacher.FirstName} {teacher.LastName} is being used!";
                    ViewBag.ErrorMessage = "This teacher cannot be deleted because it has associated data.";
                }
                return View("Error");
            }
        }

        private async Task<bool> TeacherExists(int id)
        {
            return await _teacherRepository.ExistAsync(id);
        }

        private async Task LoadDropdownData()
        {
            var subjects = await _subjectRepository.GetAllSubjectsAsync();
            var classes = await _schoolClassRepository.GetAllAsync();

            ViewBag.Subjects = subjects.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name 
            });

            ViewBag.SchoolClasses = classes.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.ClassName 
            });
        }

        public IActionResult TeacherNotFound()
        {
            return View();
        }
        private async Task SaveImageAsync(Microsoft.AspNetCore.Http.IFormFile imageFile, Guid imageId)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "teachers", $"{imageId}.jpg");

            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
        }
        private void DeleteImage(Guid imageId)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "images", "teachers", $"{imageId}.jpg");
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}
