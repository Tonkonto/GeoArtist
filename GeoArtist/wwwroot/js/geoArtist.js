window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.ensureMap = function (options) {
    return window.GeoArtist.mapRuntime.ensureMap(options);
};

window.GeoArtist.clearShapes = function (mapId) {
    return window.GeoArtist.mapRuntime.clearShapes(mapId);
};

window.GeoArtist.renderMapFromPayload = function (payload) {
    return window.GeoArtist.mapRuntime.renderMapFromPayload(payload);
};

window.GeoArtist.renderEditorFromPayload = function (payload) {
    return window.GeoArtist.editorRuntime.renderEditorFromPayload(payload);
};

window.GeoArtist.renderMap = function (geoJson, options) {
    return window.GeoArtist.renderMapFromPayload({
        mode: "map",
        mapOptions: options,
        geoJson: geoJson
    });
};

window.GeoArtist.renderEditor = function (geoJson, mapOptions, editorOptions) {
    return window.GeoArtist.renderEditorFromPayload({
        mode: "editor",
        mapOptions: mapOptions,
        editorOptions: editorOptions,
        geoJson: geoJson
    });
};

window.GeoArtist.normalizeGeoJsonInput = function (geoJson) {
    return window.GeoArtist.geoJson.normalizeGeoJsonInput(geoJson);
};

window.GeoArtist.toEditableJsonText = function (geoJson) {
    return window.GeoArtist.geoJson.toEditableJsonText(geoJson);
};

window.GeoArtist.tryParseEditorText = function (text) {
    return window.GeoArtist.geoJson.tryParseEditorText(text);
};

window.GeoArtist.getCircleExportVertexCount = function () {
    return window.GeoArtist.geoJson.getCircleExportVertexCount();
};

window.GeoArtist.setCircleExportVertexCount = function (count) {
    return window.GeoArtist.geoJson.setCircleExportVertexCount(count);
};

window.GeoArtist.getMap = function (mapId) {
    return window.GeoArtist.state.getMap(mapId);
};

window.GeoArtist.getLayer = function (mapId) {
    return window.GeoArtist.state.getLayer(mapId);
};

window.GeoArtist.getEditor = function (mapId) {
    return window.GeoArtist.state.getEditor(mapId);
};

window.GeoArtist.exportDisplayedGeoJson = function (mapId) {
    const editorState = window.GeoArtist.state.getEditor(mapId);

    if (editorState) {
        return window.GeoArtist.geoJson.exportEditorGeoItems(editorState);
    }

    return window.GeoArtist.mapRuntime.exportLayerFeatureCollection(mapId);
};

window.GeoArtist.ExportDisplayedGeoJson = window.GeoArtist.exportDisplayedGeoJson;

Object.defineProperty(window.GeoArtist, "maps", {
    get: function () {
        return window.GeoArtist.state.maps;
    }
});

Object.defineProperty(window.GeoArtist, "layers", {
    get: function () {
        return window.GeoArtist.state.layers;
    }
});

Object.defineProperty(window.GeoArtist, "editors", {
    get: function () {
        return window.GeoArtist.state.editors;
    }
});
