using GeoComponent.Core.Models;

namespace GeoComponent.Models;

public class GeoMap
{
    public GeoResult? Geo { get; set; }
    public IEnumerable<GeoResult>? GeoBatch { get; set; }
    public GeoMapOptions Options { get; set; } = new();
}