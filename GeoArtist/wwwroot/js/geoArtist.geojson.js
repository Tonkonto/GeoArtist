window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.geoJson = (function () {
    // Data for exporting Leaflet Circle layers as GeoJSON Polygon
    const Earth_Mean_Radius = 6371008.8; //Earth arithmetic mean radius
    //number of vertices for circle to polygon approximation
    let circleExportVertexCount = 60;   // default, the closing node will be added automatically

    function getCircleExportVertexCount() {
        return circleExportVertexCount;
    }

    function setCircleExportVertexCount(count) {
        const minCircleExportVertexCount = 3;
        const maxCircleExportVertexCount = 100000;

        if (!Number.isFinite(count) || count < minCircleExportVertexCount || count > maxCircleExportVertexCount) {
            console.warn(
                `GeoArtist.setCircleExportVertexCount: expected integer in [${minCircleExportVertexCount}, ${maxCircleExportVertexCount}], got:`,
                count
            );
            return false;
        }

        circleExportVertexCount = count;
        return true;
    }

    function invokeIfFunction(target, methodName, args) {
        if (!target) {
            return undefined;
        }

        const method = target[methodName];

        if (typeof method !== "function") {
            return undefined;
        }

        return method.apply(target, args ?? []);
    }

    function isLeafletGeographicCircle(layer) {
        return typeof L !== "undefined" && layer instanceof L.Circle;
    }

    function offsetLatLngByMeters(latDeg, lngDeg, bearingDeg, distanceM) {
        if (
            typeof latDeg !== "number" || !Number.isFinite(latDeg) ||
            typeof lngDeg !== "number" || !Number.isFinite(lngDeg) ||
            typeof bearingDeg !== "number" || !Number.isFinite(bearingDeg) ||
            typeof distanceM !== "number" || !Number.isFinite(distanceM)
        ) {
            return null;
        }

        const phi1 = latDeg * Math.PI / 180;
        const lambda1 = lngDeg * Math.PI / 180;
        const theta = bearingDeg * Math.PI / 180;
        const angularDist = distanceM / Earth_Mean_Radius;

        const sinPhi1 = Math.sin(phi1);
        const cosPhi1 = Math.cos(phi1);
        const sinAd = Math.sin(angularDist);
        const cosAd = Math.cos(angularDist);

        const sinPhi2 = sinPhi1 * cosAd + cosPhi1 * sinAd * Math.cos(theta);
        const phi2 = Math.asin(sinPhi2);
        const y = Math.sin(theta) * sinAd * cosPhi1;
        const x = cosAd - sinPhi1 * sinPhi2;
        let lambda2 = lambda1 + Math.atan2(y, x);

        let lngOut = lambda2 * 180 / Math.PI;
        lngOut = ((lngOut + 540) % 360) - 180;
        const latOut = phi2 * 180 / Math.PI;

        if (typeof latOut !== "number" || !Number.isFinite(latOut) ||
            typeof lngOut !== "number" || !Number.isFinite(lngOut)) {
            return null;
        }

        return { lat: latOut, lng: lngOut };
    }

    function circleLayerToPolygonFeature(layer) {
        const latLng = invokeIfFunction(layer, "getLatLng");
        const radius = invokeIfFunction(layer, "getRadius");

        if (!latLng ||
            typeof latLng.lat !== "number" || !Number.isFinite(latLng.lat) ||
            typeof latLng.lng !== "number" || !Number.isFinite(latLng.lng) ||
            typeof radius !== "number" || !Number.isFinite(radius) ||
            radius <= 0) {
            return null;
        }

        const ring = [];

        for (let i = 0; i < circleExportVertexCount; i++) {
            const bearing = (360 * i) / circleExportVertexCount;
            const p = offsetLatLngByMeters(latLng.lat, latLng.lng, bearing, radius);

            if (!p) {
                return null;
            }

            ring.push([p.lng, p.lat]);
        }

        ring.push(ring[0]);

        const props = { ...(layer.feature?.properties ?? {}) };

        delete props.geoartistCircle;

        props.shape = "Circle";
        props.radius = radius;
        props.center = [latLng.lng, latLng.lat];

        return {
            type: "Feature",
            geometry: {
                type: "Polygon",
                coordinates: [ring]
            },
            properties: props
        };
    }

    function getLayerShape(layer) {
        const shape = invokeIfFunction(layer?.pm, "getShape");

        if (typeof shape === "string" && shape.length > 0) {
            return shape;
        }

        const fallbackShape = layer?.feature?.properties?.shape;
        if (typeof fallbackShape === "string" && fallbackShape.length > 0) {
            return fallbackShape;
        }

        return null;
    }

    function enrichFeaturePropertiesByLayer(feature, layer) {
        if (!feature || feature.type !== "Feature" || !layer) {
            return feature;
        }

        const properties = {
            ...(feature.properties ?? {})
        };

        const shape = getLayerShape(layer);
        const shapeLower = typeof shape === "string" ? shape.toLowerCase() : "";

        if (shapeLower === "circlemarker") {
            properties.shape = "CircleMarker";
        } else if (shapeLower === "text") {
            properties.shape = "Text";
            const textValue = layer?.options?.text;
            if (typeof textValue === "string") {
                properties.text = textValue;
            } else if (typeof properties.text !== "string") {
                properties.text = "";
            }
        }

        feature.properties = properties;
        return feature;
    }

    function normalizeGeoJsonInput(geoJson) {
        if (!geoJson) {
            return [];
        }

        if (Array.isArray(geoJson)) {
            return geoJson;
        }

        if (typeof geoJson === "string") {
            try {
                const parsed = JSON.parse(geoJson);

                if (Array.isArray(parsed)) {
                    return parsed;
                }

                return [parsed];
            } catch (error) {
                console.error("GeoArtist.normalizeGeoJsonInput: invalid geoJson string.", error);
                return [];
            }
        }

        if (typeof geoJson === "object") {
            return [geoJson];
        }

        return [];
    }

    function expandGeoJsonItems(geoJson) {
        const normalized = normalizeGeoJsonInput(geoJson);
        const result = [];

        for (const item of normalized) {
            if (!item || typeof item !== "object") {
                continue;
            }

            if (item.type === "FeatureCollection" && Array.isArray(item.features)) {
                for (const feature of item.features) {
                    if (feature) {
                        result.push(feature);
                    }
                }

                continue;
            }

            result.push(item);
        }

        return result;
    }

    function toEditableJsonText(geoJson) {
        if (!geoJson) {
            return "";
        }

        if (typeof geoJson === "string") {
            try {
                const parsed = JSON.parse(geoJson);
                return JSON.stringify(parsed, null, 2);
            } catch {
                return geoJson;
            }
        }

        try {
            return JSON.stringify(geoJson, null, 2);
        } catch {
            return "";
        }
    }

    function tryParseEditorText(text) {
        if (!text || !text.trim()) {
            return {
                ok: true,
                value: {
                    type: "FeatureCollection",
                    features: []
                }
            };
        }

        try {
            const parsed = JSON.parse(text);

            if (parsed && parsed.type === "FeatureCollection" && Array.isArray(parsed.features)) {
                return { ok: true, value: parsed };
            }

            if (Array.isArray(parsed)) {
                return {
                    ok: true,
                    value: {
                        type: "FeatureCollection",
                        features: parsed
                    }
                };
            }

            return {
                ok: true,
                value: {
                    type: "FeatureCollection",
                    features: [parsed]
                }
            };
        } catch (error) {
            return { ok: false, error };
        }
    }

    function preserveFeatureMetadata(innerLayer, sourceItem) {
        if (!innerLayer || !sourceItem || typeof sourceItem !== "object") {
            return;
        }

        if (sourceItem.type === "Feature") {
            innerLayer.feature = innerLayer.feature || {};
            innerLayer.feature.type = "Feature";
            innerLayer.feature.geometry = innerLayer.feature.geometry ?? sourceItem.geometry ?? null;
            innerLayer.feature.properties = sourceItem.properties ?? {};
            return;
        }

        if (sourceItem.properties) {
            innerLayer.feature = innerLayer.feature || {};
            innerLayer.feature.type = innerLayer.feature.type ?? "Feature";
            innerLayer.feature.properties = sourceItem.properties;
        }
    }

    function exportEditorGeoItems(editorState) {
        const features = [];

        if (!editorState || !editorState.featureGroup) {
            return {
                type: "FeatureCollection",
                features
            };
        }

        editorState.featureGroup.eachLayer(function (layer) {
            if (isLeafletGeographicCircle(layer)) {
                const circleFeature = circleLayerToPolygonFeature(layer);

                if (circleFeature) {
                    features.push(circleFeature);
                    return;
                }
            }

            const geo = invokeIfFunction(layer, "toGeoJSON");

            if (!geo || typeof geo !== "object") {
                return;
            }

            if (geo.type === "FeatureCollection" && Array.isArray(geo.features)) {
                for (const feature of geo.features) {
                    if (feature) {
                        features.push(feature);
                    }
                }

                return;
            }

            if (geo.type === "Feature") {
                if (layer.feature && layer.feature.properties) {
                    geo.properties = {
                        ...(geo.properties ?? {}),
                        ...layer.feature.properties
                    };
                }

                features.push(enrichFeaturePropertiesByLayer(geo, layer));
            }
        });

        return {
            type: "FeatureCollection",
            features
        };
    }

    return {
        normalizeGeoJsonInput,
        expandGeoJsonItems,
        toEditableJsonText,
        tryParseEditorText,
        preserveFeatureMetadata,
        exportEditorGeoItems,
        enrichFeaturePropertiesByLayer,
        getCircleExportVertexCount,
        setCircleExportVertexCount
    };
})();
