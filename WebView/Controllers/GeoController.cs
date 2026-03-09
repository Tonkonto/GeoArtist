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
    public IActionResult PostGeoJson([FromBody] GeoJsonSingleRequest request)
    {
        var result = _geoService.ParseGeoJson(request.GeoJson);
        return Ok(ToResponse(result));
    }

    [HttpPost("geojson/batch")]
    public IActionResult PostGeoJsonBatch([FromBody] GeoJsonBatchRequest request)
    {
        var result = _geoService
            .ParseGeoJsonBatch(request.GeoJsonList)
            .Select(ToResponse)
            .ToList();

        return Ok(result);
    }

    [HttpPost("wkt")]
    public IActionResult PostWkt([FromBody] WktSingleRequest request)
    {
        var result = _geoService.ParseWkt(request.Wkt, request.Srid);
        return Ok(ToResponse(result));
    }

    [HttpPost("wkt/batch")]
    public IActionResult PostWktBatch([FromBody] WktBatchRequest request)
    {
        var result = _geoService
            .ParseWktBatch(request.WktList, request.Srid)
            .Select(ToResponse)
            .ToList();

        return Ok(result);
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