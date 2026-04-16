using ProjNet.CoordinateSystems;

namespace GeoArtist.Core.Services;

internal static class SridResolver
{
    internal static CoordinateSystem? GetById(int srid)
    {
        return srid switch
        {
            4326 => GeographicCoordinateSystem.WGS84,
            3857 => ProjectedCoordinateSystem.WebMercator,

            32642 => ProjectedCoordinateSystem.WGS84_UTM(42, true),
            32643 => ProjectedCoordinateSystem.WGS84_UTM(43, true),
            32644 => ProjectedCoordinateSystem.WGS84_UTM(44, true),
            32645 => ProjectedCoordinateSystem.WGS84_UTM(45, true),

            7694 => CreateKyrg06Zone3(),

            _ => null
        };
    }
    private static CoordinateSystem CreateKyrg06Zone3()
    {
        const string wkt = @"PROJCS[""Kyrg-06 / zone 3"",
                            GEOGCS[""Kyrg-06"",
                                DATUM[""Kyrgyzstan_Geodetic_Datum_2006"",
                                    SPHEROID[""GRS 1980"",6378137,298.257222101,
                                        AUTHORITY[""EPSG"",""7019""]],
                                    AUTHORITY[""EPSG"",""1160""]],
                                PRIMEM[""Greenwich"",0,
                                    AUTHORITY[""EPSG"",""8901""]],
                                UNIT[""degree"",0.0174532925199433,
                                    AUTHORITY[""EPSG"",""9122""]],
                                AUTHORITY[""EPSG"",""7686""]],
                            PROJECTION[""Transverse_Mercator""],
                            PARAMETER[""latitude_of_origin"",0],
                            PARAMETER[""central_meridian"",74.5166666666667],
                            PARAMETER[""scale_factor"",1],
                            PARAMETER[""false_easting"",3300000],
                            PARAMETER[""false_northing"",14743.5],
                            UNIT[""metre"",1,
                                AUTHORITY[""EPSG"",""9001""]],
                            AXIS[""Easting"",EAST],
                            AXIS[""Northing"",NORTH],
                            AUTHORITY[""EPSG"",""7694""]]";

        var factory = new CoordinateSystemFactory();
        return factory.CreateFromWkt(wkt);
    }
}