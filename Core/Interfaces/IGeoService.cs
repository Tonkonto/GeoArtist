using Core.Models;

namespace Core.Interfaces;

public interface IGeoService
{
    // GeoJson format
    GeoResult ParseGeoJson(string geoJson);
    IEnumerable<GeoResult> ParseGeoJsonBatch(IEnumerable<string> geoJsonCollection);

    // Wkt format
    GeoResult ParseWkt(string wkt, int srid);
    IEnumerable<GeoResult> ParseWktBatch(IEnumerable<string> wktCollection, int srid);
}
