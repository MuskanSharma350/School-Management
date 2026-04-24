using System.ComponentModel.DataAnnotations;

namespace SchoolManagementProject.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username (email) is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least {1} characters long.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
