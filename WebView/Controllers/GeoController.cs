using GeoComponent.Core.Interfaces;
using GeoComponent.Core.Models;
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
            return Ok(ToResponse(result));
        }

        if (request.GeoJsonList is { Count: > 0 })
        {
            var result = _geoService
                .ParseGeoJsonBatch(request.GeoJsonList)
                .Select(ToResponse)
                .ToList();

            return Ok(result);
        }

        return BadRequest("GeoJson or GeoJsonList is required.");
    }

    [HttpPost("wkt")]
    public IActionResult PostWkt([FromBody] WktRequest request)
    {
        if (request.Srid is null)
            return BadRequest("Srid is required.");

        if (!string.IsNullOrWhiteSpace(request.Wkt))
        {
            var result = _geoService.ParseWkt(request.Wkt, request.Srid.Value);
            return Ok(ToResponse(result));
        }

        if (request.WktList is { Count: > 0 })
        {
            var result = _geoService
                .ParseWktBatch(request.WktList, request.Srid.Value)
                .Select(ToResponse)
                .ToList();

            return Ok(result);
        }

        return BadRequest("Wkt or WktList is required.");
    }

    private static GeoResponse ToResponse(GeoResult x)
    {
        return new GeoResponse
        {
            GeometryType = x.GeometryType,
            CoordinateCount = x.CoordinateCount,
            GeoJson = x.GeoJson,
            IsValid = x.IsValid
        };
    }
}