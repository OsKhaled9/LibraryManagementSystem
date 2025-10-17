using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Readify_Library.Models;
using Readify_Library.Settings;
using Readify_Library.UnitOfWork;
using Readify_Library.ViewModels;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Readify_Library.Controllers
{
    [Authorize(Roles = SystemRoles.Admin)]
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Dashboard Statistics
        public async Task<IActionResult> Index()
        {
            var Statistics = new StatisticsViewModel()
            {
                BooksCount = await _context.Books.CountAsync(),
                BorrowingsCount = await _context.Borrowings.CountAsync(),
                CategoriesCount = await _context.Categories.CountAsync(),
                UsersCount = await _context.Users.CountAsync(),
                UsersTypesCount = await _context.Users.CountAsync(),
            };

            return View(nameof(Index), Statistics);
        }
        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
