using System.ComponentModel.DataAnnotations;

namespace CareerProject.Models.AccountView
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is mandatory")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is mandatory")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Id is mandatory")]
        [EmailAddress(ErrorMessage = "This is not a valid email address, use this format test@gmail.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please provide a password to secure your account")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Provide password same as previous")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Gender is mandatory")]
        public string Gender { get; set; }

        public string? Major { get; set; }
    }
}
