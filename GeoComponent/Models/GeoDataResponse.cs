namespace GeoComponent.Models;

public class GeoDataResponse
{
    public string GeometryType { get; set; } = default!;
    public int CoordinateCount { get; set; }
    public string GeoJson { get; set; } = default!;
    public bool IsValid { get; set; }
}