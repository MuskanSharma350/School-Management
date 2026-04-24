using System.ComponentModel.DataAnnotations;

namespace SchoolManagementProject.Models
{
    public class SchoolClass : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string ClassName { get; set; }

        public int? CourseId { get; set; }
        public Course Course { get; set; } 

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<TeacherSchoolClass> TeacherSchoolClasses { get; set; } = new List<TeacherSchoolClass>();
    }
}
