using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Readify_Library.Helpers;
using Readify_Library.Models;
using Readify_Library.Settings;
using Readify_Library.UnitOfWork;
using Readify_Library.ViewModels;
using System.Threading.Tasks;

namespace Readify_Library.Controllers
{
    public class BorrowingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public BorrowingsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        #region All Borrowings for Admin
        [Authorize(Roles = SystemRoles.Admin)]
        public async Task<IActionResult> AllBorrowings()
        {
            var borrowings = await _unitOfWork.Borrowings.GetAllBorrowingsWithBooksAndUsersAndUsersTypesAsync();

            foreach (var borrowing in borrowings)
            {
                await CalculatePenalty(borrowing);
            }

            await _unitOfWork.SaveAsync();

            return View(borrowings);
        }

        private async Task CalculatePenalty(Borrowing borrowing)
        {
            // إذا كانت معادة أو لم يتجاوز الموعد النهائي، لا تحسب
            if (borrowing.Status == enBorrowStatus.Returned || DateTime.Now <= borrowing.DueDate)
                return;

            // تحديث الحالة إلى متأخر
            borrowing.Status = enBorrowStatus.OverDue;

            // حساب أيام التأخير
            int overdueDays = (DateTime.Now.Date - borrowing.DueDate.Date).Days;

            if (overdueDays > 0)
            {
                // استخدام الـExtraPenalty من UserType أو قيمة افتراضية
                decimal dailyPenalty = (decimal)borrowing.User?.UserType?.ExtraPenalty!;
                borrowing.PenaltyAmount = overdueDays * dailyPenalty;
            }
        }
        #endregion

        #region Add Borrow for Admin
        [Authorize(Roles = SystemRoles.Admin)]
        [HttpGet]
        public async Task<IActionResult> AddBorrow()
        {
            var viewModel = new AddBorrowingForAdminViewModel()
            {
                AvailableBooks = await _unitOfWork.Books.GetAllAsync(new[] { "Category" }),
                AvailableUsers = await GetAllUSersInRoleUser(),
            };

            return View(nameof(AddBorrow), viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBorrow(AddBorrowingForAdminViewModel addBorrowingForAdmin)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.Users.Include(u => u.UserType)
                                                   .FirstOrDefaultAsync(u => u.Id == addBorrowingForAdmin.UserId);
                if (user is null) return NotFound("User Not Found");

                var book = await _unitOfWork.Books.GetByIdAsync(addBorrowingForAdmin.BookId);
                if (book is null) return NotFound("Book Not Found");

                if (book.AvailableCopies <= 0)
                {
                    ModelState.AddModelError("", $"No Available Copies of the Book '{book.Title}'!");

                    addBorrowingForAdmin.AvailableBooks = await _unitOfWork.Books.GetAllAsync(new[] { "Category" });
                    addBorrowingForAdmin.AvailableUsers = await GetAllUSersInRoleUser();

                    return View(nameof(AddBorrow), addBorrowingForAdmin);
                }

                if (user.NumberOfBooksAvailable <= 0)
                {
                    ModelState.AddModelError("", $"You Reached the maximum number of Borrowing!");

                    addBorrowingForAdmin.AvailableBooks = await _unitOfWork.Books.GetAllAsync(new[] { "Category" });
                    addBorrowingForAdmin.AvailableUsers = await GetAllUSersInRoleUser();

                    return View(nameof(AddBorrow), addBorrowingForAdmin);
                }

                var existingBorrowing = await _unitOfWork.Borrowings.GetAllUserBorrowingsWithBooksAsync(user.Id);
                if (existingBorrowing.Any(b => b.BookId == addBorrowingForAdmin.BookId && b.Status == enBorrowStatus.Pending))
                {
                    ModelState.AddModelError("", $"You have already borrowed the book '{book.Title}'.");

                    addBorrowingForAdmin.AvailableBooks = await _unitOfWork.Books.GetAllAsync(new[] { "Category" });
                    addBorrowingForAdmin.AvailableUsers = await GetAllUSersInRoleUser();

                    return View(nameof(AddBorrow), addBorrowingForAdmin);
                }

                int baseDays = 3;
                int totalDays = baseDays + (user.UserType.ExtraDays);

                DateTime dueDate = DateTime.Now.AddDays(totalDays);

                var borrowing = new Borrowing
                {
                    UserId = user.Id,
                    BookId = book.Id,
                    BorrowDate = DateTime.Now,
                    DueDate = dueDate,
                    Status = enBorrowStatus.Pending,
                    PenaltyAmount = 0
                };

                await _unitOfWork.Borrowings.AddAsync(borrowing);

                book.AvailableCopies -= 1;
                user.NumberOfBooksAvailable -= 1;

                await _unitOfWork.SaveAsync();

                return RedirectToAction(nameof(AllBorrowings));
            }


            addBorrowingForAdmin.AvailableBooks = await _unitOfWork.Books.GetAllAsync(new[] { "Category" });
            addBorrowingForAdmin.AvailableUsers = await GetAllUSersInRoleUser();

            return View(addBorrowingForAdmin);
        }
        #endregion

        #region Return Book for Admin
        [HttpPost]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var borrowing = await _unitOfWork.Borrowings.GetBorrowingByIdWithBookAndUserAndUserTypesAsync(id);
            if (borrowing is null)
                return Json(new { success = false, message = "Borrow Not Found" });

