using System.ComponentModel.DataAnnotations;

namespace CareerProject.Models.AccountView
{
    public class Student
    {
        
        [Key]
        public int Id { get; set; } // Unique identifier for the student
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Major { get; set; } // Student's major or field of study

        



    }
}
