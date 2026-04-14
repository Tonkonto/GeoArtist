window.GeoArtist = window.GeoArtist || {};

/*
 * Ensures map instance by mapId. Creates if missing, reuses if existing.
 */
window.GeoArtist.ensureMap = function (options) {
    return window.GeoArtist.mapRuntime.ensureMap(options);
};

/*
 * Removes rendered shapes for mapId.
 * textArea is cleared by editorRuntime.syncEditorOutputFromLayers()
 */
window.GeoArtist.clearShapes = function (mapId) {
    return window.GeoArtist.mapRuntime.clearShapes(mapId);
};

/*
 * Renders map mode from full payload. Payload comes with map options and data.
 */
window.GeoArtist.renderMapFromPayload = function (payload) {
    return window.GeoArtist.mapRuntime.renderMapFromPayload(payload);
};

/*
 * Renders editor mode from full payload. Enables editing + Geoman runtime and sync.
 */
window.GeoArtist.renderEditorFromPayload = function (payload) {
    return window.GeoArtist.editorRuntime.renderEditorFromPayload(payload);
};

/*
 * Map-mode wrapper. Builds payload from geoJson, map options
 */
window.GeoArtist.renderMap = function (geoJson, options) {
    return window.GeoArtist.renderMapFromPayload({
        mode: "map",
        mapOptions: options,
        geoJson: geoJson
    });
};

/*
 * Editor-mode wrapper. Builds payload from geoJson, map options, editor options.
 */
window.GeoArtist.renderEditor = function (geoJson, mapOptions, editorOptions) {
    return window.GeoArtist.renderEditorFromPayload({
        mode: "editor",
        mapOptions: mapOptions,
        editorOptions: editorOptions,
        geoJson: geoJson
    });
};

/*
 * Normalizes GeoJSON input. Returns shapes list used by runtime.
 */
window.GeoArtist.normalizeGeoJsonInput = function (geoJson) {
    return window.GeoArtist.geoJson.normalizeGeoJsonInput(geoJson);
};

/*
 * Converts GeoJSON to formatted text for textArea control
 */
window.GeoArtist.toEditableJsonText = function (geoJson) {
    return window.GeoArtist.geoJson.toEditableJsonText(geoJson);
};

/*
 * Parses editor text to GeoJSON. Returns validation result object.
 */
window.GeoArtist.tryParseEditorText = function (text) {
    return window.GeoArtist.geoJson.tryParseEditorText(text);
};

/*
 * Returns Circle vertex count used for export (to geoJson).
 */
window.GeoArtist.getCircleExportVertexCount = function () {
    return window.GeoArtist.geoJson.getCircleExportVertexCount();
};

/*
 * Sets Circle vertex count used for export (to geoJson).
 */
window.GeoArtist.setCircleExportVertexCount = function (count) {
    return window.GeoArtist.geoJson.setCircleExportVertexCount(count);
};

/*
 * Returns managed Leaflet map by id.
 */
window.GeoArtist.getMap = function (mapId) {
    return window.GeoArtist.state.getMap(mapId);
};

/*
 * Returns rendered map layer by id.
 */
window.GeoArtist.getLayer = function (mapId) {
    return window.GeoArtist.state.getLayer(mapId);
};

/*
 * Returns editor runtime state by id.
 */
window.GeoArtist.getEditor = function (mapId) {
    return window.GeoArtist.state.getEditor(mapId);
};

/*
 * Returns current (displayed) GeoJSON as FeatureCollection object.
 * Prefers editor data when available.
 */
window.GeoArtist.exportDisplayedGeoJson = function (mapId) {
    const editorState = window.GeoArtist.state.getEditor(mapId);

    if (editorState) {
        return window.GeoArtist.geoJson.exportEditorGeoItems(editorState);
    }

    return window.GeoArtist.mapRuntime.exportLayerFeatureCollection(mapId);
};

/*
 * PascalCase alias for .NET-style calls.
 */
window.GeoArtist.ExportDisplayedGeoJson = window.GeoArtist.exportDisplayedGeoJson;

/*
 * Read-only dictionary of maps.
 */
Object.defineProperty(window.GeoArtist, "maps", {
    get: function () {
        return window.GeoArtist.state.maps;
    }
});

/*
 * Read-only dictionary of layers.
 */
Object.defineProperty(window.GeoArtist, "layers", {
    get: function () {
        return window.GeoArtist.state.layers;
    }
});

/*
 * Read-only dictionary of editors.
 */
Object.defineProperty(window.GeoArtist, "editors", {
    get: function () {
        return window.GeoArtist.state.editors;
    }
});
