using CareerProject.AppDbContext;
using CareerProject.Models.AccountView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CareerProject.Controllers
{
    [Authorize(policy: "MustBeAdmin")]
    public class AdminController(UserManager<User> userManager, AppIdentityDbContext context) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly AppIdentityDbContext _dbContext = context;
        public async Task<IActionResult> Index()
        {
            return View(await GetUsersToManageAsync());
        }
        private async Task<List<UserViewModel>> GetUsersToManageAsync()
        {
            var User = await _userManager.Users
            .Where(x => x.Role != Role.Admin).ToListAsync();
             var listofUserAccounts = new List<UserViewModel>();
            foreach (var user in User)
            {
                listofUserAccounts.Add(new UserViewModel
                {
                    Email = user.Email,
                    FirstName = await GetNameForUser(user.Email ?? string.Empty),
                    Role = user.Role
                });
            }
             return listofUserAccounts;
        }
        private async Task<string> GetNameForUser(string Email)
        {
            var accountuser = await _userManager.FindByEmailAsync(Email);
            if (accountuser != null)
            {
                var claims = await _userManager.GetClaimsAsync(accountuser);
                if (claims != null)
                {
                    return claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
                }
            }
            return String.Empty;
        }
        public async Task<IActionResult> Edit(string Email)
        {
            var accountuser = await _userManager.FindByEmailAsync(Email);
            if (accountuser != null)
            {
                UserViewModel userViewModel = new UserViewModel
                {
                    Email = accountuser.Email,
                    FirstName = await GetNameForUser(accountuser.Email ?? string.Empty),
                    Role = accountuser.Role,
                    Roles = Enum.GetValues<Role>().Select(x => new SelectListItem
                    {
                        Text = x.ToString(),
                        Value = x.ToString(),
                        Selected = x == accountuser.Role
                    })
                };
                return View(userViewModel);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(userViewModel.Email))
                    {
                        User? user = await _userManager.FindByEmailAsync(userViewModel.Email);
                        if (user != null)
                        {
                            user.Role = userViewModel.Role;
                            var claims = await _userManager.GetClaimsAsync(user);
                            var removeResult = await _userManager.RemoveClaimsAsync(user, claims);
                            if (!removeResult.Succeeded)
                            {
                                ModelState.AddModelError(string.Empty, "Unable to update claim-removing existing claim");
                                return View(userViewModel);
                            }
                            var claimsRequired = new List<Claim>
                            {
                            new Claim(ClaimTypes.Name,userViewModel.FirstName ?? " "),
                            new Claim(ClaimTypes.Role,Enum.GetName(userViewModel.Role)?? " "),
                            new Claim(ClaimTypes.NameIdentifier,user.Id),
                            new Claim(ClaimTypes.Email,userViewModel.Email)
                            };
                            var addclaimResult = await _userManager.AddClaimsAsync(user, claimsRequired);
                            if (!addclaimResult.Succeeded)
                            {
                                ModelState.AddModelError(string.Empty, "Unable to update claim - adding new claim failed");
                                return View(userViewModel);
                            }
                            var userUpdateResult = await _userManager.UpdateAsync(user);
                            if (!userUpdateResult.Succeeded)
                            {
                                ModelState.AddModelError("", "Failed to update user");
                                return View(userViewModel);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(userViewModel);
                }
            }
            return RedirectToAction("Index", await GetUsersToManageAsync());
        }
        public async Task<IActionResult> Delete(string email)
        {
            var accountUser = await _userManager.FindByEmailAsync(email);
            if (accountUser != null)
            {
                await _userManager.DeleteAsync(accountUser);
                return View("Index", await GetUsersToManageAsync());
            }
            return NotFound();
        }
    }
}
