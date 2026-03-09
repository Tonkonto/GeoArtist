using GeoComponent.Facade.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WebView.Models.API;

namespace WebView.Controllers;

[ApiController]
[Route("api/geo")]
public class GeoController(IGeoComponentFacade geoFacade) : ControllerBase
{
    private readonly IGeoComponentFacade _geoFacade = geoFacade;

    [HttpPost("geojson")]
    public IActionResult PostGeoJson([FromBody] GeoJsonSingleRequest request)
    {
        var result = _geoFacade.FromGeoJson(request.GeoJson);
        return Ok(result);
    }

    [HttpPost("geojson/batch")]
    public IActionResult PostGeoJsonBatch([FromBody] GeoJsonBatchRequest request)
    {
        var result = _geoFacade.FromGeoJsonBatch(request.GeoJsonList);
        return Ok(result);
    }

    [HttpPost("wkt")]
    public IActionResult PostWkt([FromBody] WktSingleRequest request)
    {
        var result = _geoFacade.FromWkt(request.Wkt, request.Srid);
        return Ok(result);
    }

    [HttpPost("wkt/batch")]
    public IActionResult PostWktBatch([FromBody] WktBatchRequest request)
    {
        var result = _geoFacade.FromWktBatch(request.WktList, request.Srid);
        return Ok(result);
    }
}