window.geoComponent = {
    maps: {},

    renderMap: function (geoArray, options)
    {
        if (!options || !options.mapId) {
            console.error("geoComponent.renderMap: options.mapId is required.");
            return;
        }

        const mapId = options.mapId;
        const mapElement = document.getElementById(mapId);

        if (!mapElement) {
            console.error("geoComponent.renderMap: map element not found:", mapId);
            return;
        }

        if (this.maps[mapId]) {
            this.maps[mapId].remove();
            delete this.maps[mapId];
        }

        const map = L.map(mapId).setView(
            [options.initialLat ?? 42.87, options.initialLng ?? 74.60],
            options.initialZoom ?? 12
        );

        this.maps[mapId] = map;

        if (options.showTileLayer !== false) {
            L.tileLayer(
                options.tileLayerUrl ?? 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
                {
                    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright" target="_blank" rel="noopener noreferrer">OpenStreetMap</a>'
                }
            ).addTo(map);
        }

        let combinedBounds = null;

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
                        color: options.polygonColor ?? '#3388ff',
                        opacity: options.polygonOpacity ?? 0.8
                    }
                }).addTo(map);

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

        if ((options.fitBounds ?? true) && combinedBounds && combinedBounds.isValid()) {
            map.fitBounds(combinedBounds);
        }

        return map;
    }
};