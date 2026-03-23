namespace GeoComponent.Contracts;

public sealed class GeoComponentAssetOptions
{
    public List<string> CssPaths { get; set; } =
    [
        "/_content/GeoComponent/css/geoArtist.css"
    ];

    public List<string> JsPaths { get; set; } =
    [
        "/_content/GeoComponent/js/geoArtist.state.js",
        "/_content/GeoComponent/js/geoArtist.events.js",
        "/_content/GeoComponent/js/geoArtist.geojson.js",
        "/_content/GeoComponent/js/geoArtist.map.js",
        "/_content/GeoComponent/js/geoArtist.geoman.js",
        "/_content/GeoComponent/js/geoArtist.editor.js",
        "/_content/GeoComponent/js/geoArtist.js"
    ];

    public List<string> LeafletCssPaths { get; set; } =
    [
        "https://unpkg.com/leaflet/dist/leaflet.css"
    ];

    public List<string> LeafletJsPaths { get; set; } =
    [
        "https://unpkg.com/leaflet/dist/leaflet.js"
    ];

    public List<string> GeomanCssPaths { get; set; } =
    [
        "https://unpkg.com/@geoman-io/leaflet-geoman-free@2.18.3/dist/leaflet-geoman.css"
    ];

    public List<string> GeomanJsPaths { get; set; } =
    [
        "https://unpkg.com/@geoman-io/leaflet-geoman-free@2.18.3/dist/leaflet-geoman.min.js"
    ];
}
