using Microsoft.AspNetCore.Mvc;

namespace WebView.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult TestPolygon()
    {
        var payload = new
        {
            type = "FeatureCollection",
            features = new[]
            {
                new
                {
                    type = "Feature",
                    properties = new { source = "HomeController" },
                    geometry = new
                    {
                        type = "Polygon",
                        coordinates = new[]
                        {
                            new[]
                            {
                                new[] { 74.60, 42.87 },
                                new[] { 74.61, 42.87 },
                                new[] { 74.61, 42.88 },
                                new[] { 74.60, 42.88 },
                                new[] { 74.60, 42.87 }
                            }
                        }
                    }
                }
            }
        };

        return Json(payload);
    }
}

