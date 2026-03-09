using GeoComponent.Models;

namespace GeoComponent.Facade.Interfaces;

public interface IGeoComponentFacade
{
    GeoDataResponse FromGeoJson(string geoJson);
    IReadOnlyList<GeoDataResponse> FromGeoJsonBatch(IEnumerable<string> geoJsonList);

    GeoDataResponse FromWkt(string wkt, int srid);
    IReadOnlyList<GeoDataResponse> FromWktBatch(IEnumerable<string> wktList, int srid);
}