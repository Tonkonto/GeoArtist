window.geoComponent = {
    maps: {},
    layers: {},

    ensureMap: function (options) {
        if (!options || !options.mapId) {
            console.error("geoComponent.ensureMap: options.mapId is required.");
            return null;
        }

        const mapId = options.mapId;
        const mapElement = document.getElementById(mapId);

        if (!mapElement) {
            console.error("geoComponent.ensureMap: map element not found:", mapId);
            return null;
        }

        if (this.maps[mapId]) {
            return this.maps[mapId];
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

        this.maps[mapId] = map;
        this.layers[mapId] = null;

        return map;
    },

    renderMap: function (geoArray, options) {
        const map = this.ensureMap(options);
        if (!map) return null;

        const mapId = options.mapId;

        if (this.layers[mapId]) {
            map.removeLayer(this.layers[mapId]);
            this.layers[mapId] = null;
        }

        let combinedBounds = null;
        const featureLayers = [];

        if (Array.isArray(geoArray)) {
            geoArray.forEach(item => {
                if (!item || !item.geoJson) return;

                let parsed;
                try {
                    parsed = JSON.parse(item.geoJson);
                } catch (e) {
                    console.error("geoComponent.renderMap: invalid geoJson", e, item);
                    return;
                }

                const layer = L.geoJSON(parsed, {
                    style: {
                        color: options.polygonColor ?? "#3388ff",
                        opacity: options.polygonOpacity ?? 0.8
                    }
                });

                featureLayers.push(layer);

                const bounds = layer.getBounds();
                if (bounds && bounds.isValid()) {
                    if (combinedBounds === null) {
                        combinedBounds = bounds;
                    } else {
                        combinedBounds.extend(bounds);
                    }
                }
            });
        }

        const group = L.featureGroup(featureLayers).addTo(map);
        this.layers[mapId] = group;

        if ((options.fitBounds ?? true) && combinedBounds && combinedBounds.isValid()) {
            map.fitBounds(combinedBounds);
        }

        return map;
    },

    clearShapes: function (mapId) {
        const map = this.maps[mapId];
        const layer = this.layers[mapId];

        if (!map || !layer) return;

        map.removeLayer(layer);
        this.layers[mapId] = null;
    }
};