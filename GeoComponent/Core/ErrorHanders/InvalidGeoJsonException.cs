namespace GeoComponent.Core.ErrorHanders;

public class InvalidGeoJsonException : Exception
{
    public InvalidGeoJsonException(string message)
        : base(message)
    {
    }
}