using NetTopologySuite.Geometries;

namespace GeoComponent.Core.Interfaces;

public interface IGeometryTransformService
{
    Geometry TransformToWgs84(Geometry geometry, int sourceSrid);
}
