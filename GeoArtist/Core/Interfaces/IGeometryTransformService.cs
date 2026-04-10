using GeoArtist.Contracts;
using NetTopologySuite.Geometries;

namespace GeoArtist.Core.Interfaces;

/// <summary>
/// Coordinate transformation helper used when <see cref="GeoMapOptions.SourceSrid"/> requests reprojection to WGS 84.
/// </summary>
public interface IGeometryTransformService
{
    /// <summary>
    /// Transforms <paramref name="geometry"/> from <paramref name="sourceSrid"/> to EPSG:4326 (longitude/latitude).
    /// </summary>
    /// <param name="geometry">Geometry read from GeoJSON (NTS).</param>
    /// <param name="sourceSrid">EPSG code of the input coordinates.</param>
    /// <returns>A new geometry with <c>SRID</c> 4326.</returns>
    Geometry TransformToWgs84(Geometry geometry, int sourceSrid);
}
