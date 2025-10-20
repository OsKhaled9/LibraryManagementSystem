using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Readify_Library.Helpers;
using Readify_Library.Models;
using Readify_Library.Settings;
using Readify_Library.UnitOfWork;
using Readify_Library.ViewModels;
using System.Threading.Tasks;

namespace Readify_Library.Controllers
{
    [Authorize(Roles = SystemRoles.Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _UserImagePath;


        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
            _UserImagePath = $"{_webHostEnvironment.WebRootPath}{FileSettings.UsersImagesPath}";
            _unitOfWork = unitOfWork;
        }

        #region All Users
        public async Task<IActionResult> Index()
        {
            var usersList = await _userManager.Users.Include(u => u.UserType).Include(u => u.Borrowings).ToListAsync();

            var userViewModel = new List<UserFormViewModel>();

            foreach (var user in usersList)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var CurrentuserBorrowings = user.Borrowings.Count(b => b.Status != enBorrowStatus.Returned);
                var availableBooksForUser = user.UserType.ExtraBooks - CurrentuserBorrowings;

                if (roles.Contains(SystemRoles.User))
                {
                    userViewModel.Add(new UserFormViewModel()
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Address = user.Address,
                        PhoneNumber = user.PhoneNumber,
                        Email = user.Email,
                        NumberOfBooksAvailable = availableBooksForUser,
                        IsActive = user.IsActive,
                        RoleName = roles.FirstOrDefault(),
                        UserType = user.UserType.TypeName.ToString(),
                    });
                }
            }

