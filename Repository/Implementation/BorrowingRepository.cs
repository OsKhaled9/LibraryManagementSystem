using Microsoft.EntityFrameworkCore;
using Readify_Library.Models;
using Readify_Library.Repository.Interfaces;
using System.Collections;

namespace Readify_Library.Repository.Implementation
{
    public class BorrowingRepository : GenericRepository<Borrowing>, IBorrowingRepository
    {
        private readonly ApplicationDbContext _context;

        public BorrowingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Borrowing>> GetAllUserBorrowingsWithBooksAsync(string userId)
        {
            return await _context.Borrowings.Include(b => b.Book)
                                            .Where(b => b.UserId == userId)
                                            .OrderByDescending(b => b.BorrowDate)
                                            .ToListAsync();
        }

        public async Task<IEnumerable<Borrowing>> GetAllBorrowingsWithBooksAndUsersAndUsersTypesAsync()
        {
            return await _context.Borrowings.Include(b => b.Book)
                                            .Include(b => b.User)
                                            .ThenInclude(u => u.UserType)
                                            .OrderByDescending (b => b.BorrowDate)
                                            .ToListAsync();
        }

        public async Task<Borrowing?> GetBorrowingByIdWithBookAndUserAndUserTypesAsync(int id)
        {
            return await _context.Borrowings.Include(b => b.Book)
                                            .Include(b => b.User)
                                            .ThenInclude(u => u.UserType)
                                            .FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}
