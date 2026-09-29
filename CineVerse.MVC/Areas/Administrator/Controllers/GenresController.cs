using CineVerse.Application.Common;
using CineVerse.Application.Contracts.Services;
using CineVerse.Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineVerse.MVC.Areas.Administrator.Controllers;

[Area("Administrator")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Editor}")]
public class GenresController(IGenreService genreService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var genres = await genreService.GetAllAsync();
        return View(genres);
    }

    public IActionResult Create() => View();


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGenreRequest request)
    {
        var result = await genreService.CreateAsync(request);
        if (result.IsSuccess)
        {
            TempData["Success"] = $"Genre \"{request.Name}\" has been created";
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.ErrorMessage);
        return View(request);
    }


    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id)
    {
        var genre = await genreService.GetByIdAsync(id);
        if(genre is null)
        {
            TempData["Error"] = "Genre not found!";
            return RedirectToAction(nameof(Index));
        }

        return View(new UpdateGenreRequest(genre.Id, genre.Name));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(UpdateGenreRequest request)
    {
        var result = await genreService.UpdateAsync(request);
        if (result.IsSuccess)
        {
            TempData["Success"] = $"Genre \"{request.Name}\" has been updated.";
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.ErrorMessage);
        return View(request);
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await genreService.DeleteAsync(id);
        if (result.IsSuccess)
            TempData["Success"] = "Genre has been deleted.";
        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }


    // servis hataları "hata1,hata2" şeklinde tek tek string dönücek ; ayırıp modelstate ekliyoruz
    private void AddErrors(string errorMessage)
    {
        foreach(var error in errorMessage.Split(","))
            ModelState.AddModelError(string.Empty, error.Trim());
    }

}
