using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class Student : IEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Enrollment Date")]
        public DateTime? EnrollmentDate { get; set; }
        
        public string FormattedEnrollmentDate => EnrollmentDate?.ToString("dd/MM/yyyy");


        [Display(Name = "Status")]
        public StudentStatus Status { get; set; } = StudentStatus.Pending;

        public int? SchoolClassId { get; set; } 
        public SchoolClass SchoolClass { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();


        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        [JsonProperty] 
    public string ImageFullPath => ImageId == Guid.Empty
       ? "/images/student.png"
       : $"/images/students/{ImageId}.jpg";
    }

    public enum StudentStatus
    {
        Pending,
        Active,
        Inactive
    }
}
