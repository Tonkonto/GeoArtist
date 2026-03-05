using Core.Models;

namespace Core.Interfaces
{
    public interface IGeoService
    {
        GeoResult Parse(string geoJson);
        IEnumerable<GeoResult> ParseBatch(IEnumerable<string> geoJsonCollection);
    }
}
