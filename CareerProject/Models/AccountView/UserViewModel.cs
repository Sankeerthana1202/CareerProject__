using Microsoft.AspNetCore.Mvc.Rendering;

namespace CareerProject.Models.AccountView
{
    public class UserViewModel
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public Role Role { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; } = [];
    }
}
