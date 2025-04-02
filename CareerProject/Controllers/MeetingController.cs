using CareerProject.AppDbContext;
using CareerProject.Models.AccountView;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerProject.Controllers
{
    public class MeetingController(UserManager<User> userManager, AppIdentityDbContext context) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly AppIdentityDbContext _context = context;
        public IActionResult BookMeeting(string mentorEmail)
        {
            var studentEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var meeting = new User
            {
                MentorEmail = mentorEmail,
                Email = studentEmail,
                MeetingDate = DateTime.Now,
                Status="Pending"
            };
            return View(meeting);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookMeeting(User meeting)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(meeting.Email);
                if (existingUser != null)
                {
                    existingUser.MentorEmail = meeting.MentorEmail;
                    existingUser.MeetingDate = meeting.MeetingDate;
                    existingUser.Status = "Pending";
                    var result = await _userManager.UpdateAsync(existingUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Success");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(meeting);
        }
        public IActionResult Success()
        {
            return View("Success");
        }

    }
}
