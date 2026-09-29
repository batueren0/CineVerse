using CineVerse.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineVerse.MVC.Areas.Administrator.Controllers;


[Area("Administrator")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Editor}")]
public class HomeController : Controller
{
    public IActionResult Index() => View();
}
