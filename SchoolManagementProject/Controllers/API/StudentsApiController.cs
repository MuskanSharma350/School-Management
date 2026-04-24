using Microsoft.AspNetCore.Mvc;
using SchoolManagementProject.Repositories;
using SchoolManagementProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagementProject.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsApiController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentsApiController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet("BySchoolClass/{schoolClassId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetStudentsBySchoolClass(int schoolClassId)
        {
            var students = await _studentRepository.GetStudentsBySchoolClassIdAsync(schoolClassId);

            if (students == null || !students.Any())
            {
                return NotFound("No students found for the specified class.");
            }

            var studentViewModels = students.Select(student => new
            {
                student.Id,
                student.FirstName,
                student.LastName,
                student.Status,
                student.EnrollmentDate,
                student.SchoolClassId,
                ImageUrl = student.ImageFullPath 
            });

            return Ok(studentViewModels);
        }
    }
}
