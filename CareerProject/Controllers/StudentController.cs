using Microsoft.AspNetCore.Mvc;
using CareerProject.Models.AccountView;
using System.Linq;
using System.Threading.Tasks;
using CareerProject.AppDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace CareerProject.Controllers
{
    [Authorize(Policy = "MustbeStudent")]
    public class StudentController(UserManager<User> userManager, AppIdentityDbContext context, SignInManager<User> signInManager) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly AppIdentityDbContext _context = context;
        private readonly SignInManager<User> _signInManager = signInManager;
        public  IActionResult Options()
        {
            var email =User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            ViewData["Email"] = email;
            return View();
        }
        public async Task<IActionResult> EditProfile(string email)
        {   
            var loggedInUserEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var loggedInUser = await _userManager.FindByEmailAsync(loggedInUserEmail ?? " ");
            if (loggedInUser == null)
            {
                return NotFound();
            }
           return View(loggedInUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User student)
        {
            var existingStudent = await _userManager.Users.FirstOrDefaultAsync(x=>x.Email==student.Email);
            if (existingStudent == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    existingStudent.UserName = student.Email;
                    existingStudent.FirstName = student.FirstName;
                    existingStudent.LastName = student.LastName;
                    existingStudent.Email = student.Email;
                    existingStudent.PhoneNumber = student.PhoneNumber;
                    existingStudent.Gender = student.Gender;
                    existingStudent.Major = student.Major;
                    var updateResult = await _userManager.UpdateAsync(existingStudent);
                    if (!updateResult.Succeeded)
                    {
                        foreach (var error in updateResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(student);
                    }
                    var claims = await _userManager.GetClaimsAsync(existingStudent);
                    var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                    if (nameClaim != null)
                    {
                        await _userManager.RemoveClaimAsync(existingStudent, nameClaim);
                    }
                    await _userManager.AddClaimAsync(existingStudent, new Claim(ClaimTypes.Name, student.FirstName));
                    // Re-sign the user in
                    await _signInManager.RefreshSignInAsync(existingStudent);
                }
                   catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.Email))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Index", "Home");
        }

            return View(student);
        }
        private bool StudentExists(string Email)
        {
            return _context.Students.Any(e => e.Email == Email);
        }
        public async Task<IActionResult> ViewMentors(string searchString)
        {
            var mentors = from m in _userManager.Users
                          where m.Role == Role.Mentor
                          select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                mentors = mentors.Where(s => s.Skills.Contains(searchString));
            }
            ViewData["CurrentFilter"] = searchString;
            return View(await mentors.ToListAsync());
        }
        public async Task<IActionResult> MentorDetails(string email)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            var mentorDetails = await _context.Users.FirstOrDefaultAsync(y => y.Email == email);
            if (mentorDetails == null)
            {
                return RedirectToAction("Index");
            }
            return View(mentorDetails);
        }
        public async Task<IActionResult> MyMeetings()
        {
            var studentEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (studentEmail == null)
            {
                return Unauthorized();
            }
            var meetings = await _context.Users
                .Where(m => m.Email == studentEmail)
                .ToListAsync();
            return View(meetings);
        }

















    }
}
