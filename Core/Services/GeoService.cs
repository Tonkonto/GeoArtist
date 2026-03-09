using Core.ErrorHanders;
using Core.Interfaces;
using Core.Models;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Core.Services;

public class GeoService : IGeoService
{
    private readonly GeoJsonReader _geoJsonReader;
    private readonly GeoJsonWriter _geoJsonWriter;
    private readonly WKTReader _wktReader;

    public GeoService()
    {
        _geoJsonReader = new GeoJsonReader();
        _geoJsonWriter = new GeoJsonWriter();
        _wktReader = new WKTReader();
    }

    public GeoResult ParseGeoJson(string geoJson)
    {
        if (string.IsNullOrWhiteSpace(geoJson))
            throw new InvalidGeoJsonException("GeoJson is empty.");

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
            throw new InvalidGeoJsonException("Parsed geometry is null.");

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
            throw new ArgumentException("WKT is empty.");

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
            throw new ArgumentException("Parsed WKT geometry is null.");

        geometry.SRID = srid;

        ValidateGeometryType(geometry);

        // ♦♦♦ todo ♦♦♦
        // Пока без transform.
        // Если входной SRID != 4326, для карты это надо будет преобразовать.
        // Временный вариант: считать, что уже приходит 4326.
        // Следующим шагом добавим transform.
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