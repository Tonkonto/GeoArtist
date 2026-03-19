using System.Text.Json;
using System.Text.Json.Nodes;
using GeoComponent.Core.Exceptions;
using GeoComponent.Core.Interfaces;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace GeoComponent.Core.Services;

public sealed class GeoJsonValidationService(IGeometryTransformService geometryTransformService)
{
    private readonly IGeometryTransformService _geometryTransformService = geometryTransformService;

    public string NormalizeForRender(string? geoJson, int? sourceSrid)
    {
        var featureCollection = string.IsNullOrWhiteSpace(geoJson)
            ? CreateEmptyFeatureCollection()
            : ToFeatureCollection(ParseInputStrict(geoJson));

        ValidateFeatureCollection(featureCollection);

        if (sourceSrid.HasValue)
        {
            if (sourceSrid.Value <= 0)
                throw new ArgumentException("Source SRID must be a positive EPSG code.", nameof(sourceSrid));

            if (sourceSrid.Value != 4326)
                TransformFeatureCollectionToWgs84(featureCollection, sourceSrid.Value);
        }

        return featureCollection.ToJsonString();
    }

    private static JsonNode ParseInputStrict(string geoJson)
    {
        try
        {
            return JsonNode.Parse(geoJson) ?? throw new InvalidGeoJsonException("GeoJSON payload is null.");
        }
        catch (JsonException ex)
        {
            throw new InvalidGeoJsonException($"Invalid GeoJSON payload: {ex.Message}");
        }
    }

    private static JsonObject ToFeatureCollection(JsonNode sourceNode)
    {
        if (sourceNode is JsonObject sourceObject)
            return ToFeatureCollectionFromObject(sourceObject);

        if (sourceNode is JsonArray sourceArray)
            return ToFeatureCollectionFromArray(sourceArray);

        throw new InvalidGeoJsonException("GeoJSON root must be an object or array.");
    }

    private static JsonObject ToFeatureCollectionFromObject(JsonObject sourceObject)
    {
        var type = sourceObject["type"]?.GetValue<string>();

        if (string.Equals(type, "FeatureCollection", StringComparison.OrdinalIgnoreCase))
        {
            var cloned = sourceObject.DeepClone() as JsonObject ?? CreateEmptyFeatureCollection();

            if (cloned["features"] is not JsonArray)
                throw new InvalidGeoJsonException("FeatureCollection must contain a 'features' array.");

            cloned["type"] = "FeatureCollection";
            return cloned;
        }

        var featureCollection = CreateEmptyFeatureCollection();
        var features = (JsonArray)featureCollection["features"]!;
        features.Add(NormalizeObjectAsFeature(sourceObject));

        return featureCollection;
    }

    private static JsonObject ToFeatureCollectionFromArray(JsonArray sourceArray)
    {
        var featureCollection = CreateEmptyFeatureCollection();
        var features = (JsonArray)featureCollection["features"]!;

        foreach (var item in sourceArray)
        {
            if (item is not JsonObject itemObject)
                throw new InvalidGeoJsonException("GeoJSON arrays must contain objects only.");

            var type = itemObject["type"]?.GetValue<string>();
            if (string.Equals(type, "FeatureCollection", StringComparison.OrdinalIgnoreCase))
                throw new InvalidGeoJsonException("Nested FeatureCollection is not allowed in top-level arrays.");

            features.Add(NormalizeObjectAsFeature(itemObject));
        }

        return featureCollection;
    }

    private static JsonObject NormalizeObjectAsFeature(JsonObject sourceObject)
    {
        var type = sourceObject["type"]?.GetValue<string>();

        if (string.Equals(type, "Feature", StringComparison.OrdinalIgnoreCase))
            return sourceObject.DeepClone() as JsonObject ?? throw new InvalidGeoJsonException("Invalid Feature node.");

        if (IsGeometryType(type))
            return WrapGeometryAsFeature(sourceObject);

        if (sourceObject["geometry"] is JsonObject geometryObject)
        {
            var feature = WrapGeometryAsFeature(geometryObject);
            var propertiesNode = sourceObject["properties"];

            if (propertiesNode is JsonObject propertiesObject)
                feature["properties"] = propertiesObject.DeepClone();
            else if (propertiesNode is null)
                feature["properties"] = new JsonObject();
            else
                throw new InvalidGeoJsonException("Feature 'properties' must be an object.");

            return feature;
        }

        throw new InvalidGeoJsonException("Object cannot be converted to FeatureCollection contract.");
    }