            if (borrowing.Status == enBorrowStatus.Returned)
            {
                return Json(new
                {
                    success = false,
                    message = "The Borrow has already been Returned!."
                });
            }

            borrowing.Status = enBorrowStatus.Returned;
            borrowing.ReturnDate = DateTime.Now;
            borrowing.User.NumberOfBooksAvailable += 1;
            borrowing.Book.AvailableCopies += 1;
            borrowing.PenaltyAmount = 0;

            _unitOfWork.Borrowings.Update(borrowing);
            await _unitOfWork.SaveAsync();

            return Json(new
            {
                success = true,
                message = "The Book is Returned Successfully"
            });
        }
        #endregion

        #region Delete Borrow for Admin
        [HttpPost]
        public async Task<IActionResult> DeleteBorrow(int id)
        {
            var borrowing = await _unitOfWork.Borrowings.GetBorrowingByIdWithBookAndUserAndUserTypesAsync(id);

            if (borrowing is null)
                return Json(new { success = false, message = "Borrowing not found!" });

            if (borrowing.Status != enBorrowStatus.Returned)
            {
                return Json(new
                {
                    success = false,
                    message = "Cannot delete borrowing! The book has not been returned yet."
                });
            }

            if (borrowing.PenaltyAmount > 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Cannot delete borrowing! There are unpaid penalties."
                });
            }

            await _unitOfWork.Borrowings.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return Json(new
            {
                success = true,
                message = "Borrowing record deleted successfully."
            });
        }
        #endregion

        #region Details of Borrow for Admin
        [Authorize(Roles = SystemRoles.Admin)]
        public async Task<IActionResult> BorrowDetails(int id)
        {
            var borrowing = await _unitOfWork.Borrowings.GetBorrowingByIdWithBookAndUserAndUserTypesAsync(id);

            if (borrowing is null) return NotFound("Borrow not found!");

            return View(nameof(BorrowDetails), borrowing);
        }
        #endregion

        //---------------------------------------------------------------
        #region All Borrowings for User
        [Authorize(Roles = SystemRoles.User)]
        public async Task<IActionResult> AllUserBorrowings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var borrowings = await _unitOfWork.Borrowings.GetAllUserBorrowingsWithBooksAsync(user.Id);

            var viewModelList = new List<AllUserBorrowingsViewModel>();

            foreach (var borrowing in borrowings)
            {
                var viewModel = new AllUserBorrowingsViewModel()
                {
                    Id = borrowing.Id,
                    BookTitle = borrowing.Book.Title,
                    BookAuthor = borrowing.Book.Author,
                    BorrowDate = borrowing.BorrowDate,
                    DueDate = borrowing.DueDate,
                    ReturnDate = borrowing.ReturnDate,
                    BookImage = borrowing.Book.ImageURL,
                    PenaltyAmount = borrowing.PenaltyAmount,
                    Status = borrowing.Status.ToString(),
                };

                viewModelList.Add(viewModel);
            }

            return View(nameof(AllUserBorrowings), viewModelList);
        }
        #endregion

        #region Borrow Book for User
        [HttpPost]
        public async Task<IActionResult> BorrowBook(int id)
        {
            try
            {
                //var user = await _userManager.GetUserAsync(User);
                var user = await _userManager.Users.Include(u => u.UserType)
                                                   .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
                if (user is null) return NotFound("User Not Found");

                var book = await _unitOfWork.Books.GetByIdAsync(id);
                if (book is null) return NotFound("Book Not Found");

                if (book.AvailableCopies <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "No Available Copies of this Book!"
                    });
                }

                if (user.NumberOfBooksAvailable <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"You Reached the maximum number of Borrowing!"
                    });
                }

                var existingBorrowing = await _unitOfWork.Borrowings.GetAllUserBorrowingsWithBooksAsync(user.Id);
                if (existingBorrowing.Any(b => b.BookId == id && b.Status == enBorrowStatus.Pending))
                {
                    return Json(new { success = false, message = "You already borrowed this book." });
                }

                int baseDays = 3;
                int totalDays = baseDays + (user.UserType.ExtraDays);

                DateTime dueDate = DateTime.Now.AddDays(totalDays);

                var borrowing = new Borrowing
                {
                    UserId = user.Id,
                    BookId = id,
                    BorrowDate = DateTime.Now,
                    DueDate = dueDate,
                    Status = enBorrowStatus.Pending,
                    PenaltyAmount = 0
                };

                await _unitOfWork.Borrowings.AddAsync(borrowing);

                book.AvailableCopies -= 1;
                user.NumberOfBooksAvailable -= 1;

                await _unitOfWork.SaveAsync();

                return Json(new
                {
                    success = true,
                    message = $"Book borrowed successfully! Please return before {dueDate:MMMM dd, yyyy}",
                    newCopies = book.AvailableCopies  // العدد الجديد للنسخ المتاحة
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion


        public async Task<List<ApplicationUser>> GetAllUSersInRoleUser()
        {
            var users = await _userManager.Users.Include(u => u.UserType).Where(u => u.IsActive == true).ToListAsync();

            var usersWithRoles = new List<ApplicationUser>();
            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, SystemRoles.User))
                {
                    usersWithRoles.Add(user);
                }
            }

            return usersWithRoles;
        }
    }
}
