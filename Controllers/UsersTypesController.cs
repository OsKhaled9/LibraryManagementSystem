using Microsoft.AspNetCore.Mvc;
using Readify_Library.Models;
using Readify_Library.UnitOfWork;
using System.Threading.Tasks;

namespace Readify_Library.Controllers
{
    public class UsersTypesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsersTypesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region All Users Types
        public async Task<IActionResult> Index()
        {
            var usersTypes = await _unitOfWork.UsersTypes.GetAllAsync(new[] { "Users" });
            return View(nameof(Index), usersTypes);
        }
        #endregion

        #region Update User Type
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usertype = await _unitOfWork.UsersTypes.GetByIdAsync(id);
            if (usertype is null) return NotFound();

            return View(nameof(Edit), usertype);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserType userType)
        {
            if (ModelState.IsValid)
            {
                var oldUserType = await _unitOfWork.UsersTypes.GetByIdAsync(userType.Id);
                if (oldUserType is null) return NotFound();

                oldUserType.ExtraBooks = userType.ExtraBooks;
                oldUserType.ExtraDays = userType.ExtraDays;
                oldUserType.ExtraPenalty = userType.ExtraPenalty;

                _unitOfWork.UsersTypes.Update(oldUserType);
                await _unitOfWork.SaveAsync();

                return RedirectToAction(nameof(Index), "UsersTypes");
            }

            return View(nameof(Edit), userType);
        }
        #endregion
    }
}
