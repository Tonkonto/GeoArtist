using GeoComponent.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GeoComponent.Core.Services;

/// <summary>
/// <see cref="IGeoDataSerializer"/> implementation using <see cref="System.Text.Json"/> with camelCase property names.
/// </summary>
public sealed class SystemTextJsonGeoDataSerializer : IGeoDataSerializer
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    /// <inheritdoc />
    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, _options);
    }
}
