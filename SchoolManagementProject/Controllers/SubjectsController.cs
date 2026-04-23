using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    [Authorize(Roles = "Employee,Admin")]

    public class SubjectsController : Controller
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IConverterHelper _converterHelper;

        public SubjectsController(ISubjectRepository subjectRepository, IConverterHelper converterHelper)
        {
            _subjectRepository = subjectRepository;
            _converterHelper = converterHelper;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectRepository.GetAll().ToListAsync();
            var subjectViewModels = subjects.Select(s => _converterHelper.ToSubjectViewModel(s)).ToList();
            return View(subjectViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subject = await _converterHelper.ToSubjectAsync(model, true);
                await _subjectRepository.CreateAsync(subject);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                return new NotFoundViewResult("SubjectNotFound");
            }

            var model = _converterHelper.ToSubjectViewModel(subject);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subject = await _converterHelper.ToSubjectAsync(model, false);
                await _subjectRepository.UpdateAsync(subject);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                return new NotFoundViewResult("SubjectNotFound");
            }
            var model = _converterHelper.ToSubjectViewModel(subject);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null)
            {
                return new NotFoundViewResult("SubjectNotFound");
            }

            try
            {
                await _subjectRepository.DeleteAsync(subject);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{subject.Name} is being used!";
                    ViewBag.ErrorMessage = "This subject cannot be deleted because it has associated data.";
                }
                return View("Error");
            }
        }

        public IActionResult SubjectNotFound()
        {
            return View();
        }
        public async Task<IActionResult> Details(int id)
        {
            var model = await _subjectRepository.GetSubjectDetailsViewModelAsync(id);
            if (model == null)
            {
                return new NotFoundViewResult("SubjectNotFound");
            }

            return View(model);
        }
    }
}
