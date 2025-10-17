using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readify_Library.Models;
using Readify_Library.Settings;
using Readify_Library.UnitOfWork;
using Readify_Library.ViewModels;
using System.Threading.Tasks;

namespace Readify_Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public HomeController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region All Books for User
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            if (User.IsInRole(SystemRoles.Admin))
            {
                return RedirectToAction("Index", "Admins");
            }
            else
            {


                var books = await _unitOfWork.Books.GetAllAsync(new[] { "Category" });

                var filteredBooks = books.Where(b => b.Title.ToLower().Contains(searchTerm.ToLower()) ||
                                                     b.Author.ToLower().Contains(searchTerm.ToLower()) ||
                                                     b.Category.Name.ToLower().Contains(searchTerm.ToLower()))
                                         .ToList();

                List<DisplayAllBooksViewModel> booksViewModels = new List<DisplayAllBooksViewModel>();
                foreach (var book in filteredBooks)
                {
                    booksViewModels.Add(new DisplayAllBooksViewModel()
                    {
                        Id = book.Id,
                        Title = book.Title,
                        Author = book.Author,
                        BookCover = book.ImageURL,
                        Category = book.Category.Name,
                        BookCopies = book.AvailableCopies
                    });
                }

                ViewData["SearchTerm"] = searchTerm;

                return View(nameof(Index), booksViewModels);
            }
        }
        #endregion

        #region Details of Book for User
        [Authorize(Roles = SystemRoles.User)]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _unitOfWork.Books.GetOneRecordWithIncludesAsync(b => b.Id == id, new[] { "Category" });
            if (book is null)
            {
                return NotFound();
            }

            return View(nameof(Details), book);
        }
        #endregion

        #region Privacy Policy for User
        public IActionResult PrivacyPolicy()
        {
            return View(nameof(PrivacyPolicy));
        }
        #endregion

        #region About Us for User
        public async Task<IActionResult> AboutUs()
        {
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalCategories = await _context.Categories.CountAsync();

            ViewBag.DeveloperImage = "MyImage.jpg";

            return View(nameof(AboutUs));
        }
        #endregion
    }
}
