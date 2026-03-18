window.GeoArtist = (function () {
    const maps = {};
    const layers = {};

    function ensureMap(options) {
        if (!options || !options.mapId) {
            console.error("GeoArtist.ensureMap: options.mapId is required.");
            return null;
        }

        const mapId = options.mapId;
        const mapElement = document.getElementById(mapId);

        if (!mapElement) {
            console.error("GeoArtist.ensureMap: map element not found:", mapId);
            return null;
        }

        if (maps[mapId]) {
            return maps[mapId];
        }

        const map = L.map(mapId).setView(
            [options.initialLat ?? 42.87, options.initialLng ?? 74.60],
            options.initialZoom ?? 12
        );

        if (options.showTileLayer !== false) {
            L.tileLayer(
                options.tileLayerUrl ?? "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                {
                    attribution: options.tileLayerAttribution ?? "&copy; OpenStreetMap contributors"
                }
            ).addTo(map);
        }

        maps[mapId] = map;
        layers[mapId] = null;

        return map;
    }

    function clearShapes(mapId) {
        const map = maps[mapId];
        const layer = layers[mapId];

        if (!map || !layer) {
            return;
        }

        map.removeLayer(layer);
        layers[mapId] = null;
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

    function renderMapFromPayload(payload) {
        if (!payload || !payload.mapOptions) {
            console.error("GeoArtist.renderMapFromPayload: payload.mapOptions is required.");
            return null;
        }

        const options = payload.mapOptions;
        const map = ensureMap(options);

        if (!map) {
            return null;
        }

        const mapId = options.mapId;
        clearShapes(mapId);

        const geoItems = normalizeGeoJsonInput(payload.geoJson);
        const featureLayers = [];
        let combinedBounds = null;

        for (const item of geoItems) {
            if (!item) {
                continue;
            }

            let layer;

            try {
                layer = L.geoJSON(item, {
                    style: {
                        color: options.polygonColor ?? "#3388ff",
                        opacity: options.polygonOpacity ?? 0.4
                    }
                });
            } catch (error) {
                console.error("GeoArtist.renderMapFromPayload: failed to render GeoJSON.", error, item);
                continue;
            }

            featureLayers.push(layer);

            if (typeof layer.getBounds === "function") {
                const bounds = layer.getBounds();
                if (bounds && bounds.isValid && bounds.isValid()) {
                    if (combinedBounds === null) {
                        combinedBounds = bounds;
                    } else {
                        combinedBounds.extend(bounds);
                    }
                }
            }
        }

        const group = L.featureGroup(featureLayers).addTo(map);
        layers[mapId] = group;

        if ((options.fitBounds ?? true) && combinedBounds && combinedBounds.isValid()) {
            map.fitBounds(combinedBounds);
        }

        return map;
    }

    function bootstrap(payload) {
        if (!payload) {
            console.error("GeoArtist.bootstrap: payload is required.");
            return null;
        }

        if (!payload.mapOptions) {
            console.error("GeoArtist.bootstrap: payload.mapOptions is required.");
            return null;
        }

        if (!payload.mapOptions.mapId && payload.mapId) {
            payload.mapOptions.mapId = payload.mapId;
        }

        switch ((payload.mode ?? "map").toLowerCase()) {
            case "map":
                return renderMapFromPayload(payload);

            case "editor":
                return renderMapFromPayload(payload); //♦placeholder♦

            default:
                console.error("GeoArtist.bootstrap: unsupported mode:", payload.mode);
                return null;
        }
    }

    return {
        bootstrap,
        ensureMap,
        clearShapes,
        renderMapFromPayload,
        maps,
        layers
    };
})();