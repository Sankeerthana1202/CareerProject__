using CareerProject.AppDbContext;
using CareerProject.Models.AccountView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CareerProject.Controllers
{
    public class MentorController(UserManager<User> userManager, AppIdentityDbContext context, SignInManager<User> signInManager ): Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly AppIdentityDbContext _context = context;
        private readonly SignInManager<User> _signInManager = signInManager;

        [Authorize(policy: "MustBeMentor")]
        public IActionResult Index()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            ViewData["Email"] = email;
            return View();
        }
        public async Task<IActionResult> MeetingRequests()
        {
            var mentorEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var meetingRequests = await _context.Users
                .Where(m => m.MentorEmail == mentorEmail && m.Status == "Pending" || m.Status == "Accepted")
                .ToListAsync();
            return View(meetingRequests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptMeeting(string email)
        {
            var mentorEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var meeting = await _context.Users.FirstOrDefaultAsync(m => m.Email == email && m.MentorEmail == mentorEmail);
            if (meeting == null)
            {
                return NotFound();
            }
            meeting.Status = "Accepted";
            _context.Update(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction("MeetingRequests");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DenyMeeting(string email)
        {
            var mentorEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var meeting = await _context.Users.FirstOrDefaultAsync(m => m.Email == email && m.MentorEmail == mentorEmail);
            if (meeting == null)
            {
                return NotFound();
            }
            meeting.Status = "Denied";
            _context.Update(meeting);
            await _context.SaveChangesAsync();
            return RedirectToAction("MeetingRequests");
        }
        public async Task<IActionResult> EditProfileForMentor
            (string email)
        {
            var loggedInUserEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var loggedInUser = await _userManager.FindByEmailAsync(loggedInUserEmail ?? " ");
            ViewData["loggedInUserEmail"] = email;
            if (loggedInUser == null)
            {
                return NotFound();
            }
            return View(loggedInUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileForMentor(User mentor)
        {

            var existingMentor = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == mentor.Email);
            if (existingMentor == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    existingMentor.UserName = mentor.Email;
                    existingMentor.FirstName = mentor.FirstName;
                    existingMentor.LastName = mentor.LastName;
                    existingMentor.Email = mentor.Email;
                    existingMentor.PhoneNumber = mentor.PhoneNumber;
                    existingMentor.Gender = mentor.Gender;
                    existingMentor.Qualification = mentor.Major;
                    existingMentor.Achievements = mentor.Achievements;
                    existingMentor.Experience = mentor.Experience;
                    existingMentor.Company = mentor.Company;
                    existingMentor.Skills = mentor.Skills;

                    var updateResult = await _userManager.UpdateAsync(existingMentor);
                    if (!updateResult.Succeeded)
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(mentor);
                    }
                    var claims = await _userManager.GetClaimsAsync(existingMentor);
                    var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                    if (nameClaim != null)
                    {
                        await _userManager.RemoveClaimAsync(existingMentor, nameClaim);
                    }
                    await _userManager.AddClaimAsync(existingMentor, new Claim(ClaimTypes.Name, mentor.FirstName));
                    // Re-sign the user in
                    await _signInManager.RefreshSignInAsync(existingMentor);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MentorExists(mentor.Email))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                 return RedirectToAction("Index");
            }

            return View(mentor);
        }
        private bool MentorExists(string Email)
        {
            return _context.Students.Any(e => e.Email == Email);
        }


    }
}
