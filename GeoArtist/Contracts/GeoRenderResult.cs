namespace GeoArtist.Contracts;

/// <summary>
/// Output of server-side rendering: HTML fragment, bootstrap script, asset URLs, and JSON payload for hosts (e.g. WebView messaging).
/// </summary>
public sealed class GeoRenderResult
{
    /// <summary>
    /// HTML fragment containing the map shell and an inline script that calls <c>GeoArtist.initialize</c> when assets are embedded.
    /// </summary>
    public string Html { get; set; } = "";

    /// <summary>
    /// Standalone bootstrap script (same logic as embedded in <see cref="Html"/>), useful for custom layout composition.
    /// </summary>
    public string BootstrapScript { get; set; } = "";

    /// <summary>
    /// Map container element id (from options) for correlating instances on the page.
    /// </summary>
    public string MapId { get; set; } = "";

    /// <summary>
    /// JSON serialization of the <see cref="GeoArtistPayload"/> (camelCase) for <c>postMessage</c> or other IPC.
    /// </summary>
    public string SerializedPayload { get; set; } = "";

    /// <summary>
    /// Stylesheet URLs in load order: Leaflet, component CSS, and Geoman CSS when in editor mode.
    /// </summary>
    public IReadOnlyList<string> StylePaths { get; init; } = [];

    /// <summary>
    /// Script URLs in load order: Leaflet, Geoman (editor), then GeoArtist modules (<c>geoArtist.hostRuntime.js</c> before <c>geoArtist.js</c>).
    /// </summary>
    public IReadOnlyList<string> ScriptPaths { get; init; } = [];
}
