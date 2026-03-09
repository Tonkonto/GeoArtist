using Microsoft.AspNetCore.Mvc;

namespace WebView.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}