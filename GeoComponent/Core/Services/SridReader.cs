using ProjNet.CoordinateSystems;

namespace GeoComponent.Core.Services;

internal static class SridReader
{
    internal static CoordinateSystem? GetCSbyID(int srid)
    {
        return srid switch
        {
            4326 => GeographicCoordinateSystem.WGS84,
            3857 => ProjectedCoordinateSystem.WebMercator,

            32642 => ProjectedCoordinateSystem.WGS84_UTM(42, true),
            32643 => ProjectedCoordinateSystem.WGS84_UTM(43, true),
            32644 => ProjectedCoordinateSystem.WGS84_UTM(44, true),
            32645 => ProjectedCoordinateSystem.WGS84_UTM(45, true),

            _ => null
        };
    }
}