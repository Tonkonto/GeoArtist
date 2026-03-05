using Core.ErrorHanders;
using Core.Interfaces;
using Core.Models;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services;

public class GeoService(GeoJsonReader reader, GeoJsonWriter writer) : IGeoService
{
    private readonly GeoJsonReader _reader = reader;
    private readonly GeoJsonWriter _writer = writer;


    public GeoParseResult Parse(string geoJson)
    {
        if (string.IsNullOrWhiteSpace(geoJson))
            throw new InvalidGeoJsonException("GeoJSON is empty.");

        Geometry geometry;

        try
        {
            geometry = _reader.Read<Geometry>(geoJson);
        }
        catch (Exception ex) {
            throw new InvalidGeoJsonException($"Invalid GeoJSON: {ex.Message}"); }

        if (geometry == null)
            throw new InvalidGeoJsonException("Parsed geometry is null.");

        if (geometry is not Polygon and not MultiPolygon)
            throw new InvalidGeoJsonException($"Unsupported geometry type: {geometry.GeometryType}");

        return new GeoParseResult
        {
            GeometryType = geometry.GeometryType,
            CoordinateCount = geometry.Coordinates.Length,
            NormalizedGeoJson = _writer.Write(geometry)
        };
    }

    public IEnumerable<GeoParseResult> ParseBatch(IEnumerable<string> geoJsonCollection)
    {
        foreach (var geo in geoJsonCollection)
        {
            yield return Parse(geo);
        }
    }
}
