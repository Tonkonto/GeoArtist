namespace GeoArtist.Contracts;

/// <summary>
/// Serializable payload passed to the browser runtime (<c>GeoArtist.initialize</c>) after server-side normalization.
/// </summary>
public sealed class GeoArtistPayload
{
    /// <summary>
    /// Host mode: <c>Map</c> (read-only) or <c>Editor</c> (Geoman + optional textarea).
    /// </summary>
    public string Mode { get; set; } = "map";

    /// <summary>
    /// Duplicate of <see cref="GeoMapOptions.MapId"/> for convenience in the client script bootstrap.
    /// </summary>
    public string MapId { get; set; } = "geoartist-map";

    /// <summary>
    /// Normalized GeoJSON string (FeatureCollection in WGS 84) ready for Leaflet GeoJSON layers.
    /// </summary>
    public string GeoJson { get; set; } = "";

    /// <summary>
    /// Map layout, tiles, and styling options consumed by the Leaflet runtime.
    /// </summary>
    public GeoMapOptions MapOptions { get; set; } = new();

    /// <summary>
    /// Editor-only options; ignored when <see cref="Mode"/> is <c>Map</c>.
    /// </summary>
    public GeoEditorOptions? EditorOptions { get; set; }

    /// <summary>
    /// Gets whether the payload targets editor mode (<see cref="Mode"/> equals <c>editor</c>, case-insensitive).
    /// </summary>
    /// <value><see langword="true"/> if editor assets and UI should load.</value>
    public bool IsEditable => string.Equals(Mode, "editor", StringComparison.OrdinalIgnoreCase);
}
