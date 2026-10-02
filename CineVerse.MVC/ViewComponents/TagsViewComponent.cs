using CineVerse.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;


namespace CineVerse.MVC.ViewComponents;

public class TagsViewComponent(ITagService tagService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var tags = await tagService.GetAllAsync();
        return View(tags);
    }
}
