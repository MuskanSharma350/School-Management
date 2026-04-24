using System.ComponentModel.DataAnnotations;

namespace SchoolManagementProject.ViewModels
{
    public class RecoverPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }
}
