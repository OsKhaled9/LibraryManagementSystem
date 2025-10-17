using Readify_Library.Models;

namespace Readify_Library.Repository.Interfaces
{
    public interface IBorrowingRepository : IGenericRepository<Borrowing>
    {
        Task<IEnumerable<Borrowing>> GetAllUserBorrowingsWithBooksAsync(string userId);
        Task<IEnumerable<Borrowing>> GetAllBorrowingsWithBooksAndUsersAndUsersTypesAsync();
        Task<Borrowing?> GetBorrowingByIdWithBookAndUserAndUserTypesAsync(int id);
    }
}
