using GeoComponent.Core.Interfaces;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace GeoComponent.Core.Services;

public class GeometryTransformService : IGeometryTransformService
{
    public Geometry TransformToWgs84(Geometry geometry, int sourceSrid)
    {
        if (geometry is null)
            throw new ArgumentNullException(nameof(geometry));
        if (sourceSrid <= 0)
            throw new ArgumentException("Source SRID must be a positive EPSG code.", nameof(sourceSrid));

        if (sourceSrid == 4326)
        {
            geometry.SRID = 4326;
            return geometry;
        }

        var source = SridReader.GetCSbyID(sourceSrid);
        if (source is null)
            throw new ArgumentException($"Unsupported or unknown SRID: {sourceSrid}");

        var target = GeographicCoordinateSystem.WGS84;

        var transformFactory = new CoordinateTransformationFactory();
        var coordinateTransform = transformFactory.CreateFromCoordinateSystems(source, target);
        var mathTransform = coordinateTransform.MathTransform;

        var clone = (Geometry)geometry.Copy();
        clone.Apply(new MathTransformFilter(mathTransform));
        clone.SRID = 4326;

        return clone;
    }

    private sealed class MathTransformFilter(MathTransform mathTransform) : ICoordinateSequenceFilter
    {
        private readonly MathTransform _mathTransform = mathTransform;

        public bool Done => false;
        public bool GeometryChanged => true;

        public void Filter(CoordinateSequence seq, int i)
        {
            var transformed = _mathTransform.Transform([seq.GetX(i), seq.GetY(i)]);

            seq.SetX(i, transformed[0]);
            seq.SetY(i, transformed[1]);

            if (seq.Dimension > 2 && transformed.Length > 2)
                seq.SetOrdinate(i, Ordinate.Z, transformed[2]);
        }
    }
}