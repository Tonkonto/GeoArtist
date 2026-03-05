using Core.ErrorHanders;
using Core.Interfaces;
using Core.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Core.Services;

public class GeoService : IGeoService
{
    private readonly GeoJsonReader _reader;
    private readonly GeoJsonWriter _writer;
    private readonly GeometryFactory _factory;

    public GeoService()
    {
        var services = NtsGeometryServices.Instance;

        _factory = services.CreateGeometryFactory(srid: 4326);
        _reader = new GeoJsonReader();
        _writer = new GeoJsonWriter();
    }

    public GeoResult Parse(string geoJson)
    {
        if (string.IsNullOrWhiteSpace(geoJson))
            throw new InvalidGeoJsonException("GeoJSON is empty.");

        Geometry geometry;
        
        try
        {
            geometry = _reader.Read<Geometry>(geoJson);
        }
        catch (Exception ex)
        {
            throw new InvalidGeoJsonException($"Invalid GeoJSON: {ex.Message}");
        }

        if (geometry is null)
            throw new InvalidGeoJsonException("Parsed geometry is null.");

        geometry = _factory.CreateGeometry(geometry);

        if (geometry is not Polygon and not MultiPolygon)
            throw new InvalidGeoJsonException(
                $"Unsupported geometry type: {geometry.GeometryType}");

        return new GeoResult
        {
            GeometryType = geometry.GeometryType,
            CoordinateCount = geometry.NumPoints,
            GeoJson = _writer.Write(geometry),
            BoundingBox = geometry.EnvelopeInternal,
            Centroid = geometry.Centroid.Coordinate,
            IsValid = geometry.IsValid
        };
    }

    public IEnumerable<GeoResult> ParseBatch(IEnumerable<string> geoJsonCollection)
    {
        foreach (var geo in geoJsonCollection)
            yield return Parse(geo);
    }
}