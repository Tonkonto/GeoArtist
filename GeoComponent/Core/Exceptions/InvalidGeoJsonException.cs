namespace GeoComponent.Core.Exceptions;

/// <summary>
/// Thrown when input GeoJSON cannot be parsed or does not satisfy the normalization rules used before rendering.
/// </summary>
public class InvalidGeoJsonException(string message) : Exception(message)
{

}
