using System.ComponentModel.DataAnnotations;

namespace SchoolManagementProject.ViewModels
{
    public class CreateUserViewModel
    {
        [MaxLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Assign Role")]
        public string Role { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
