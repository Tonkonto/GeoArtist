window.geoModule = {
    renderMap: function (mapId, geoArray, polygonColor, polygonOpacity) {
        const mapElement = document.getElementById(mapId);
        if (!mapElement) return;

        const map = L.map(mapId);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        let bounds = null;

        geoArray.forEach(g => {
            const geo = JSON.parse(g.geoJson);

            const layer = L.geoJSON(geo, {
                style: {
                    color: polygonColor,
                    opacity: polygonOpacity
                }
            }).addTo(map);

            const layerBounds = layer.getBounds();
            if (layerBounds.isValid()) {
                if (bounds === null) {
                    bounds = layerBounds;
                } else {
                    bounds.extend(layerBounds);
                }
            }
        });

        if (bounds && bounds.isValid()) {
            map.fitBounds(bounds);
        } else {
            map.setView([42.87, 74.60], 12);
        }
    }
};