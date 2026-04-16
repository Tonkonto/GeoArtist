window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.state = (function () {
    const maps = {};
    const layers = {};
    const editors = {};

    function getMap(mapId) {
        return maps[mapId] ?? null;
    }

    function getLayer(mapId) {
        return layers[mapId] ?? null;
    }

    function getEditor(mapId) {
        return editors[mapId] ?? null;
    }

    return {
        maps,
        layers,
        editors,
        getMap,
        getLayer,
        getEditor
    };
})();