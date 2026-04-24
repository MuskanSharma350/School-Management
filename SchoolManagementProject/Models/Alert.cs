using System.ComponentModel.DataAnnotations;

namespace SchoolManagementProject.Models
{
    public class Alert : IEntity
    {
        public int Id { get; set; }


        [MaxLength(500)]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsResolved { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }
    
    }
}
