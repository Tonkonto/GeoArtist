using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebView.Models.Home;

namespace WebView.Controllers;

public sealed class HomeController : Controller
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public IActionResult Index()
    {
        return View(new HomeMapDemoViewModel
        {
            GeoJson = BuildRectangleFeatureCollection(74.60, 42.87, 74.61, 42.88, "Rectangle A")
        });
    }

    [HttpGet]
    public IActionResult DemoGeoJsonSingle()
    {
        var geoJson = BuildRectangleFeatureCollection(74.605, 42.872, 74.619, 42.881, "Rectangle GeoJson Single");
        return Content(geoJson, "application/json");
    }

    [HttpGet]
    public IActionResult DemoGeoJsonBatch()
    {
        return Content(BuildRectangleFeatureCollectionBatch(), "application/json");
    }

    [HttpGet]
    public IActionResult DemoWktSingle()
    {
        var geoJson = BuildRectangleFeatureCollection(74.622, 42.866, 74.632, 42.879, "Rectangle WKT Single");
        return Content(geoJson, "application/json");
    }

    [HttpGet]
    public IActionResult DemoWktBatch()
    {
        return Content(BuildRectangleFeatureCollectionPair(), "application/json");
    }

    private static string BuildRectangleFeatureCollection(double lng1, double lat1, double lng2, double lat2, string name)
    {
        double[][] rectangle =
        [
            [lng1, lat1],
            [lng2, lat1],
            [lng2, lat2],
            [lng1, lat2],
            [lng1, lat1]
        ];

        var payload = new
        {
            type = "FeatureCollection",
            features = new[]
            {
                new
                {
                    type = "Feature",
                    properties = new { source = "HomeController", shape = "Rectangle", name },
                    geometry = new
                    {
                        type = "Polygon",
                        coordinates = new[] { rectangle }
                    }
                }
            }
        };

        return JsonSerializer.Serialize(payload, SerializerOptions);
    }

    private static string BuildRectangleFeatureCollectionPair()
    {
        var payload = new
        {
            type = "FeatureCollection",
            features = new object[]
            {
                BuildRectangleFeature(74.59, 42.865, 74.601, 42.875, "Rectangle WKT Batch A"),
                BuildRectangleFeature(74.625, 42.878, 74.633, 42.887, "Rectangle WKT Batch B")
            }
        };

        return JsonSerializer.Serialize(payload, SerializerOptions);
    }

    private static string BuildRectangleFeatureCollectionBatch()
    {
        var payload = new
        {
            type = "FeatureCollection",
            features = new object[]
            {
                BuildRectangleFeature(74.602, 42.871, 74.611, 42.879, "Rectangle GeoJson Batch A"),
                BuildRectangleFeature(74.616, 42.884, 74.628, 42.891, "Rectangle GeoJson Batch B")
            }
        };

        return JsonSerializer.Serialize(payload, SerializerOptions);
    }

    private static object BuildRectangleFeature(double lng1, double lat1, double lng2, double lat2, string name)
    {
        double[][] rectangle =
        [
            [lng1, lat1],
            [lng2, lat1],
            [lng2, lat2],
            [lng1, lat2],
            [lng1, lat1]
        ];

        return new
        {
            type = "Feature",
            properties = new { source = "HomeController", shape = "Rectangle", name },
            geometry = new
            {
                type = "Polygon",
                coordinates = new[] { rectangle }
            }
        };
    }
}
