window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.mapRuntime = (function () {
    const state = window.GeoArtist.state;
    const events = window.GeoArtist.events;
    const geoJson = window.GeoArtist.geoJson;

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

        if (state.maps[mapId]) {
            return state.maps[mapId];
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
        return L.geoJSON(item, {
            style: {
                color: options.polygonColor ?? "#3388ff",
                opacity: options.polygonOpacity ?? 0.4
            }
        });
    }

    function extendBoundsFromLayer(currentBounds, layer) {
        if (!layer) {
            return currentBounds;
        }

        if (typeof layer.getBounds === "function") {
            const bounds = layer.getBounds();

            if (bounds && bounds.isValid && bounds.isValid()) {
                if (currentBounds === null) {
                    return bounds;
                }

                currentBounds.extend(bounds);
                return currentBounds;
            }
        }

        if (typeof layer.getLatLng === "function") {
            const latLng = layer.getLatLng();

            if (latLng) {
                const pointBounds = L.latLngBounds([latLng, latLng]);

                if (currentBounds === null) {
                    return pointBounds;
                }

                currentBounds.extend(pointBounds);
            }
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

        const options = payload.mapOptions;
        const mapId = options.mapId;
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

    return {
        ensureMap,
        clearShapes,
        buildGeoJsonLayer,
        extendBoundsFromLayer,
        renderGeoItems,
        renderMapFromPayload
    };
})();