            return View(nameof(Index), userViewModel);
        }
        #endregion

        #region Add User
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var createViewModel = new CreateUserViewModel()
            {
                UsersTypes = await _unitOfWork.UsersTypes.GetAllAsync(),
                Roles = await GetRolesAsync()
            };

            return View(nameof(Add), createViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateUserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(userViewModel.Email) is not null)
                {
                    ModelState.AddModelError("Email", "Email is Already Exists!.");

                    userViewModel.UsersTypes = await _unitOfWork.UsersTypes.GetAllAsync();
                    userViewModel.Roles = await GetRolesAsync();

                    return View(nameof(Add), userViewModel);
                }

                var UserProfileImage = await Utilities.SaveFileAsync(userViewModel.ProfileImage, _UserImagePath);

                ApplicationUser user = new ApplicationUser()
                {
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                    Email = userViewModel.Email,
                    UserName = $"{userViewModel.FirstName}_{userViewModel.LastName}",
                    PhoneNumber = userViewModel.PhoneNumber,
                    Address = userViewModel.Address,
                    DateOfBirth = userViewModel.DateOfBirth,
                    IsActive = userViewModel.IsActive,
                    UserTypeId = userViewModel.UserTypeId,
                    ProfileImageURL = UserProfileImage.FileName,
                    NumberOfBooksAvailable = await GetExtraBooksByUserTypeId(userViewModel.UserTypeId),
                };

                var result = await _userManager.CreateAsync(user, userViewModel.Password);
                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByIdAsync(userViewModel.RoleId);
                    await _userManager.AddToRoleAsync(user, role.Name);

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                if (!string.IsNullOrEmpty(user.ProfileImageURL))
                    Utilities.DeleteFile(user.ProfileImageURL, _UserImagePath);

                userViewModel.UsersTypes = await _unitOfWork.UsersTypes.GetAllAsync();
                userViewModel.Roles = await GetRolesAsync();

                return View(nameof(Add), userViewModel);
            }

            userViewModel.UsersTypes = await _unitOfWork.UsersTypes.GetAllAsync();
            userViewModel.Roles = await GetRolesAsync();

            return View(nameof(Add), userViewModel);
        }
        #endregion

        #region Update User
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var roles = await GetRolesAsync();
            var userRoleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            var userRoleId = roles.FirstOrDefault(r => r.Text == userRoleName)?.Value;

            var userViewModel = new EditUserViewModel()
            {
                UserId = id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                IsActive = user.IsActive,
                CurrenProfileImage = user.ProfileImageURL,
                RoleId = userRoleId,
                Roles = roles,
                UserTypeId = user.UserTypeId,
                UsersTypes = await _unitOfWork.UsersTypes.GetAllAsync()
            };

            return View(nameof(Edit), userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel editUserViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(editUserViewModel.UserId);
                if (user is null)
                {
                    return NotFound("User Not Found");
                }

                if (editUserViewModel.Email != user.Email && await _userManager.FindByEmailAsync(editUserViewModel.Email) is not null)
                {
                    ModelState.AddModelError("Email", "Email is Already Exists!.");

                    editUserViewModel.UsersTypes = await _unitOfWork.UsersTypes.GetAllAsync();
                    editUserViewModel.Roles = await GetRolesAsync();

                    return View(nameof(Edit), editUserViewModel);
                }

                var hasNewCover = editUserViewModel.ProfileImage is not null;
                var oldCover = user.ProfileImageURL;

                user.FirstName = editUserViewModel.FirstName;
                user.LastName = editUserViewModel.LastName;
                user.Email = editUserViewModel.Email;
                user.PhoneNumber = editUserViewModel.PhoneNumber;
                user.Address = editUserViewModel.Address;
                user.DateOfBirth = editUserViewModel.DateOfBirth;
                user.UserTypeId = editUserViewModel.UserTypeId;
                user.IsActive = editUserViewModel.IsActive;
                user.NumberOfBooksAvailable = await GetExtraBooksByUserTypeId(editUserViewModel.UserTypeId);

                if (hasNewCover)
                {
                    var cover = await Utilities.SaveFileAsync(editUserViewModel.ProfileImage!, _UserImagePath);
                    user.ProfileImageURL = cover.FileName;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (hasNewCover)
                    {
                        Utilities.DeleteFile(oldCover!, _UserImagePath);
                    }

                    var existingRoles = await _userManager.GetRolesAsync(user);
                    if (existingRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, existingRoles);
                    }

                    var newRole = await _roleManager.FindByIdAsync(editUserViewModel.RoleId);
                    if (newRole is not null)
                    {
                        await _userManager.AddToRoleAsync(user, newRole.Name);
                    }

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                Utilities.DeleteFile(user.ProfileImageURL!, _UserImagePath);

                editUserViewModel.Roles = await GetRolesAsync();
                editUserViewModel.UsersTypes = await _unitOfWork.UsersTypes.GetAllAsync();

                return View(nameof(Edit), editUserViewModel);

            }

            editUserViewModel.UsersTypes = await _unitOfWork.UsersTypes.GetAllAsync();
            editUserViewModel.Roles = await GetRolesAsync();
            return RedirectToAction(nameof(Edit), editUserViewModel);
        }
        #endregion

        #region Details of User
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.Users
                                         .Include(u => u.UserType)
                                         .Include(u => u.Borrowings)
                                         .ThenInclude(u => u.Book)
                                         .FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var userDetailsViewModel = new UserDetailsViewModel()
            {
                UserId = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                Role = roles.FirstOrDefault(),
                Phone = user.PhoneNumber,
                BirthDate = user.DateOfBirth.ToString("yyyy-MM-dd"),
                Status = user.IsActive ? "Active" : "Inactive",
                TypeOfUser = user.UserType.TypeName.ToString(),
                ProfileImageUrl = user.ProfileImageURL,
                TotalBorrowedBooks = user.Borrowings.Count(),
                BorrowedBooks = user.Borrowings.ToList()
            };

            return View(nameof(Details), userDetailsViewModel);
        }
        #endregion

        #region Delete User
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Borrowings)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null)
                return Json(new { success = false, message = "User not found." });


            bool hasAnyBorrowings = user.Borrowings?.Any() == true;

            if (hasAnyBorrowings)
            {
                return Json(new
                {
                    success = false,
                    message = "Cannot delete user. This user has borrowing history.!"
                });
            }

            if (!string.IsNullOrEmpty(user.ProfileImageURL))
            {
                Utilities.DeleteFile(user.ProfileImageURL, _UserImagePath);
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Json(new { success = true, message = "User deleted successfully." });
            }

            string error = "";
            foreach (var err in result.Errors)
            {
                error = error + $" {err.Description}";
            }

            return Json(new
            {
                success = false,
                message = error
            });
        }
        #endregion

        private async Task<List<SelectListItem>> GetRolesAsync()
        {
            return await _roleManager.Roles.Where(u => u.Name == SystemRoles.User)
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id
                }).ToListAsync();
        }


        private async Task<int> GetExtraBooksByUserTypeId(int userTypeId)
        {
            var userType = await _unitOfWork.UsersTypes.GetByIdAsync(userTypeId);
            return userType?.ExtraBooks ?? 0;
        }
    }
}