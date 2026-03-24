window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.geoJson = (function () {
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

                features.push(geo);
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
        exportEditorGeoItems
    };
})();
