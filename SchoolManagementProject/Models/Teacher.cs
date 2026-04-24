using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementProject.Models
{
    public class Teacher : IEntity
    {
        public int Id { get; set; }
        public string? UserId { get; set; }

        public User? User { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Academic Degree")]
        public AcademicDegree AcademicDegree { get; set; }

        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; }

        public string FormattedHireDate => HireDate?.ToString("dd/MM/yyyy");

        [Display(Name = "Status")]
        public TeacherStatus Status { get; set; } = TeacherStatus.Active;

        public ICollection<TeacherSchoolClass> TeacherSchoolClasses { get; set; } = new List<TeacherSchoolClass>();

        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        [JsonProperty] 
        public string ImageFullPath => ImageId == Guid.Empty
       ? "/images/teacher.png"
       : $"/images/teachers/{ImageId}.jpg";
    }

    public enum TeacherStatus
    {
        Pending,
        Active,
        Inactive
    }
}
