window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.geoman = (function () {
    const events = window.GeoArtist.events;
    const geomanDisableMethods = [
        "disableGlobalDrawMode",
        "disableGlobalEditMode",
        "disableGlobalDragMode",
        "disableGlobalRemovalMode",
        "disableGlobalCutMode",
        "disableGlobalRotateMode",
        "removeControls"
    ];

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

    function enableGeoman(editorState) {
        if (!editorState || !editorState.map) {
            return;
        }

        const map = editorState.map;
        const editorOptions = editorState.editorOptions ?? {};

        if (editorOptions.enabled === false) {
            return;
        }

        if (!map.pm) {
            console.warn("GeoArtist.enableGeoman: Leaflet-Geoman is not loaded. Editor will stay text-driven only.");
            return;
        }

        if (editorState.geomanInitialized) {
            return;
        }

        map.pm.addControls({
            position: "topleft",
            drawMarker: editorOptions.allowMarker === true,
            drawCircleMarker: editorOptions.allowCircleMarker === true,
            drawPolyline: editorOptions.allowPolyline === true,
            drawRectangle: editorOptions.allowRectangle === true,
            drawPolygon: editorOptions.allowPolygon !== false,
            drawCircle: editorOptions.allowCircle === true,
            editMode: editorOptions.allowEdit !== false,
            dragMode: editorOptions.allowDrag === true,
            cutPolygon: editorOptions.allowCut === true,
            removalMode: editorOptions.allowDelete !== false,
            rotateMode: editorOptions.allowRotate === true
        });

        map.pm.setGlobalOptions({
            layerGroup: editorState.featureGroup
        });

        editorState.pmCreateHandler = function (e) {
            if (e && e.layer && !editorState.featureGroup.hasLayer(e.layer)) {
                editorState.featureGroup.addLayer(e.layer);
            }

            editorState.syncFromLayers("draw");

            events.emit("geoartist:featureCreated", {
                mapId: editorState.mapId,
                geoJson: editorState.payload.geoJson
            });
        };

        editorState.pmEditHandler = function () {
            editorState.syncFromLayers("edit");

            events.emit("geoartist:featureUpdated", {
                mapId: editorState.mapId,
                geoJson: editorState.payload.geoJson
            });
        };

        editorState.pmRemoveHandler = function () {
            editorState.syncFromLayers("remove");

            events.emit("geoartist:featureDeleted", {
                mapId: editorState.mapId,
                geoJson: editorState.payload.geoJson
            });
        };

        editorState.pmCutHandler = function () {
            editorState.syncFromLayers("cut");
        };

        map.on("pm:create", editorState.pmCreateHandler);
        map.on("pm:edit", editorState.pmEditHandler);
        map.on("pm:remove", editorState.pmRemoveHandler);
        map.on("pm:cut", editorState.pmCutHandler);

        editorState.geomanInitialized = true;
    }

    function disableGeoman(editorState) {
        if (!editorState || !editorState.map) {
            return;
        }

        const map = editorState.map;

        if (!map.pm) {
            return;
        }

        try {
            geomanDisableMethods.forEach(function (methodName) {
                invokeIfFunction(map.pm, methodName);
            });
        } catch (error) {
            console.error("GeoArtist.disableGeoman: failed to disable geoman controls.", error);
        }
    }

    return {
        enableGeoman,
        disableGeoman
    };
})();
