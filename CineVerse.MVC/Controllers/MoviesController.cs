using CineVerse.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineVerse.MVC.Controllers;

public class MoviesController(IMovieService movieService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var movies = await movieService.GetAllAsync();
        return View(movies);
    }

    public async Task<IActionResult> InTheaters()
    {
        var movies = await movieService.GetInTheatersAsync();
        return View("Index", movies);
    }

    [Route("movie/{slug}")]
    public async Task<IActionResult> Details(string slug)
    {
        var movie = await movieService.GetBySlugAsync(slug);
        if (movie is null)
            return NotFound();

        return View(movie);
    }
}
