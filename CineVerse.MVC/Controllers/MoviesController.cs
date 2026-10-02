using CineVerse.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineVerse.MVC.Controllers;

public class MoviesController(IMovieService movieService,
    IGenreService genreService,
    ITagService tagService
    ) : Controller
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

    public async Task<IActionResult> Genre(Guid id)
    {
        var genre = await genreService.GetByIdAsync(id);
        if (genre is null)
            return NotFound();

        var movies = await movieService.GetAllByGenreIdAsync(id);

        ViewData["Heading"] = genre.Name;
        ViewData["Subheading"] = $"All {genre.Name} movies on CineVerse.";

        return View("Index", movies);
    }

    public async Task<IActionResult> Tag(Guid id)
    {
        var tag = await tagService.GetByIdAsync(id);
        if (tag is null)
            return NotFound();

        var movies = await movieService.GetAllByTagIdAsync(id);

        ViewData["Heading"] = $"#{tag.Name}";
        ViewData["Subheading"] = $"Movies tagged with {tag.Name}.";
        return View("Index", movies);
    }
}
