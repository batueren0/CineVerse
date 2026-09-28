using Microsoft.AspNetCore.Mvc;

namespace CineVerse.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
