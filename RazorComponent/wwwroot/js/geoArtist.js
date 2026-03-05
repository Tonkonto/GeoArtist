window.geoModule = {
    renderMap: function (mapId, geoJson, minX, minY, maxX, maxY) {
        var map = L.map(mapId);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
        }).addTo(map);

        var geo = JSON.parse(geoJson);
        var layer = L.geoJSON(geo).addTo(map);

        // автоцентрирование по bounding box
        map.fitBounds([[minY, minX], [maxY, maxX]]);
    }
};