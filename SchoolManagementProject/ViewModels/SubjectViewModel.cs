using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagementProject.ViewModels
{
    public class SubjectViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string SubjectName { get; set; }

        public string? Description { get; set; } 

        [Required]
        public int Credits { get; set; }

        [Required]
        public int TotalClasses { get; set; }

    }
}
