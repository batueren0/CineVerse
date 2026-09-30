using CineVerse.Application.Common;
using CineVerse.Application.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineVerse.MVC.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Editor}")]
    public class ReviewsController(IReviewService reviewService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var reviews = await reviewService.GetAllAsync();
            return View(reviews);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await reviewService.DeleteAsync(id);

            if (result.IsSuccess)
                TempData["Success"] = "Review has been deleted.";
            else
                TempData["Error"] = result.ErrorMessage;

            return RedirectToAction(nameof(Index));
        }
    }
}
