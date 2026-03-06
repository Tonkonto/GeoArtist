using NetTopologySuite.Geometries;

namespace Core.Models;

public class GeoResult
{
    public string GeometryType { get; init; } = default!;
    public int CoordinateCount { get; init; }
    public string GeoJson { get; init; } = default!;
    public NetTopologySuite.Geometries.Envelope BoundingBox { get; init; } = default!;
    public NetTopologySuite.Geometries.Coordinate Centroid { get; init; } = default!;
    public bool IsValid { get; init; }
}