using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Readify_Library.Models;
using Readify_Library.UnitOfWork;
using System.Threading.Tasks;

namespace Readify_Library.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region All Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(new[] {"Books"} );
            return View(nameof(Index), categories);
        }
        #endregion

        #region Add Category
        [HttpGet]
        public IActionResult Add()
        {
            return View(nameof(Add));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Add), category);
        }
        #endregion

        #region Update Category
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return View(nameof(Edit), category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Edit), category);
        }
        #endregion

        #region Details of Category
        public async Task<IActionResult> Details(int id)
        {
            var category = await _unitOfWork.Categories.GetOneRecordWithIncludesAsync(c => c.Id == id, new[] {"Books"});
            if (category is null) return NotFound();

            return View(nameof(Details), category);
        }
        #endregion

        #region Delete Category
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Categories.GetOneRecordWithIncludesAsync(c => c.Id == id, new[] { "Books" });
            if (category is null)
                return Json(new { success = false, message = "Category Not Found" });

            // Condition (Check if the Category has Books then Can not delete)
            if (category.Books is not null && category.Books.Any())
                return Json(new { success = false, message = "Cannot delete category because it contains books" });


            await _unitOfWork.Categories.DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Category deleted successfully." });
        }
        #endregion

        // Remote Validation Method
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsUniqueName(string name, int id)
        {
            var existingCategory = await _unitOfWork.Categories
                .GetByCriteriaAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != id);

            return existingCategory == null ? Json(true) : Json($"Category name '{name}' already exists.");
        }
    }
}
