using CineVerse.Application.Contracts.Services;
using CineVerse.Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CineVerse.MVC.Controllers;

[Authorize]
public class ReviewsController(IReviewService reviewService) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReviewRequest request ,string slug) 
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        request.UserId = Guid.Parse(userId!);

        var result = await reviewService.CreateAsync(request);
        if (result.IsSuccess)
            TempData["Success"] = "Thanks! your review has been posted";
        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction("Details", "Movies", new {slug});
    }
}
