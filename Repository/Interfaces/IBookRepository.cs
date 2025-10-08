using Readify_Library.Models;
using Readify_Library.ViewModels;

namespace Readify_Library.Repository.Interfaces
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<Book> CreateBookWithImage(CreateBookViewModel model);
        Task<Book?> UpdateBookWithImage(EditBookViewModel model);
    }
}
