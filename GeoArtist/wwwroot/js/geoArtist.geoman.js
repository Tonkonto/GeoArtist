window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.geoman = (function () {
    const events = window.GeoArtist.events;

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

    function resolveNodeSize(editorOptions) {
        return editorOptions.nodeSize;
    }

    function resolveNodeColor(editorOptions, mapOptions) {
        return editorOptions?.nodeColor ?? editorOptions?.drawColor ?? mapOptions?.polygonColor ?? undefined;
    }

    function resolveDrawStyle(editorOptions, mapOptions) {
        const color = editorOptions?.drawColor ?? mapOptions?.polygonColor;
        const opacity = editorOptions?.drawOpacity ?? mapOptions?.polygonOpacity;

        return {
            color,
            opacity
        };
    }

    function resolveMapStyle(mapOptions) {
        return {
            color: mapOptions?.polygonColor,
            opacity: mapOptions?.polygonOpacity
        };
    }

    function buildPathOptions(drawStyle) {
        const color = drawStyle.color;
        const opacity = drawStyle.opacity;

        return {
            color,
            fillColor: color,
            opacity,
            fillOpacity: opacity
        };
    }

    function buildDrawPreviewOptions(drawStyle) {
        const color = drawStyle.color;
        const opacity = drawStyle.opacity;

        return {
            templineStyle: {
                color,
                opacity,
                fillColor: color,
                fillOpacity: opacity
            },
            hintlineStyle: {
                color,
                opacity,
                dashArray: [5, 5]
            }
        };
    }

    function applyLayerStyle(layer, pathOptions) {
        if (!layer || !pathOptions) {
            return;
        }

        if (typeof layer.setStyle === "function") {
            layer.setStyle(pathOptions);
            return;
        }

        if (typeof layer.eachLayer === "function") {
            layer.eachLayer(function (innerLayer) {
                applyLayerStyle(innerLayer, pathOptions);
            });
        }
    }

    function applyMapStyleToEditorLayers(editorState, mapPathOptions) {
        const featureGroup = editorState?.featureGroup;

        if (!featureGroup || typeof featureGroup.eachLayer !== "function") {
            return;
        }

        featureGroup.eachLayer(function (layer) {
            applyLayerStyle(layer, mapPathOptions);
        });
    }

    function getNodeStyleElementId(mapId) {
        return `geoartist-node-size-${mapId}`;
    }

    function getUiStyleElementId(mapId) {
        return `geoartist-ui-style-${mapId}`;
    }

    function escapeCssIdentifier(value) {
        if (window.CSS && typeof window.CSS.escape === "function") {
            return window.CSS.escape(value);
        }

        return String(value).replace(/[^a-zA-Z0-9_-]/g, "\\$&");
    }

    function resolveScaleValue(value, fallback) {
        if (typeof value === "number" && Number.isFinite(value) && value > 0) {
            return value;
        }

        return fallback;
    }

    function resolveUiScaleConfig(editorOptions) {
        const baseScale = resolveScaleValue(editorOptions?.uiScale, 1);
        const toolbarScale = baseScale * resolveScaleValue(editorOptions?.toolbarScale, 1);
        const actionsScale = baseScale * resolveScaleValue(editorOptions?.actionsScale, 1);
        const tooltipScale = baseScale * resolveScaleValue(editorOptions?.tooltipScale, 1);

        return {
            toolbarScale,
            actionsScale,
            tooltipScale,
            actionsVertical: editorOptions?.actionsVertical === true
        };
    }

    function applyNodeSize(editorState, nodeSize, nodeColor) {
        const mapId = editorState?.mapId;

        if (!mapId) {
            return;
        }

        const middleSize = Math.max(1, Math.round(nodeSize * 0.72));
        const half = Math.round(nodeSize / 2);
        const middleHalf = Math.round(middleSize / 2);
        const styleId = getNodeStyleElementId(mapId);
        const escapedMapId = escapeCssIdentifier(mapId);
        const colorCss = nodeColor
            ? `background:${nodeColor};border-color:${nodeColor};`
            : "";

        let styleElement = document.getElementById(styleId);

        if (!styleElement) {
            styleElement = document.createElement("style");
            styleElement.id = styleId;
            document.head.appendChild(styleElement);
        }

        // !important is needed since geoman itself uses this hack
        styleElement.textContent =
            `#${escapedMapId} .leaflet-pane .marker-icon{width:${nodeSize}px!important;height:${nodeSize}px!important;margin:-${half}px 0 0 -${half}px!important;${colorCss}}` +
            `#${escapedMapId} .leaflet-pane .marker-icon-middle{width:${middleSize}px!important;height:${middleSize}px!important;margin:-${middleHalf}px 0 0 -${middleHalf}px!important;${colorCss}}`;
    }

    function clearNodeSize(editorState) {
        const mapId = editorState?.mapId;

        if (!mapId) {
            return;
        }

        const styleElement = document.getElementById(getNodeStyleElementId(mapId));

        if (styleElement) {
            styleElement.remove();
        }
    }

    function applyUiScale(editorState, uiScaleConfig) {
        const mapId = editorState?.mapId;

        if (!mapId || !uiScaleConfig) {
            return;
        }

        const styleId = getUiStyleElementId(mapId);
        const escapedMapId = escapeCssIdentifier(mapId);
        const actionsDirection = uiScaleConfig.actionsVertical ? "column" : "row";
        const tooltipFontSize = 12 * uiScaleConfig.tooltipScale;
        const tooltipPaddingVertical = 4 * uiScaleConfig.tooltipScale;
        const tooltipPaddingHorizontal = 8 * uiScaleConfig.tooltipScale;

        let styleElement = document.getElementById(styleId);

        if (!styleElement) {
            styleElement = document.createElement("style");
            styleElement.id = styleId;
            document.head.appendChild(styleElement);
        }

        styleElement.textContent =
            `#${escapedMapId} .leaflet-pm-toolbar{zoom:${uiScaleConfig.toolbarScale};width:max-content;}` +
            `#${escapedMapId} .leaflet-pm-actions-container{zoom:${uiScaleConfig.actionsScale};width:max-content;max-width:none;}` +
            `#${escapedMapId} .button-container.active .leaflet-pm-actions-container{display:inline-flex;flex-direction:${actionsDirection};}` +
            `#${escapedMapId} .leaflet-pm-actions-container .leaflet-pm-action{white-space:nowrap;}` +
            `#${escapedMapId} .leaflet-tooltip{font-size:${tooltipFontSize}px;padding:${tooltipPaddingVertical}px ${tooltipPaddingHorizontal}px;line-height:1.2;max-width:320px;}`;
    }

    function clearUiScale(editorState) {
        const mapId = editorState?.mapId;

        if (!mapId) {
            return;
        }

        const styleElement = document.getElementById(getUiStyleElementId(mapId));

        if (styleElement) {
            styleElement.remove();
        }
    }

    function resolveDragClickTolerance(editorOptions) {
        const value = editorOptions?.dragClickTolerance ?? editorOptions?.DragClickTolerance;

        if (typeof value !== "number" || !Number.isFinite(value)) {
            return null;
        }

        return Math.max(0, Math.floor(value));
    }

    function applyDragClickTolerance(editorState, editorOptions) {
        const map = editorState?.map;
        const draggable = map?.dragging?._draggable;

        if (!draggable || !draggable.options) {
            return;
        }

        if (!Object.prototype.hasOwnProperty.call(editorState, "originalDragClickTolerance")) {
            editorState.originalDragClickTolerance = draggable.options.clickTolerance;
        }

        const tolerance = resolveDragClickTolerance(editorOptions);

        if (tolerance === null) {
            return;
        }

        draggable.options.clickTolerance = tolerance;
    }

    function restoreDragClickTolerance(editorState) {
        const map = editorState?.map;
        const draggable = map?.dragging?._draggable;
        const originalTolerance = editorState?.originalDragClickTolerance;

        if (!draggable || !draggable.options) {
            return;
        }

        if (typeof originalTolerance !== "number" || !Number.isFinite(originalTolerance)) {
            return;
        }

        draggable.options.clickTolerance = originalTolerance;
    }

    function detachPmEventHandlers(editorState) {
        const map = editorState?.map;

        if (!map || typeof map.off !== "function") {
            return;
        }

        if (typeof editorState.pmCreateHandler === "function") {
            map.off("pm:create", editorState.pmCreateHandler);
        }

        if (typeof editorState.pmEditHandler === "function") {
            map.off("pm:edit", editorState.pmEditHandler);
        }

        if (typeof editorState.pmRemoveHandler === "function") {
            map.off("pm:remove", editorState.pmRemoveHandler);
        }

        if (typeof editorState.pmCutHandler === "function") {
            map.off("pm:cut", editorState.pmCutHandler);
        }

        editorState.pmCreateHandler = null;
        editorState.pmEditHandler = null;
        editorState.pmRemoveHandler = null;
        editorState.pmCutHandler = null;
    }

    function disableActiveGlobalModes(mapPm) {
        const modePairs = [
            ["globalDrawModeEnabled", "disableGlobalDrawMode"],
            ["globalEditModeEnabled", "disableGlobalEditMode"],
            ["globalDragModeEnabled", "disableGlobalDragMode"],
            ["globalRemovalModeEnabled", "disableGlobalRemovalMode"],
            ["globalCutModeEnabled", "disableGlobalCutMode"],
            ["globalRotateModeEnabled", "disableGlobalRotateMode"]
        ];

        modePairs.forEach(function (pair) {
            const isEnabledMethod = pair[0];
            const disableMethod = pair[1];
            const isEnabled = invokeIfFunction(mapPm, isEnabledMethod);

            if (isEnabled === true) {
                invokeIfFunction(mapPm, disableMethod);
            }
        });
    }

    function enableGeoman(editorState) {
        if (!editorState || !editorState.map) {
            return;
        }

        const map = editorState.map;
        const editorOptions = editorState.editorOptions ?? {};
        const mapOptions = editorState.options ?? {};
        const mapStyle = resolveMapStyle(mapOptions);
        const drawStyle = resolveDrawStyle(editorOptions, mapOptions);
        const mapPathOptions = buildPathOptions(mapStyle);
        const drawPathOptions = buildPathOptions(drawStyle);
        const drawPreviewOptions = buildDrawPreviewOptions(drawStyle);
        const uiScaleConfig = resolveUiScaleConfig(editorOptions);
        const nodeColor = resolveNodeColor(editorOptions, mapOptions);

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
            drawText: editorOptions.allowText === true,
            editMode: editorOptions.allowEdit !== false,
            dragMode: editorOptions.allowDrag === true,
            cutPolygon: editorOptions.allowCut === true,
            removalMode: editorOptions.allowDelete !== false,
            rotateMode: editorOptions.allowRotate === true
        });

        map.pm.setGlobalOptions({
            layerGroup: editorState.featureGroup,
            snapDistance: editorOptions.snapSensitivity,
            pathOptions: drawPathOptions,
            templineStyle: drawPreviewOptions.templineStyle,
            hintlineStyle: drawPreviewOptions.hintlineStyle
        });

        applyNodeSize(editorState, resolveNodeSize(editorOptions), nodeColor);
        applyUiScale(editorState, uiScaleConfig);
        applyDragClickTolerance(editorState, editorOptions);

        editorState.pmCreateHandler = function (e) {
            if (e && e.layer && !editorState.featureGroup.hasLayer(e.layer)) {
                editorState.featureGroup.addLayer(e.layer);
            }

            applyLayerStyle(e?.layer, mapPathOptions);
            applyMapStyleToEditorLayers(editorState, mapPathOptions);

            editorState.syncFromLayers("draw");

            events.emit("geoartist:featureCreated", {
                mapId: editorState.mapId,
                geoJson: editorState.payload.geoJson
            });
        };

        editorState.pmEditHandler = function () {
            applyMapStyleToEditorLayers(editorState, mapPathOptions);
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
            applyMapStyleToEditorLayers(editorState, mapPathOptions);
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

        detachPmEventHandlers(editorState);
        clearNodeSize(editorState);
        clearUiScale(editorState);
        restoreDragClickTolerance(editorState);

        if (!map.pm || editorState.geomanInitialized !== true) {
            editorState.geomanInitialized = false;
            return;
        }

        try {
            disableActiveGlobalModes(map.pm);
            invokeIfFunction(map.pm, "removeControls");
            editorState.geomanInitialized = false;
        } catch (error) {
            console.error("GeoArtist.disableGeoman: failed to disable geoman controls.", error);
        }
    }

    return {
        enableGeoman,
        disableGeoman
    };
})();
