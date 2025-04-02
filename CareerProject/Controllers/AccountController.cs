using CareerProject.Models.AccountView;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerProject.Controllers
{
    public class AccountController(UserManager<User> userManager) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        public IActionResult Index()
        {
            return View();
        }
        //Get for Register
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }
        //Post method of Register where it receives an object of type RegisterViewModel from the view.
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }
            if (!registerViewModel.Password.Equals(registerViewModel.ConfirmPassword))
            {
                ModelState.AddModelError("Password", "Passwords do not match");
                return View(registerViewModel);
            }
            User user = new()
            {
                UserName = registerViewModel.Email,
                Email = registerViewModel.Email,
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                Gender = registerViewModel.Gender,
                Major = registerViewModel.Major,
                Role = Role.Student
            };
            //Create user
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(registerViewModel);
            }
            else
            { 
                //Create claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, registerViewModel.FirstName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, "Student")
                };
                //Add claims
                var claimResult = await _userManager.AddClaimsAsync(user, claims);
                //Update user in database
                await _userManager.UpdateAsync(user);
                if (!claimResult.Succeeded)
                {
                    foreach (var error in claimResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(registerViewModel);
                }
            }
            return View("Success");
        }
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }
        //Post method for Login that receives an object of type LoginViewModel from the view.
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string RetunUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);  
            }
            //Checks if user exists in database
            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return View(loginViewModel);
            }
            //Checks if entered password matches with the password in passwordhash in database
            var result = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
                return View(loginViewModel);
            }
            else
            {   //Get claims from database
                var claims = user != null ? await _userManager.GetClaimsAsync(user) : null;
                if (claims != null)
                {
                    var scheme = IdentityConstants.ApplicationScheme;
                    var claimsIdentity = new ClaimsIdentity(claims, scheme);
                    var principal = new ClaimsPrincipal(claimsIdentity);
                    var authenticationProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20)
                    };
                    await HttpContext.SignInAsync(scheme, principal, authenticationProperties);
                   
                    return Redirect(RetunUrl);

                }
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Redirect("/Home");

        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
