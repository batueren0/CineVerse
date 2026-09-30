using CineVerse.Application.Common;
using CineVerse.Application.Contracts.Services;
using CineVerse.Application.DTOs.Requests;
using CineVerse.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CineVerse.MVC.Areas.Administrator.Controllers;

[Area("Administrator")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Editor}")]
public class MoviesController(
    IMovieService movieService,
    IGenreService genreService,
    ITagService tagService,
    IMovieTagService movieTagService,
    IWebHostEnvironment environment
    ) : Controller
{

    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxPosterSize = 2 * 1024 * 1024; // 2 mb

    public async Task<IActionResult> Index()
    {
        var movies = await movieService.GetAllAsync();
        return View( movies );
    }

    public async Task<IActionResult> Create()
    {
        await LoadGenresAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMovieRequest request, IFormFile? posterFile)
    {
        if(posterFile is not null)
        {
            var upload = await SavePosterAsync(posterFile);
            if (!upload.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, upload.ErrorMessage);
                await LoadGenresAsync();
                return View(request);
            }

            request = request with { PosterUrl = upload.Data };
        }

        var result = await movieService.CreateAsync(request);
        if (result.IsSuccess)
        {
            TempData["Success"] = $"Movie \"{request.Title}\" has been created.";
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.ErrorMessage);
        await LoadGenresAsync();
        return View(request);
    }

    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id)
    {
        var movie = await movieService.GetByIdAsync(id);
        if(movie is null)
        {
            TempData["Error"] = "Movie not found.";
            return RedirectToAction(nameof(Index));
        }

        await LoadGenresAsync();
        return View(new UpdateMovieRequest(
            movie.Id,
            movie.Genre.Id,
            movie.Title,
            movie.Director,
            movie.ReleaseYear,
            movie.PosterUrl,
            movie.Overview,
            movie.IsInTheaters
            ));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(UpdateMovieRequest request, IFormFile? posterFile)
    {
        if(posterFile is not null)
        {
            var upload = await SavePosterAsync(posterFile);
            if (!upload.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, upload.ErrorMessage);
                await LoadGenresAsync();
                return View(request);
            }

            request = request with { PosterUrl = upload.Data };
        }

        var result = await movieService.UpdateAsync(request);
        if (result.IsSuccess)
        {
            TempData["Success"] = $"Movie \"{request.Title}\" has been updated.";
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.ErrorMessage);
        await LoadGenresAsync();
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await movieService.DeleteAsync(id);
        if (result.IsSuccess)
            TempData["Success"] = "Movie has been deleted.";
        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Tags(Guid id)
    {
        var movie = await movieService.GetByIdAsync(id);
        if(movie is null)
        {
            TempData["Error"] = "Movie not found.";
            return RedirectToAction(nameof(Index));
        }

        // Dropdownda sadece filme eklemeyenleri görmek için
        var allTags = await tagService.GetAllAsync();
        var movieTagIds = movie.Tags.Select(t => t.Id).ToList();

        ViewBag.AvailableTags = allTags
            .Where(t => !movieTagIds.Contains(t.Id))
            .Select(t => new SelectListItem(t.Name, t.Id.ToString()));

        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTag(Guid movieId , Guid tagId)
    {
        var result = await movieTagService.CreateAsync(new CreateMovieTagRequest(movieId, tagId));
        if (result.IsSuccess)
            TempData["Success"] = "Tag has been added.";
        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction(nameof(Tags), new {id = movieId});
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveTag(Guid movieId, Guid tagId)
    {
        var result = await movieTagService.DeleteAsync(movieId, tagId);
        if (result.IsSuccess)
            TempData["Success"] = "Tag has been removed.";
        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction(nameof(Tags), new { id = movieId });
    }





    // Dropdown için liste
    private async Task LoadGenresAsync()
    {
        var genres = await genreService.GetAllAsync();
        ViewBag.Genres = genres.Select(g => new SelectListItem(g.Name, g.Id.ToString()));
    }


    private async Task<Result<string>> SavePosterAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return Result<string>.Failure("Poster must be a JPG, PNG or WEBP image.");

        if (file.Length > MaxPosterSize)
            return Result<string>.Failure("Poster must be smaller than 2 MB.");

        var folder = Path.Combine(environment.WebRootPath, "uploads", "posters");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Result<string>.Success($"/uploads/posters/{fileName}");
    }

    private void AddErrors(string errorMessage)
    {
        foreach (var error in errorMessage.Split(","))
            ModelState.AddModelError(string.Empty, error.Trim());
    }
}
