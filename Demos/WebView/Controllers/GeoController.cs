using GeoArtist.Core.Exceptions;
using GeoArtist.Core.Interfaces;
using GeoArtist.Core.Services;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Text.Json.Nodes;
using WebView.Models.API;

namespace WebView.Controllers;

[ApiController]
[Route("api/geo")]
public sealed class GeoController(
    GeoJsonValidationService geoJsonValidationService,
    IGeometryTransformService geometryTransformService) : ControllerBase
{
    private static readonly WKTReader WktReader = new();
    private static readonly GeoJsonWriter GeoJsonWriter = new();

    private readonly GeoJsonValidationService _geoJsonValidationService = geoJsonValidationService;
    private readonly IGeometryTransformService _geometryTransformService = geometryTransformService;

    [HttpPost("geojson")]
    public IActionResult PostGeoJson([FromBody] GeoJsonSingleRequest request)
    {
        var normalized = _geoJsonValidationService.NormalizeForRender(request.GeoJson, sourceSrid: null);
        return Ok(ToGeoDataResponse(normalized));
    }

    [HttpPost("geojson/batch")]
    public IActionResult PostGeoJsonBatch([FromBody] GeoJsonBatchRequest request)
    {
        var result = request.GeoJsonList
            .Select(item => _geoJsonValidationService.NormalizeForRender(item, sourceSrid: null))
            .Select(ToGeoDataResponse)
            .ToList();

        return Ok(result);
    }

    [HttpPost("wkt")]
    public IActionResult PostWkt([FromBody] WktSingleRequest request)
    {
        var normalized = ConvertWktToFeatureCollection(request.Wkt, request.Srid);
        return Ok(ToGeoDataResponse(normalized));
    }

    [HttpPost("wkt/batch")]
    public IActionResult PostWktBatch([FromBody] WktBatchRequest request)
    {
        var result = request.WktList
            .Select(wkt => ConvertWktToFeatureCollection(wkt, request.Srid))
            .Select(ToGeoDataResponse)
            .ToList();

        return Ok(result);
    }

    private string ConvertWktToFeatureCollection(string wkt, int sourceSrid)
    {
        if (string.IsNullOrWhiteSpace(wkt))
            throw new ArgumentException("WKT payload must not be empty.", nameof(wkt));

        Geometry geometry;

        try
        {
            geometry = WktReader.Read(wkt) ?? throw new InvalidGeoJsonException("Parsed geometry is null.");
        }
        catch (InvalidGeoJsonException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidGeoJsonException($"Invalid WKT payload: {ex.Message}");
        }

        if (sourceSrid != 4326)
            geometry = _geometryTransformService.TransformToWgs84(geometry, sourceSrid);

        var geoJsonGeometry = GeoJsonWriter.Write(geometry);

        return _geoJsonValidationService.NormalizeForRender(geoJsonGeometry, sourceSrid: null);
    }

    private static GeoDataResponse ToGeoDataResponse(string normalizedGeoJson)
    {
        var geometryTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var coordinateCount = 0;

        var root = JsonNode.Parse(normalizedGeoJson) as JsonObject;
        var features = root?["features"] as JsonArray;

        if (features is not null)
        {
            foreach (var featureNode in features)
            {
                if (featureNode is not JsonObject featureObject)
                    continue;

                if (featureObject["geometry"] is not JsonObject geometryObject)
                    continue;

                var geometryType = geometryObject["type"]?.GetValue<string>();

                if (!string.IsNullOrWhiteSpace(geometryType))
                    geometryTypes.Add(geometryType);

                coordinateCount += CountCoordinates(geometryObject);
            }
        }

        return new GeoDataResponse
        {
            GeometryType = geometryTypes.Count switch
            {
                0 => "None",
                1 => geometryTypes.First(),
                _ => "Mixed"
            },
            CoordinateCount = coordinateCount,
            GeoJson = normalizedGeoJson,
            IsValid = true
        };
    }

    private static int CountCoordinates(JsonObject geometryObject)
    {
        var geometryType = geometryObject["type"]?.GetValue<string>();

        if (string.Equals(geometryType, "GeometryCollection", StringComparison.OrdinalIgnoreCase))
        {
            var total = 0;
            var geometries = geometryObject["geometries"] as JsonArray;

            if (geometries is null)
                return total;

            foreach (var item in geometries)
            {
                if (item is JsonObject childGeometry)
                    total += CountCoordinates(childGeometry);
            }

            return total;
        }

        return CountCoordinatePositions(geometryObject["coordinates"]);
    }

    private static int CountCoordinatePositions(JsonNode? node)
    {
        if (node is null)
            return 0;

        if (node is not JsonArray array)
            return 0;

        if (IsCoordinatePair(array))
            return 1;

        var count = 0;

        foreach (var child in array)
            count += CountCoordinatePositions(child);

        return count;
    }

    private static bool IsCoordinatePair(JsonArray array)
    {
        if (array.Count < 2)
            return false;

        return IsNumberNode(array[0]) && IsNumberNode(array[1]);
    }

    private static bool IsNumberNode(JsonNode? node)
    {
        return node is JsonValue value && value.TryGetValue<double>(out _);
    }
}

