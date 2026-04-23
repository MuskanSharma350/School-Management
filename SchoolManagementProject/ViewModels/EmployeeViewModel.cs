using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pending User is required")]
        [Display(Name = "Pending User")]
        public string UserId { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Department is required")]
        public Department Department { get; set; } 

        [Display(Name = "Academic Degree")]
        public AcademicDegree AcademicDegree { get; set; }

        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; }

        [Display(Name = "Status")]
        public IEnumerable<User>? PendingUsers { get; set; }
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }

        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
       ? "/images/employee.png"
       : $"/images/employees/{ImageId}.jpg";
    }
}
