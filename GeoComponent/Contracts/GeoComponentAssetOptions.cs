namespace GeoComponent.Contracts;

/// <summary>
/// Configures URIs for CSS and JavaScript dependencies loaded around rendered HTML.
/// </summary>
/// <remarks>
/// Override these in <c>services.AddGeoComponent(assets =&gt; …)</c> to point at CDNs, local static files, or a custom bundle.
/// </remarks>
public sealed class GeoComponentAssetOptions
{
    /// <summary>
    /// GeoArtist and host-specific stylesheets (after Leaflet)
    /// </summary>
    public List<string> CssPaths { get; set; } =
    [
        "/_content/GeoComponent/css/geoArtist.css"
    ];

    /// <summary>
    /// GeoArtist script modules in dependency order
    /// </summary>
    public List<string> JsPaths { get; set; } =
    [
        "/_content/GeoComponent/js/geoArtist.messages.js",
        "/_content/GeoComponent/js/geoArtist.state.js",
        "/_content/GeoComponent/js/geoArtist.events.js",
        "/_content/GeoComponent/js/geoArtist.geojson.js",
        "/_content/GeoComponent/js/geoArtist.map.js",
        "/_content/GeoComponent/js/geoArtist.geoman.js",
        "/_content/GeoComponent/js/geoArtist.editor.js",
        "/_content/GeoComponent/js/geoArtist.js"
    ];

    /// <summary>
    /// Leaflet stylesheet URLs (loaded before <see cref="CssPaths"/>).
    /// </summary>
    public List<string> LeafletCssPaths { get; set; } =
    [
        "https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"
    ];

    /// <summary>
    /// Leaflet script URLs (loaded before Geoman and <see cref="JsPaths"/>).
    /// </summary>
    public List<string> LeafletJsPaths { get; set; } =
    [
        "https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"
    ];

    /// <summary>
    /// Geoman stylesheet URLs; included only when rendering editor mode.
    /// </summary>
    public List<string> GeomanCssPaths { get; set; } =
    [
        "https://unpkg.com/@geoman-io/leaflet-geoman-free@2.18.3/dist/leaflet-geoman.css"
    ];

    /// <summary>
    /// Geoman script URLs; included only when rendering editor mode.
    /// </summary>
    public List<string> GeomanJsPaths { get; set; } =
    [
        "https://unpkg.com/@geoman-io/leaflet-geoman-free@2.18.3/dist/leaflet-geoman.min.js"
    ];
}
