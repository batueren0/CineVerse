using CineVerse.Application.Contracts.Services;
using CineVerse.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace CineVerse.MVC.Controllers
{
    public class HomeController(IMovieService movieService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var carousel = await movieService.GetCarouselAsync();
            var inTheaters = await movieService.GetInTheatersAsync();
            var latest = await movieService.GetAllAsync();
            var recent = await movieService.GetRecentAsync();

            var model = new HomeViewModel(
                carousel,
                inTheaters.Take(6),
                latest.Take(9),
                recent
                );

            return View(model);
        }
    }
}
