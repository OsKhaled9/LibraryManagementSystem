using Readify_Library.Models;
using Readify_Library.Repository.Interfaces;

namespace Readify_Library.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }
        IBookRepository Books { get; }
        IGenericRepository<UserType> UsersTypes { get; }

        Task SaveAsync();
    }
}
