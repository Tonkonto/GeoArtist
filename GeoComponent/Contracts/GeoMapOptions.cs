using System.ComponentModel.DataAnnotations;

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

    [Range(0, 24)]
    public int InitialZoom { get; set; } = 12;

    [Range(0, 24)]
    public int? MaxZoom { get; set; } = 20;
    //misc
    public bool FitBounds { get; set; } = true;


    // Polygons
    public string PolygonColor { get; set; } = "#3388ff";

    [Range(0.0, 1.0)]
    public double PolygonOpacity { get; set; } = 0.5;


    // Map System
    public bool ShowTileLayer { get; set; } = true;
    public string TileLayerUrl { get; set; } = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
    public string TileLayerAttribution { get; init; } = "<a href=\"https://www.openstreetmap.org/copyright\" target=\"_blank\" rel=\"noopener noreferrer\">&copy; OpenStreetMap</a>";


    // GeoJSON data
    public int? SourceSrid { get; set; }
}
