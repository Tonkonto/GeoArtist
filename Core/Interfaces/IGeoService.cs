using Core.Models;

namespace Core.Interfaces
{
    public interface IGeoService
    {
        GeoParseResult Parse(string geoJson);
        IEnumerable<GeoParseResult> ParseBatch(IEnumerable<string> geoJsonCollection);
    }
}
