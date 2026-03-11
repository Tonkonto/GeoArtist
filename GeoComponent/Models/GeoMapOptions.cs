namespace GeoComponent.Models;

public class GeoMapOptions
{
    public string MapId { get; set; } = $"geoMap_{Guid.NewGuid():N}";
    public string Height { get; set; } = "600px";

    public string PolygonColor { get; set; } = "#3388ff";
    public double PolygonOpacity { get; set; } = 0.5;

    public int InitialZoom { get; set; } = 12;
    public double InitialLat { get; set; } = 42.87;
    public double InitialLng { get; set; } = 74.60;

    public bool FitBounds { get; set; } = true;
    public bool ShowTileLayer { get; set; } = true;

    public string TileLayerUrl { get; set; } = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
    public string TileLayerAttribution { get; init; } = "<a href=\"https://www.openstreetmap.org/copyright\" target=\"_blank\" rel=\"noopener noreferrer\">&copy; OpenStreetMap</a>";
}
