namespace GeoComponent.Core.ErrorHanders;

public class InvalidGeoJsonException(string message) : Exception(message)
{

}