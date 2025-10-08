using Microsoft.AspNetCore.Http.HttpResults;
using Readify_Library.Helpers;
using Readify_Library.Models;
using Readify_Library.Repository.Interfaces;
using Readify_Library.Settings;
using Readify_Library.ViewModels;
using static System.Collections.Specialized.BitVector32;

namespace Readify_Library.Repository.Implementation
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _BookImagePath;

        public BookRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment) : base(context)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _BookImagePath = $"{_webHostEnvironment.WebRootPath}{FileSettings.BooksImagesPath}";
        }

        public async Task<Book> CreateBookWithImage(CreateBookViewModel model)
        {
            var bookCover = await Utilities.SaveFileAsync(model.BookCover, _BookImagePath);

            var Book = new Book()
            {
                Title = model.Title,
                Author = model.Author,
                ISBN = model.ISBN,
                PublishYear = model.PublishYear,
                AvailableCopies = model.AvailableCopies,
                Description = model.Description,
                ImageURL = bookCover.FileName,
                CategoryId = model.CategoryId,
            };

            return Book;
        }

        public async Task<Book?> UpdateBookWithImage(EditBookViewModel model)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == model.Id);

            if (book is null)
                return null;

            var hasNewCover = model.BookCover is not null;
            var oldCover = book.ImageURL;

            book.Title = model.Title;
            book.Author = model.Author;
            book.ISBN = model.ISBN;
            book.PublishYear = model.PublishYear;
            book.AvailableCopies = model.AvailableCopies;
            book.Description = model.Description;
            book.CategoryId = model.CategoryId;

            if (hasNewCover)
            {
                var cover = await Utilities.SaveFileAsync(model.BookCover!, _BookImagePath);
                book.ImageURL = cover.FileName;
            }

            var effectedRows = _context.SaveChanges();
            if (effectedRows > 0) // means applied changes in database
            {
                if (hasNewCover)
                {
                    Utilities.DeleteFile(oldCover!, _BookImagePath);
                }
            }
            else
            {
                Utilities.DeleteFile(book.ImageURL!, _BookImagePath);
            }

            return book;
        }
    }
}
