window.GeoArtist = (function () {
    const maps = {};
    const layers = {};
    const editors = {};

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

        if (maps[mapId]) {
            return maps[mapId];
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

        maps[mapId] = map;
        layers[mapId] = null;

        return map;
    }

    function clearShapes(mapId) {
        const map = maps[mapId];
        const layer = layers[mapId];

        if (!map || !layer) {
            return;
        }

        map.removeLayer(layer);
        layers[mapId] = null;
    }

    function normalizeGeoJsonInput(geoJson) {
        if (!geoJson) {
            return [];
        }

        if (Array.isArray(geoJson)) {
            return geoJson;
        }

        if (typeof geoJson === "string") {
            try {
                const parsed = JSON.parse(geoJson);

                if (Array.isArray(parsed)) {
                    return parsed;
                }

                return [parsed];
            } catch (error) {
                console.error("GeoArtist.normalizeGeoJsonInput: invalid geoJson string.", error);
                return [];
            }
        }

        if (typeof geoJson === "object") {
            return [geoJson];
        }

        return [];
    }

    function toEditableJsonText(geoJson) {
        if (!geoJson) {
            return "";
        }

        if (typeof geoJson === "string") {
            try {
                const parsed = JSON.parse(geoJson);
                return JSON.stringify(parsed, null, 2);
            } catch {
                return geoJson;
            }
        }

        try {
            return JSON.stringify(geoJson, null, 2);
        } catch {
            return "";
        }
    }

    function tryParseEditorText(text) {
        if (!text || !text.trim()) {
            return { ok: true, value: [] };
        }

        try {
            const parsed = JSON.parse(text);

            if (Array.isArray(parsed)) {
                return { ok: true, value: parsed };
            }

            return { ok: true, value: [parsed] };
        } catch (error) {
            return { ok: false, error };
        }
    }

    function buildGeoJsonLayer(item, options) {
        return L.geoJSON(item, {
            style: {
                color: options.polygonColor ?? "#3388ff",
                opacity: options.polygonOpacity ?? 0.4
            }
        });
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

            if (typeof layer.getBounds === "function") {
                const bounds = layer.getBounds();

                if (bounds && bounds.isValid && bounds.isValid()) {
                    if (combinedBounds === null) {
                        combinedBounds = bounds;
                    } else {
                        combinedBounds.extend(bounds);
                    }
                }
            }
        }

        const group = L.featureGroup(featureLayers).addTo(map);
        layers[mapId] = group;

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
        const geoItems = normalizeGeoJsonInput(payload.geoJson);

        return renderGeoItems(mapId, geoItems, options);
    }

    function getEditorOutputElement(mapId) {
        return document.getElementById(`${mapId}-geojson-output`);
    }

    function setEditorStatus(outputElement, isInvalid) {
        if (!outputElement) {
            return;
        }

        if (isInvalid) {
            outputElement.setAttribute("data-geoartist-invalid", "true");
        } else {
            outputElement.removeAttribute("data-geoartist-invalid");
        }
    }

    function disposeEditor(mapId) {
        const editor = editors[mapId];

        if (!editor) {
            return;
        }

        if (editor.outputElement && editor.inputHandler) {
            editor.outputElement.removeEventListener("input", editor.inputHandler);
        }

        delete editors[mapId];
    }

    function initializeEditorState(payload) {
        const options = payload.mapOptions;
        const mapId = options.mapId;
        const outputElement = getEditorOutputElement(mapId);

        if (!outputElement) {
            console.error("GeoArtist.initializeEditorState: editor output element not found:", `${mapId}-geojson-output`);
            return null;
        }

        disposeEditor(mapId);

        const editorState = {
            mapId,
            outputElement,
            payload,
            lastValidGeoItems: normalizeGeoJsonInput(payload.geoJson)
        };

        outputElement.value = toEditableJsonText(payload.geoJson);
        setEditorStatus(outputElement, false);

        editorState.inputHandler = function () {
            const parseResult = tryParseEditorText(outputElement.value);

            if (!parseResult.ok) {
                setEditorStatus(outputElement, true);
                return;
            }

            setEditorStatus(outputElement, false);

            editorState.lastValidGeoItems = parseResult.value;
            payload.geoJson = parseResult.value;

            renderGeoItems(mapId, parseResult.value, options);
        };

        outputElement.addEventListener("input", editorState.inputHandler);
        editors[mapId] = editorState;

        return editorState;
    }

    function renderEditorFromPayload(payload) {
        if (!payload || !payload.mapOptions) {
            console.error("GeoArtist.renderEditorFromPayload: payload.mapOptions is required.");
            return null;
        }

        const options = payload.mapOptions;
        const mapId = options.mapId;

        const editorState = initializeEditorState(payload);
        const initialGeoItems = editorState?.lastValidGeoItems ?? normalizeGeoJsonInput(payload.geoJson);

        return renderGeoItems(mapId, initialGeoItems, options);
    }

    function getMap(mapId) {
        return maps[mapId] ?? null;
    }

    function getLayer(mapId) {
        return layers[mapId] ?? null;
    }

    function getEditor(mapId) {
        return editors[mapId] ?? null;
    }

    function bootstrap(payload) {
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
                return renderMapFromPayload(payload);

            case "editor":
                return renderEditorFromPayload(payload);

            default:
                console.error("GeoArtist.bootstrap: unsupported mode:", payload.mode);
                return null;
        }
    }

    return {
        bootstrap,
        ensureMap,
        clearShapes,
        renderMapFromPayload,
        renderEditorFromPayload,
        normalizeGeoJsonInput,
        toEditableJsonText,
        tryParseEditorText,
        getMap,
        getLayer,
        getEditor,
        maps,
        layers,
        editors
    };
})();