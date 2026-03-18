namespace GeoComponent.Contracts;

public sealed class GeoMapOptions
{
    // DOM data
    public string MapId { get; set; } = "geoartist-map";
    public string Height { get; set; } = "600px";

    // Map
        //position
    public double InitialLat { get; set; } = 42.8746;
    public double InitialLng { get; set; } = 74.5698;
    public int InitialZoom { get; set; } = 12;
        //misc
    public bool FitBounds { get; set; } = true;

    // Polygons
    public string PolygonColor { get; set; } = "#3388ff";
    public double PolygonOpacity { get; set; } = 0.4;

    // Map System
    public bool ShowTileLayer { get; set; } = true;
    public string TileLayerUrl { get; set; } = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
    public string TileLayerAttribution { get; init; } = "<a href=\"https://www.openstreetmap.org/copyright\" target=\"_blank\" rel=\"noopener noreferrer\">&copy; OpenStreetMap</a>";
}