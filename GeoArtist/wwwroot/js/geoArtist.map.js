window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.mapRuntime = (function () {
    const state = window.GeoArtist.state;
    const events = window.GeoArtist.events;
    const geoJson = window.GeoArtist.geoJson;

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

    function resolveZoomOption(value) {
        if (!Number.isFinite(value)) {
            return null;
        }

        return Math.max(0, Math.floor(value));
    }

    function buildTileLayerConfig(options) {
        const maxZoom = resolveZoomOption(options?.maxZoom);
        const tileLayerUrl = options?.tileLayerUrl ?? "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
        const tileLayerOptions = {
            attribution: options?.tileLayerAttribution
        };

        if (maxZoom !== null) {
            tileLayerOptions.maxZoom = maxZoom;
        }

        return {
            tileLayerUrl,
            tileLayerOptions
        };
    }

    function attachAdaptiveNativeZoomFallback(baseLayer) {
        if (!baseLayer || typeof baseLayer.on !== "function") {
            return;
        }

        const errorCountersByZoom = {};

        baseLayer.on("tileerror", function (eventArgs) {
            const failedZoom = eventArgs?.coords?.z;

            if (!Number.isFinite(failedZoom) || failedZoom <= 0) {
                return;
            }

            const nextCount = (errorCountersByZoom[failedZoom] ?? 0) + 1;
            errorCountersByZoom[failedZoom] = nextCount;

            // A few failures at the same zoom level usually means the provider has no native tiles there.
            if (nextCount < 3) {
                return;
            }

            const candidateNativeZoom = failedZoom - 1;
            const currentNativeZoom = baseLayer.options.maxNativeZoom;

            if (Number.isFinite(currentNativeZoom) && currentNativeZoom <= candidateNativeZoom) {
                return;
            }

            baseLayer.options.maxNativeZoom = candidateNativeZoom;

            if (typeof baseLayer.redraw === "function") {
                baseLayer.redraw();
            }
        });
    }

    function applyMapZoomOptions(map, options) {
        const maxZoom = resolveZoomOption(options?.maxZoom);

        if (maxZoom === null || typeof map.setMaxZoom !== "function") {
            return;
        }

        map.setMaxZoom(maxZoom);

        if (typeof map.getZoom === "function" && typeof map.setZoom === "function" && map.getZoom() > maxZoom) {
            map.setZoom(maxZoom);
        }
    }

    function resolveMapOptions(mapId, options) {
        const map = state.maps[mapId] ?? null;
        const previousOptions = map?.__geoArtistMapOptions ?? null;

        if (!previousOptions) {
            return options;
        }

        return {
            ...previousOptions,
            ...options,
            mapId
        };
    }

    function storeMapOptions(map, options) {
        if (!map) {
            return;
        }

        map.__geoArtistMapOptions = {
            ...options
        };
    }

    function syncTileLayer(map, options) {
        const existingBaseLayer = map.__geoArtistBaseLayer ?? null;

        if (existingBaseLayer) {
            map.removeLayer(existingBaseLayer);
            map.__geoArtistBaseLayer = null;
        }

        if (options?.showTileLayer === false) {
            return;
        }

        const tileLayerConfig = buildTileLayerConfig(options);
        const baseLayer = L.tileLayer(tileLayerConfig.tileLayerUrl, tileLayerConfig.tileLayerOptions).addTo(map);
        attachAdaptiveNativeZoomFallback(baseLayer);
        map.__geoArtistBaseLayer = baseLayer;
    }

    function ensureMap(options) {
        if (!options || !options.mapId) {
            console.error("GeoArtist.ensureMap: options.mapId is required.");
            return null;
        }

        const mapId = options.mapId;
        const resolvedOptions = resolveMapOptions(mapId, options);
        const mapElement = document.getElementById(mapId);

        if (!mapElement) {
            console.error("GeoArtist.ensureMap: map element not found:", mapId);
            return null;
        }

        if (state.maps[mapId]) {
            const existingMap = state.maps[mapId];
            applyMapZoomOptions(existingMap, resolvedOptions);
            syncTileLayer(existingMap, resolvedOptions);
            storeMapOptions(existingMap, resolvedOptions);
            return existingMap;
        }

        const map = L.map(mapId).setView(
            [resolvedOptions.initialLat ?? 0.0, resolvedOptions.initialLng ?? 0.0],
            resolvedOptions.initialZoom ?? 12
        );

        applyMapZoomOptions(map, resolvedOptions);
        syncTileLayer(map, resolvedOptions);
        storeMapOptions(map, resolvedOptions);

        state.maps[mapId] = map;
        state.layers[mapId] = null;

        return map;
    }

    function clearShapes(mapId) {
        const map = state.maps[mapId];
        const layer = state.layers[mapId];

        if (!map || !layer) {
            return;
        }

        map.removeLayer(layer);
        state.layers[mapId] = null;
    }

    function buildGeoJsonLayer(item, options) {
        const color = options.polygonColor ?? "red";
        const opacity = options.polygonOpacity;

        return L.geoJSON(item, {
            style: {
                color,
                fillColor: color,
                opacity,
                fillOpacity: opacity
            }
        });
    }

    function extendBoundsFromLayer(currentBounds, layer) {
        if (!layer) {
            return currentBounds;
        }

        const bounds = invokeIfFunction(layer, "getBounds");

        if (bounds && bounds.isValid && bounds.isValid()) {
            if (currentBounds === null) {
                return bounds;
            }

            currentBounds.extend(bounds);
            return currentBounds;
        }

        const latLng = invokeIfFunction(layer, "getLatLng");

        if (latLng) {
            const pointBounds = L.latLngBounds([latLng, latLng]);

            if (currentBounds === null) {
                return pointBounds;
            }

            currentBounds.extend(pointBounds);
        }

        return currentBounds;
    }

    function renderGeoItems(mapId, geoItems, options) {
        const map = ensureMap(options);

        if (!map) {
            return null;
        }

        clearShapes(mapId);

        const featureLayers = [];
        let combinedBounds = null;

        for (const item of geoItems) {
            if (!item) {
                continue;
            }

            let layer;

            try {
                layer = buildGeoJsonLayer(item, options);
            } catch (error) {
                console.error("GeoArtist.renderGeoItems: failed to render GeoJSON.", error, item);
                continue;
            }

            featureLayers.push(layer);
            combinedBounds = extendBoundsFromLayer(combinedBounds, layer);
        }

        const group = L.featureGroup(featureLayers).addTo(map);
        state.layers[mapId] = group;

        if ((options.fitBounds ?? true) && combinedBounds && combinedBounds.isValid()) {
            map.fitBounds(combinedBounds);
        }

        return map;
    }

    function renderMapFromPayload(payload) {
        if (!payload || !payload.mapOptions) {
            console.error("GeoArtist.renderMapFromPayload: payload.mapOptions is required.");
            return null;
        }

        const mapId = payload.mapOptions.mapId;
        const options = resolveMapOptions(mapId, payload.mapOptions);
        const geoItems = geoJson.normalizeGeoJsonInput(payload.geoJson);
        const map = renderGeoItems(mapId, geoItems, options);

        if (map) {
            events.emit("geoartist:ready", {
                mapId,
                mode: "map"
            });
        }

        return map;
    }

    function exportLayerFeatureCollection(mapId) {
        const layer = state.layers[mapId];

        if (!layer || typeof layer.toGeoJSON !== "function") {
            return null;
        }

        const geo = layer.toGeoJSON();

        if (!geo || typeof geo !== "object") {
            return null;
        }

        if (geo.type === "FeatureCollection") {
            return geo;
        }

        if (geo.type === "Feature") {
            return {
                type: "FeatureCollection",
                features: [geo]
            };
        }

        return {
            type: "FeatureCollection",
            features: []
        };
    }

    return {
        ensureMap,
        clearShapes,
        buildGeoJsonLayer,
        extendBoundsFromLayer,
        renderGeoItems,
        renderMapFromPayload,
        exportLayerFeatureCollection
    };
})();
