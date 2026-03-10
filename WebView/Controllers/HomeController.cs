using Microsoft.AspNetCore.Mvc;

namespace WebView.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult TestPolygon()
    {
        var polygon = new
        {
            geometryType = "Polygon",
            coordinateCount = 5,
            geoJson = """
        {
          "type": "Polygon",
          "coordinates": [
            [
              [74.60,42.87],
              [74.61,42.87],
              [74.61,42.88],
              [74.60,42.88],
              [74.60,42.87]
            ]
          ]
        }
        """,
            isValid = true
        };

        return Json(polygon);
    }
}