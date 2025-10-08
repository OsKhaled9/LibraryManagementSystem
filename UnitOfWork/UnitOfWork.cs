using Readify_Library.Models;
using Readify_Library.Repository.Implementation;
using Readify_Library.Repository.Interfaces;

namespace Readify_Library.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IGenericRepository<Category> Categories { get; private set; }
        public IBookRepository Books { get; private set; }

        public UnitOfWork(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            Categories = new GenericRepository<Category>(_context);
            Books = new BookRepository(_context, _webHostEnvironment);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
