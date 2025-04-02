using System.ComponentModel.DataAnnotations;

namespace CareerProject.Models.AccountView
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is mandatory")]
        public String UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is mandatory")]
        public string Password { get; set; } = string.Empty;
    }
}
