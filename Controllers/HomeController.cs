using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readify_Library.Settings;
using Readify_Library.UnitOfWork;
using Readify_Library.ViewModels;
using System.Threading.Tasks;

namespace Readify_Library.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _unitOfWork.Books.GetAllAsync(new[] { "Category" });

            List<DisplayAllBooksViewModel> booksViewModels = new List<DisplayAllBooksViewModel>();
            foreach (var book in books)
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

            return View(nameof(Index), booksViewModels);
        }

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
    }
}
