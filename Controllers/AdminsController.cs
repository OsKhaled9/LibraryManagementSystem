using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readify_Library.Models;
using Readify_Library.Settings;
using System.Diagnostics;

namespace Readify_Library.Controllers
{
    [Authorize(Roles = SystemRoles.Admin)]
    public class AdminsController : Controller
    {
        private readonly ILogger<AdminsController> _logger;

        public AdminsController(ILogger<AdminsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

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
