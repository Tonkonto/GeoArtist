using GeoComponent.Core.Models;

namespace GeoComponent.Models;

public class GeoMapVm
{
    public GeoResult? Geo { get; set; }
    public IEnumerable<GeoResult>? GeoBatch { get; set; }
    public GeoMapOptions Options { get; set; } = new();
}