    private static bool IsGeometryType(string? type)
    {
        return type switch
        {
            "Point" => true,
            "MultiPoint" => true,
            "LineString" => true,
            "MultiLineString" => true,
            "Polygon" => true,
            "MultiPolygon" => true,
            "GeometryCollection" => true,
            _ => false
        };
    }

    private static JsonObject WrapGeometryAsFeature(JsonNode geometryNode)
    {
        return new JsonObject
        {
            ["type"] = "Feature",
            ["geometry"] = geometryNode.DeepClone(),
            ["properties"] = new JsonObject()
        };
    }

    private static void ValidateFeatureCollection(JsonObject featureCollection)
    {
        var type = featureCollection["type"]?.GetValue<string>();
        if (!string.Equals(type, "FeatureCollection", StringComparison.Ordinal))
            throw new InvalidGeoJsonException("Payload must be normalized to FeatureCollection.");

        if (featureCollection["features"] is not JsonArray features)
            throw new InvalidGeoJsonException("FeatureCollection must contain a 'features' array.");

        var geoJsonReader = new GeoJsonReader();

        for (var i = 0; i < features.Count; i++)
        {
            if (features[i] is not JsonObject feature)
                throw new InvalidGeoJsonException($"Feature at index {i} is not an object.");

            var featureType = feature["type"]?.GetValue<string>();
            if (!string.Equals(featureType, "Feature", StringComparison.Ordinal))
                throw new InvalidGeoJsonException($"Feature at index {i} must have type 'Feature'.");

            if (feature["properties"] is not null && feature["properties"] is not JsonObject)
                throw new InvalidGeoJsonException($"Feature properties at index {i} must be an object.");

            var geometryNode = feature["geometry"];
            if (geometryNode is null)
                continue;

            if (geometryNode is not JsonObject)
                throw new InvalidGeoJsonException($"Feature geometry at index {i} must be an object.");

            try
            {
                var geometry = geoJsonReader.Read<Geometry>(geometryNode.ToJsonString());
                _ = geometry ?? throw new InvalidGeoJsonException($"Feature geometry at index {i} is null.");
            }
            catch (InvalidGeoJsonException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidGeoJsonException($"Invalid feature geometry at index {i}: {ex.Message}");
            }
        }
    }

    private void TransformFeatureCollectionToWgs84(JsonObject featureCollection, int sourceSrid)
    {
        if (featureCollection["features"] is not JsonArray features)
            return;

        var geoJsonReader = new GeoJsonReader();
        var geoJsonWriter = new GeoJsonWriter();

        for (var i = 0; i < features.Count; i++)
        {
            if (features[i] is not JsonObject feature)
                continue;

            var geometryNode = feature["geometry"];
            if (geometryNode is null)
                continue;

            if (geometryNode is not JsonObject)
                throw new InvalidGeoJsonException($"Feature geometry at index {i} must be an object.");

            try
            {
                var geometry = geoJsonReader.Read<Geometry>(geometryNode.ToJsonString())
                               ?? throw new InvalidGeoJsonException($"Feature geometry at index {i} is null.");

                var transformed = _geometryTransformService.TransformToWgs84(geometry, sourceSrid);
                feature["geometry"] = JsonNode.Parse(geoJsonWriter.Write(transformed));
            }
            catch (InvalidGeoJsonException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidGeoJsonException($"Failed to transform feature geometry at index {i}: {ex.Message}");
            }
        }
    }

    private static JsonObject CreateEmptyFeatureCollection()
    {
        return new JsonObject
        {
            ["type"] = "FeatureCollection",
            ["features"] = new JsonArray()
        };
    }
}
