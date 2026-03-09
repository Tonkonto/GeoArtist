using GeoComponent.Core.Interfaces;
using GeoComponent.Core.Models;
using GeoComponent.Facade.Interfaces;
using GeoComponent.Models;

namespace GeoComponent.Facade.Services;

public class GeoComponentFacade(IGeoService geoService) : IGeoComponentFacade
{
    private readonly IGeoService _geoService = geoService;


    public GeoDataResponse FromGeoJson(string geoJson)
    {
        var result = _geoService.ParseGeoJson(geoJson);
        return Map(result);
    }

    public IReadOnlyList<GeoDataResponse> FromGeoJsonBatch(IEnumerable<string> geoJsonList)
    {
        return _geoService
            .ParseGeoJsonBatch(geoJsonList)
            .Select(Map)
            .ToList();
    }

    public GeoDataResponse FromWkt(string wkt, int srid)
    {
        var result = _geoService.ParseWkt(wkt, srid);
        return Map(result);
    }

    public IReadOnlyList<GeoDataResponse> FromWktBatch(IEnumerable<string> wktList, int srid)
    {
        return _geoService
            .ParseWktBatch(wktList, srid)
            .Select(Map)
            .ToList();
    }

    private static GeoDataResponse Map(GeoResult x)
    {
        return new GeoDataResponse
        {
            GeometryType = x.GeometryType,
            CoordinateCount = x.CoordinateCount,
            GeoJson = x.GeoJson,
            IsValid = x.IsValid
        };
    }
}