using GeoComponent.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoComponent.Core.Services;

public sealed class SystemTextJsonGeoDataSerializer : IGeoDataSerializer
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, _options);
    }
}