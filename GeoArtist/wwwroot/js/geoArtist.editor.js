window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.editorRuntime = (function () {
    const state = window.GeoArtist.state;
    const events = window.GeoArtist.events;
    const geoJson = window.GeoArtist.geoJson;
    const mapRuntime = window.GeoArtist.mapRuntime;
    const geoman = window.GeoArtist.geoman;

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
        const editor = state.editors[mapId];

        if (!editor) {
            return;
        }

        if (editor.outputElement && editor.inputHandler) {
            editor.outputElement.removeEventListener("input", editor.inputHandler);
        }

        geoman.disableGeoman(editor);

        if (editor.featureGroup) {
            try {
                editor.featureGroup.clearLayers();

                if (editor.map && editor.map.hasLayer(editor.featureGroup)) {
                    editor.map.removeLayer(editor.featureGroup);
                }
            } catch (error) {
                console.error("GeoArtist.disposeEditor: failed to dispose feature group.", error);
            }
        }

        delete state.editors[mapId];
    }

    function importGeoItemsToEditor(editorState, geoJsonInput, fitBounds) {
        if (!editorState || !editorState.featureGroup) {
            return;
        }

        editorState.featureGroup.clearLayers();

        const items = geoJson.expandGeoJsonItems(geoJsonInput);
        let combinedBounds = null;

        for (const item of items) {
            if (!item) {
                continue;
            }

            let geoJsonLayer;

            try {
                geoJsonLayer = mapRuntime.buildGeoJsonLayer(item, editorState.options);
            } catch (error) {
                console.error("GeoArtist.importGeoItemsToEditor: failed to build layer.", error, item);
                continue;
            }

            geoJsonLayer.eachLayer(function (innerLayer) {
                geoJson.preserveFeatureMetadata(innerLayer, item);
                editorState.featureGroup.addLayer(innerLayer);
                combinedBounds = mapRuntime.extendBoundsFromLayer(combinedBounds, innerLayer);
            });
        }

        if ((fitBounds ?? true) && (editorState.options.fitBounds ?? true) && combinedBounds && combinedBounds.isValid()) {
            editorState.map.fitBounds(combinedBounds);
        }
    }

    function syncEditorOutputFromLayers(editorState, source) {
        if (!editorState) {
            return;
        }

        const exportedGeoJson = geoJson.exportEditorGeoItems(editorState);

        editorState.lastValidGeoJson = exportedGeoJson;
        editorState.payload.geoJson = exportedGeoJson;

        if (editorState.outputElement) {
            editorState.suppressTextSync = true;
            editorState.outputElement.value = geoJson.toEditableJsonText(exportedGeoJson);
            setEditorStatus(editorState.outputElement, false);
            editorState.suppressTextSync = false;
        }

        events.emit("geoartist:geojsonChanged", {
            mapId: editorState.mapId,
            source: source ?? "map",
            geoJson: exportedGeoJson
        });
    }

    function initializeEditorState(payload) {
        const options = payload.mapOptions;
        const mapId = options.mapId;
        const useGeoJsonTextArea = (payload.editorOptions?.useGeoJsonTextArea) !== false;
        const outputElement = useGeoJsonTextArea ? getEditorOutputElement(mapId) : null;

        if (useGeoJsonTextArea && !outputElement) {
            console.error("GeoArtist.initializeEditorState: editor output element not found:", `${mapId}-geojson-output`);
            return null;
        }

        disposeEditor(mapId);

        const map = mapRuntime.ensureMap(options);

        if (!map) {
            return null;
        }

        mapRuntime.clearShapes(mapId);

        const featureGroup = L.featureGroup().addTo(map);

        const editorState = {
            mapId,
            map,
            outputElement,
            payload,
            options,
            editorOptions: payload.editorOptions ?? {},
            allowJsonEditing: useGeoJsonTextArea && (payload.editorOptions?.allowJsonEditing!==false),
            autoApplyJsonChanges: payload.editorOptions?.autoApplyJsonChanges!==false,
            featureGroup,
            lastValidGeoJson: payload.geoJson,
            suppressTextSync: false,
            geomanInitialized: false,
            inputHandler: null,
            pmCreateHandler: null,
            pmLayerGeometryHandler: null,
            pmRemoveHandler: null,
            pmCutHandler: null,
            syncFromLayers: null
        };

        editorState.syncFromLayers = function (source) {
            syncEditorOutputFromLayers(editorState, source);
        };

        if (outputElement) {
            outputElement.value = geoJson.toEditableJsonText(payload.geoJson);
            setEditorStatus(outputElement, false);
            outputElement.readOnly = !editorState.allowJsonEditing;

            editorState.inputHandler = function () {
                if (editorState.suppressTextSync) {
                    return;
                }

                const parseResult = geoJson.tryParseEditorText(outputElement.value);

                if (!parseResult.ok) {
                    setEditorStatus(outputElement, true);
                    return;
                }

                setEditorStatus(outputElement, false);

                editorState.lastValidGeoJson = parseResult.value;
                payload.geoJson = parseResult.value;

                if (editorState.autoApplyJsonChanges) {
                    importGeoItemsToEditor(editorState, parseResult.value, true);
                }

                events.emit("geoartist:geojsonChanged", {
                    mapId,
                    source: "text",
                    geoJson: parseResult.value
                });
            };

            if (editorState.allowJsonEditing) {
                outputElement.addEventListener("input", editorState.inputHandler);
            }
        }

        state.editors[mapId] = editorState;

        geoman.enableGeoman(editorState);

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

        if (!editorState) {
            return null;
        }

        const initialGeoJson = editorState.lastValidGeoJson ?? payload.geoJson;

        importGeoItemsToEditor(editorState, initialGeoJson, true);
        syncEditorOutputFromLayers(editorState, "init");

        events.emit("geoartist:ready", {
            mapId,
            mode: "editor"
        });

        return editorState.map;
    }

    return {
        getEditorOutputElement,
        setEditorStatus,
        disposeEditor,
        importGeoItemsToEditor,
        syncEditorOutputFromLayers,
        initializeEditorState,
        renderEditorFromPayload
    };
})();
