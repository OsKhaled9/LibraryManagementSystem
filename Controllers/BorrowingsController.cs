using Microsoft.AspNetCore.Mvc;

namespace Readify_Library.Controllers
{
    public class BorrowingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
