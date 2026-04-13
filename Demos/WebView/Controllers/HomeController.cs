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
            GeoJson = BuildRectangleFeatureCollection(74.60, 42.87, 0.01, 0.01, "Rectangle A")
        });
    }

    /// <summary>Returns executable JavaScript that calls <c>window.GeoArtist.updateGeoJson(...)</c> with demo geometry.</summary>
    [HttpGet]
    public IActionResult TestPolygon()
    {
        var geoJson = BuildRectangleFeatureCollection(74.60, 42.87, 0.01, 0.01, "Rectangle TestPolygon");
        return ScriptUpdateGeoJson(geoJson);
    }

    [HttpGet]
    public IActionResult DemoGeoJsonSingle()
    {
        var geoJson = BuildRectangleFeatureCollection(74.605, 42.872, 0.014, 0.009, "Rectangle GeoJson Single");
        return ScriptUpdateGeoJson(geoJson);
    }

    [HttpGet]
    public IActionResult DemoGeoJsonBatch()
    {
        return ScriptUpdateGeoJson(BuildRectangleFeatureCollectionBatch());
    }

    [HttpGet]
    public IActionResult DemoWktSingle()
    {
        var geoJson = BuildRectangleFeatureCollection(74.622, 42.866, 0.010, 0.013, "Rectangle WKT Single");
        return ScriptUpdateGeoJson(geoJson);
    }

    [HttpGet]
    public IActionResult DemoWktBatch()
    {
        return ScriptUpdateGeoJson(BuildRectangleFeatureCollectionPair());
    }

    private static IActionResult ScriptUpdateGeoJson(string geoJson)
    {
        // System.Text.Json output is valid JS when spliced here (JSON ⊆ expression grammar for these payloads).
        var body = $"window.GeoArtist.updateGeoJson({geoJson});";
        return new ContentResult
        {
            Content = body,
            ContentType = "application/javascript",
            StatusCode = 200
        };
    }

    private static string BuildRectangleFeatureCollection(double minLon, double minLat, double width, double height, string name)
    {
        var maxLon = minLon + width;
        var maxLat = minLat + height;

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
                        coordinates = new[]
                        {
                            new[]
                            {
                                new[] { minLon, minLat },
                                new[] { maxLon, minLat },
                                new[] { maxLon, maxLat },
                                new[] { minLon, maxLat },
                                new[] { minLon, minLat }
                            }
                        }
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
                BuildRectangleFeature(74.59, 42.865, 0.01, 0.01, "Rectangle WKT Batch A"),
                BuildRectangleFeature(74.625, 42.878, 0.008, 0.009, "Rectangle WKT Batch B")
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
                BuildRectangleFeature(74.602, 42.871, 0.009, 0.008, "Rectangle GeoJson Batch A"),
                BuildRectangleFeature(74.616, 42.884, 0.012, 0.007, "Rectangle GeoJson Batch B")
            }
        };

        return JsonSerializer.Serialize(payload, SerializerOptions);
    }

    private static object BuildRectangleFeature(double minLon, double minLat, double width, double height, string name)
    {
        var maxLon = minLon + width;
        var maxLat = minLat + height;

        return new
        {
            type = "Feature",
            properties = new { source = "HomeController", shape = "Rectangle", name },
            geometry = new
            {
                type = "Polygon",
                coordinates = new[]
                {
                    new[]
                    {
                        new[] { minLon, minLat },
                        new[] { maxLon, minLat },
                        new[] { maxLon, maxLat },
                        new[] { minLon, maxLat },
                        new[] { minLon, minLat }
                    }
                }
            }
        };
    }
}
