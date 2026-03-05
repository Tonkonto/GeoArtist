using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeoArtist.WebDemo.Controllers;

public class HomeController : Controller
{
    private readonly IGeoService _geoService;

    public HomeController(IGeoService geoService)
    {
        _geoService = geoService;
    }

    public IActionResult Index()
    {
        string sampleGeoJson = @"{
          ""type"": ""Polygon"",
          ""coordinates"": [[[74.6,42.87],[74.61,42.87],[74.61,42.88],[74.6,42.88],[74.6,42.87]]]
        }";

        GeoResult geo = _geoService.Parse(sampleGeoJson);

        return View(geo);
    }
}
