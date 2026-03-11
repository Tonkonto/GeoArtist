using NetTopologySuite.Geometries;

public interface IGeometryTransformService
{
    Geometry TransformToWgs84(Geometry geometry, int sourceSrid);
}
