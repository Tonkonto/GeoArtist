using GeoComponent.Models;

namespace GeoComponent.Facade.Interfaces;

public interface IGeoComponentFacade
{
    // Responses
        //GeoJson
    GeoDataResponse FromGeoJson(string geoJson);
    IReadOnlyList<GeoDataResponse> FromGeoJsonBatch(IEnumerable<string> geoJsonList);

        //WKT
    GeoDataResponse FromWkt(string wkt, int srid);
    IReadOnlyList<GeoDataResponse> FromWktBatch(IEnumerable<string> wktList, int srid);

    // Models
        //GeoJson
    GeoMap MapFromGeoJson(string geoJson, GeoMapOptions? options = null);
    GeoMap MapFromGeoJsonBatch(IEnumerable<string> geoJsonList, GeoMapOptions? options = null);
        //WKT
    GeoMap MapFromWkt(string wkt, int srid, GeoMapOptions? options = null);
    GeoMap MapFromWktBatch(IEnumerable<string> wktList, int srid, GeoMapOptions? options = null);
}