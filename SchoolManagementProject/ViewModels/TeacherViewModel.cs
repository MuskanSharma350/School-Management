using Microsoft.AspNetCore.Mvc.ModelBinding;
using SchoolManagementSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SchoolManagementSystem.Models
{
    public class TeacherViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pending User is required")]
        [Display(Name = "Pending User")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Academic Degree")]
        public AcademicDegree AcademicDegree { get; set; }

        public DateTime? HireDate { get; set; }

        public string FormattedHireDate => HireDate?.ToString("dd/MM/yyyy");

        public ICollection<int> SchoolClassIds { get; set; } = new List<int>();

        public ICollection<int> SubjectIds { get; set; } = new List<int>();

        public Guid ImageId { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
        ? "/images/teacher.png"
        : $"/images/teachers/{ImageId}.jpg";

        public TeacherStatus Status { get; set; } = TeacherStatus.Active;
        public IEnumerable<User>? PendingUsers { get; set; }
        public IEnumerable<SchoolClass>? SchoolClasses { get; set; }
        public IEnumerable<Subject>? Subjects { get; set; }
    }
}
