using System.ComponentModel.DataAnnotations;

namespace CareerProject.Models.AccountView
{
    public class Mentor
    {
        [Required]
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Major { get; set; }
    }

}
