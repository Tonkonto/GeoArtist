namespace Core.Models
{
    public class GeoParseResult
    {
        public string GeometryType { get; init; } = default!;
        public int CoordinateCount { get; init; }
        public string NormalizedGeoJson { get; init; } = default!;
    }
}
