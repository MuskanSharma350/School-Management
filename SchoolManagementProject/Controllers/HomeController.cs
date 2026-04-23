using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementProject.Data;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Repositories;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICourseRepository _courseRepository;
        private readonly ISchoolClassRepository _schoolClassRepository;
        private readonly IAlertRepository _alertRepository;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ICourseRepository courseRepository, ISchoolClassRepository schoolClassRepository, IAlertRepository alertRepository, ApplicationDbContext context)  
        {
            _logger = logger;
            _courseRepository = courseRepository;
            _schoolClassRepository = schoolClassRepository;
            _alertRepository = alertRepository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.StudentCount = _context.Students.Count();
            ViewBag.TeacherCount = _context.Teachers.Count();
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Courses()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
