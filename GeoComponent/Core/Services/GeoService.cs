using GeoComponent.Core.ErrorHanders;
using GeoComponent.Core.Interfaces;
using GeoComponent.Core.Models;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace GeoComponent.Core.Services;

public class GeoService(
        GeoJsonReader geoJsonReader,
        GeoJsonWriter geoJsonWriter,
        WKTReader wktReader,
        IGeometryTransformService geometryTransformService)
    : IGeoService
{
    private readonly GeoJsonReader _geoJsonReader = geoJsonReader;
    private readonly GeoJsonWriter _geoJsonWriter = geoJsonWriter;
    private readonly WKTReader _wktReader = wktReader;
    private readonly IGeometryTransformService _geometryTransformService = geometryTransformService;


    public GeoResult ParseGeoJson(string geoJson)
    {
        if (string.IsNullOrWhiteSpace(geoJson))
            throw new InvalidGeoJsonException("GeoJson is empty");

        Geometry geometry;
        try
        {
            geometry = _geoJsonReader.Read<Geometry>(geoJson);
        }
        catch (Exception ex)
        {
            throw new InvalidGeoJsonException($"Invalid GeoJson: {ex.Message}");
        }

        if (geometry is null)
            throw new InvalidGeoJsonException("Parsed geometry is null");

        geometry.SRID = 4326;

        ValidateGeometryType(geometry);

        return BuildResult(geometry);
    }

    public IEnumerable<GeoResult> ParseGeoJsonBatch(IEnumerable<string> geoJsonCollection)
    {
        foreach (var geoJson in geoJsonCollection)
            yield return ParseGeoJson(geoJson);
    }

    public GeoResult ParseWkt(string wkt, int srid)
    {
        if (string.IsNullOrWhiteSpace(wkt))
            throw new ArgumentException("WKT is empty");

        Geometry geometry;

        try
        {
            geometry = _wktReader.Read(wkt);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Invalid WKT: {ex.Message}");
        }

        if (geometry is null)
            throw new ArgumentException("Parsed WKT geometry is null");

        geometry.SRID = srid;

        geometry = _geometryTransformService.TransformToWgs84(geometry, srid);

        ValidateGeometryType(geometry);

        return BuildResult(geometry);
    }

    public IEnumerable<GeoResult> ParseWktBatch(IEnumerable<string> wktCollection, int srid)
    {
        foreach (var wkt in wktCollection)
            yield return ParseWkt(wkt, srid);
    }

    private static void ValidateGeometryType(Geometry geometry)
    {
        if (geometry is not Polygon && geometry is not MultiPolygon)
            throw new ArgumentException($"Unsupported geometry type: {geometry.GeometryType}");
    }

    private GeoResult BuildResult(Geometry geometry)
    {
        return new GeoResult
        {
            GeometryType = geometry.GeometryType,
            CoordinateCount = geometry.NumPoints,
            GeoJson = _geoJsonWriter.Write(geometry),
            BoundingBox = geometry.EnvelopeInternal,
            Centroid = geometry.Centroid.Coordinate,
            IsValid = geometry.IsValid
        };
    }
}