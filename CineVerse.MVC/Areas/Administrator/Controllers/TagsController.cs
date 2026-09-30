using CineVerse.Application.Common;
using CineVerse.Application.Contracts.Services;
using CineVerse.Application.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineVerse.MVC.Areas.Administrator.Controllers;

[Area("Administrator")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Editor}")]
public class TagsController(ITagService tagService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var tags = await tagService.GetAllAsync();
        return View(tags);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTagRequest request)
    {
        var result = await tagService.CreateAsync(request);
        if (result.IsSuccess)
        {
            TempData["Success"] = $"Tag \"{request.Name}\" has been created.";
            return RedirectToAction(nameof(Index));
        }

        AddErrors(result.ErrorMessage);
        return View(request);
    }

    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(Guid id)
    {
        var tag = await tagService.GetByIdAsync(id);
        if(tag is null)
        {
            TempData["Error"] = "Tag not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new UpdateTagRequest (tag.Id, tag.Name));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Update(UpdateTagRequest request)
    {
        var result = await tagService.UpdateAsync(request);
        if (result.IsSuccess)
        {
            TempData["Success"] = $"Tag \"{request.Name}\" has been updated.";
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
        var result = await tagService.DeleteAsync(id);
        if (result.IsSuccess)
            TempData["Success"] = "Tag has been deleted.";

        else
            TempData["Error"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }

    private void AddErrors(string errorMessage)
    {
        foreach(var error in errorMessage.Split(","))
            ModelState.AddModelError(string.Empty, error.Trim());

    }
}
