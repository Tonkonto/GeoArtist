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
        return MapResponse(result);
    }

    public IReadOnlyList<GeoDataResponse> FromGeoJsonBatch(IEnumerable<string> geoJsonList)
    {
        return _geoService
            .ParseGeoJsonBatch(geoJsonList)
            .Select(MapResponse)
            .ToList();
    }

    public GeoDataResponse FromWkt(string wkt, int srid)
    {
        var result = _geoService.ParseWkt(wkt, srid);
        return MapResponse(result);
    }

    public IReadOnlyList<GeoDataResponse> FromWktBatch(IEnumerable<string> wktList, int srid)
    {
        return _geoService
            .ParseWktBatch(wktList, srid)
            .Select(MapResponse)
            .ToList();
    }

    public GeoMap MapFromGeoJson(string geoJson, GeoMapOptions? options = null)
    {
        return new GeoMap
        {
            Geo = _geoService.ParseGeoJson(geoJson),
            Options = options ?? new GeoMapOptions()
        };
    }

    public GeoMap MapFromGeoJsonBatch(IEnumerable<string> geoJsonList, GeoMapOptions? options = null)
    {
        return new GeoMap
        {
            GeoBatch = _geoService.ParseGeoJsonBatch(geoJsonList).ToList(),
            Options = options ?? new GeoMapOptions()
        };
    }

    public GeoMap MapFromWkt(string wkt, int srid, GeoMapOptions? options = null)
    {
        return new GeoMap
        {
            Geo = _geoService.ParseWkt(wkt, srid),
            Options = options ?? new GeoMapOptions()
        };
    }

    public GeoMap MapFromWktBatch(IEnumerable<string> wktList, int srid, GeoMapOptions? options = null)
    {
        return new GeoMap
        {
            GeoBatch = _geoService.ParseWktBatch(wktList, srid).ToList(),
            Options = options ?? new GeoMapOptions()
        };
    }

    private static GeoDataResponse MapResponse(GeoResult x)
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