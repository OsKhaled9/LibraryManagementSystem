using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class BooksController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _BookImagePath;

        public BooksController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _BookImagePath = $"{_webHostEnvironment.WebRootPath}{FileSettings.BooksImagesPath}";
        }

        #region All Books
        public async Task<IActionResult> Index()
        {
            var books = await _unitOfWork.Books.GetAllAsync(new[] { "Category" });
            return View(nameof(Index), books);
        }
        #endregion

        #region Add Book
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var bookViewModel = new CreateBookViewModel()
            {
                Categories = await _unitOfWork.Categories.GetAllAsync()
            };
            return View(nameof(Add), bookViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateBookViewModel createBookViewModel)
        {
            if (ModelState.IsValid)
            {
                var book = await _unitOfWork.Books.CreateBookWithImage(createBookViewModel);
                if (book is null) return NotFound();

                await _unitOfWork.Books.AddAsync(book);
                await _unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index), "Books");
            }

            createBookViewModel.Categories = await _unitOfWork.Categories.GetAllAsync();
            return View(nameof(Add), createBookViewModel);
        }
        #endregion

        #region Update Book
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book is null) return NotFound();

            var editBookViewModel = new EditBookViewModel()
            {
                Id = id,
                Author = book.Author,
                Title = book.Title,
                AvailableCopies = book.AvailableCopies,
                ISBN = book.ISBN,
                PublishYear = book.PublishYear,
                Description = book.Description,
                CategoryId = book.CategoryId,
                CurrentCover = book.ImageURL,
                Categories = await _unitOfWork.Categories.GetAllAsync()
            };

            return View(nameof(Edit), editBookViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBookViewModel editBookViewModel)
        {
            if (ModelState.IsValid)
            {
                var book = await _unitOfWork.Books.UpdateBookWithImage(editBookViewModel);
                if (book is null) return NotFound();

                _unitOfWork.Books.Update(book);
                await _unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index), "Books");
            }

            editBookViewModel.Categories = await _unitOfWork.Categories.GetAllAsync();
            return View(nameof(Edit), editBookViewModel);
        }
        #endregion

        #region Details of Book
        public async Task<IActionResult> Details(int id)
        {
            var book = await _unitOfWork.Books.GetOneRecordWithIncludesAsync(b => b.Id == id, new[] { "Category" });

            if (book is null) return NotFound();

            return View(nameof(Details), book);
        }
        #endregion

        #region Delete Book
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _unitOfWork.Books.GetOneRecordWithIncludesAsync(b => b.Id == id, new[] { "Borrowings" } );
            if (book is null)
                return Json(new { success = false, message = "Book Not Found" });

            // Condtion (When there are a Borowings for this book) in Future
            bool hasActiveBorrowings = book.Borrowings.Any(b => b.Status != enBorrowStatus.Returned);

            if (hasActiveBorrowings)
            {
                return Json(new
                {
                    success = false,
                    message = "Cannot delete book! There are active borrowings for this book."
                });
            }

            if (!string.IsNullOrEmpty(book.ImageURL))
                Utilities.DeleteFile(book.ImageURL, _BookImagePath);

            await _unitOfWork.Books.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Book deleted successfully." });
        }
        #endregion

        #region Increase and Decrease Copies of Books
        [HttpPost]
        public async Task<IActionResult> IncreaseCopies(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book is null) return NotFound();

            if (book.AvailableCopies >= 1000)
            {
                return Json(new
                {
                    success = false,
                    message = "Can not exceed maximum of 1000 copies!",
                    newCopies = book.AvailableCopies
                });
            }

            book.AvailableCopies += 1;
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, newCopies = book.AvailableCopies });
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseCopies(int id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book is null) return NotFound();

            if (book.AvailableCopies <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Can not have less than 0 copy",
                    newCopies = book.AvailableCopies
                });
            }

            book.AvailableCopies -= 1;
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, newCopies = book.AvailableCopies });
        }
        #endregion
    }
}
