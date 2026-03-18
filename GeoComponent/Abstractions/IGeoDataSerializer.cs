namespace GeoComponent.Abstractions;

public interface IGeoDataSerializer
{
    string Serialize<T>(T value);
}