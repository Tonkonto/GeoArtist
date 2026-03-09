using GeoComponent.Core.Models;

namespace GeoComponent.Models;

public class GeoMapVm
{
    public GeoResult? Geo { get; set; }
    public IEnumerable<GeoResult>? GeoBatch { get; set; }

    public string MapId { get; set; } = "geoMap";
    public string Height { get; set; } = "500px";
    public string PolygonColor { get; set; } = "#3388ff";
    public double PolygonOpacity { get; set; } = 0.5;
}