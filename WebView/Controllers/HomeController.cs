using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebView.Controllers;

public class HomeController(IGeoService geoService) : Controller
{
    private readonly IGeoService _geoService = geoService;

    public IActionResult Index()
    {
        string sampleGeoJson = """
        {
          "type": "Polygon",
          "coordinates": [
            [[74.60, 42.87],[74.61, 42.87],[74.61, 42.88],[74.60, 42.88],[74.60, 42.87]]
          ]
        }
        """;

        GeoResult geo = _geoService.Parse(sampleGeoJson);

        return View(geo);
    }
}