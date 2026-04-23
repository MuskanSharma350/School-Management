using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models
{
    public class Employee : IEntity
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

        [Display(Name = "Department")]
        public Department Department { get; set; }  

        [Display(Name = "Academic Degree")]
        public AcademicDegree AcademicDegree { get; set; }

        [Display(Name = "Hire Date")]
        public DateTime? HireDate { get; set; }

        public string FormattedHireDate => HireDate?.ToString("dd/MM/yyyy");

        [Display(Name = "Status")]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }
        [JsonProperty] 
        public string ImageFullPath => ImageId == Guid.Empty
       ? "/images/employee.png"
       : $"/images/employees/{ImageId}.jpg";
    }

    public enum EmployeeStatus
    {
        Pending,
        Active,
        Inactive
    }

    public enum Department
    {
        [Display(Name = "Administration")]
        Administration,

        [Display(Name = "Human Resources")]
        HumanResources,

        [Display(Name = "Finance")]
        Finance,

        [Display(Name = "IT")]
        IT,

        [Display(Name = "Maintenance")]
        Maintenance,

        [Display(Name = "Teaching Support")]
        TeachingSupport,

        [Display(Name = "Security")]
        Security,

        [Display(Name = "Library")]
        Library
    }
}
