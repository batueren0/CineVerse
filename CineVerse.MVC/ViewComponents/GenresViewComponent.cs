using CineVerse.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace CineVerse.MVC.ViewComponents;

public class GenresViewComponent(IGenreService genreService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(string viewName = "Default")
    {
        var genres = await genreService.GetAllAsync();
        return View(viewName, genres);
    }
}
