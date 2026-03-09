using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebView.Models.API;

namespace WebView.Controllers;

[ApiController]
[Route("api/geo")]
public class GeoController(IGeoService geoService) : ControllerBase
{
    private readonly IGeoService _geoService = geoService;

    [HttpPost("geojson")]
    public IActionResult PostGeoJson([FromBody] GeoJsonRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.GeoJson))
        {
            var result = _geoService.ParseGeoJson(request.GeoJson);
            return Ok(result);
        }

        if (request.GeoJsonList is { Count: > 0 })
        {
            var result = _geoService.ParseGeoJsonBatch(request.GeoJsonList).ToList();
            return Ok(result);
        }

        return BadRequest("GeoJson or GeoJsonList is required.");
    }

    [HttpPost("wkt")]
    public IActionResult PostWkt([FromBody] WktRequest request)
    {
        if (request.Srid is null)
            return BadRequest("Srid is required for WKT.");

        if (!string.IsNullOrWhiteSpace(request.Wkt))
        {
            var result = _geoService.ParseWkt(request.Wkt, request.Srid.Value);
            return Ok(result);
        }

        if (request.WktList is { Count: > 0 })
        {
            var result = _geoService.ParseWktBatch(request.WktList, request.Srid.Value).ToList();
            return Ok(result);
        }

        return BadRequest("Wkt or WktList is required.");
    }
}