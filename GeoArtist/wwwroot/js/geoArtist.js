window.GeoArtist = window.GeoArtist || {};

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

function disposeEditorIfAny(payload) {
    const mapId = payload?.mapOptions?.mapId ?? payload?.mapId;

    if (!mapId) {
        return;
    }

    const editorRuntime = window.GeoArtist.editorRuntime;
    invokeIfFunction(editorRuntime, "disposeEditor", [mapId]);
}

window.GeoArtist.bootstrap = function (payload) {
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
            disposeEditorIfAny(payload);
            return window.GeoArtist.mapRuntime.renderMapFromPayload(payload);

        case "editor":
            return window.GeoArtist.editorRuntime.renderEditorFromPayload(payload);

        default:
            console.error("GeoArtist.bootstrap: unsupported mode:", payload.mode);
            return null;
    }
};

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

window.GeoArtist.getMap = function (mapId) {
    return window.GeoArtist.state.getMap(mapId);
};

window.GeoArtist.getLayer = function (mapId) {
    return window.GeoArtist.state.getLayer(mapId);
};

window.GeoArtist.getEditor = function (mapId) {
    return window.GeoArtist.state.getEditor(mapId);
};

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
