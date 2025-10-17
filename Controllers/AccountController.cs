using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Readify_Library.Helpers;
using Readify_Library.Models;
using Readify_Library.Settings;
using Readify_Library.UnitOfWork;
using Readify_Library.ViewModels;
using System.Threading.Tasks;

namespace Readify_Library.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _UserImagePath;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _UserImagePath = $"{_webHostEnvironment.WebRootPath}{FileSettings.UsersImagesPath}";
        }

        #region Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                if (user is null)
                {
                    ModelState.AddModelError("", "Invalid Login Attempts!");
                    return View(nameof(Login), loginViewModel);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError("", "Your account is InActive, Please Contact Admin to Active you!");
                    return View(nameof(Login), loginViewModel);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (await _userManager.IsInRoleAsync(user, SystemRoles.Admin))
                        return RedirectToAction(nameof(Index), "Admins");
                    else if (await _userManager.IsInRoleAsync(user, SystemRoles.User))
                        return RedirectToAction(nameof(Index), "Home");
                    else
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError("", "Invalid Role");

                        return View(nameof(Login), loginViewModel);
                    }
                }

                ModelState.AddModelError("", "Invalid Email or Password!");
                return View(nameof(Login), loginViewModel);
            }

            return View(nameof(Login), loginViewModel);
        }
        #endregion

        #region Register Page
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(registerViewModel.Email) is not null)
                {
                    ModelState.AddModelError("Email", "Email is Already Exists!.");

                    return View(nameof(Register), registerViewModel);
                }

                var UserProfileImage = await Utilities.SaveFileAsync(registerViewModel.ProfileImage, _UserImagePath);

                ApplicationUser user = new ApplicationUser()
                {
                    FirstName = registerViewModel.FirstName,
                    LastName = registerViewModel.LastName,
                    Email = registerViewModel.Email,
                    UserName = $"{registerViewModel.FirstName}_{registerViewModel.LastName}",
                    PhoneNumber = registerViewModel.PhoneNumber,
                    Address = registerViewModel.Address,
                    DateOfBirth = registerViewModel.DateOfBirth,
                    IsActive = true,
                    UserTypeId = 1,
                    ProfileImageURL = UserProfileImage.FileName,
                };

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, SystemRoles.User);

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction(nameof(Index), "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                if (!string.IsNullOrEmpty(user.ProfileImageURL))
                    Utilities.DeleteFile(user.ProfileImageURL, _UserImagePath);

                return View(nameof(Register), registerViewModel);
            }

            return View(nameof(Register), registerViewModel);
        }
        #endregion

        #region Log Out
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }
        #endregion
    }
}
