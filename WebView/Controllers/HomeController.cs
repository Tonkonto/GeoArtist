using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebView.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}