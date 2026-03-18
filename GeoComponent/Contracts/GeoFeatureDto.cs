namespace GeoComponent.Contracts;

public sealed class GeoFeatureDto
{
    public string? Id { get; set; }
    public string? GeometryType { get; set; }
    public string? PropertiesJson { get; set; }
}