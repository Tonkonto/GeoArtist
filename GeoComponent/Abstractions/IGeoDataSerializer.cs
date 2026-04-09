namespace GeoComponent.Abstractions;

/// <summary>
/// Serializes strongly typed options and payloads to JSON for inline scripts and WebView messages.
/// </summary>
public interface IGeoDataSerializer
{
    /// <summary>
    /// Serializes <paramref name="value"/> to JSON (camelCase, nulls omitted) for embedding in HTML or IPC.
    /// </summary>
    /// <typeparam name="T">Type to serialize.</typeparam>
    /// <param name="value">Object graph to serialize.</param>
    /// <returns>Compact JSON string.</returns>
    string Serialize<T>(T value);
}
