using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CareerProject.Models.AccountView
{
    public class User : IdentityUser
    {
        public Role Role { get; set; }
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public string? Gender { get; set; }

        public string? Major { get; set; }

        public DateTime MeetingDate { get; set; }
        public string? Status { get; set; }

        public string? MentorEmail { get; set; }

        public string? Experience { get; set; }
        public string? Qualification { get; set; }
        public string? Company { get; set; }
        public string? Skills { get; set; }
        public string? Achievements { get; set; }
    }
    public enum Role
    {   Student,
        Mentor,
        User,
        Admin
    }
}
