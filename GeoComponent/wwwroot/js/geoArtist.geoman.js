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

    function resolveNodeSize(editorOptions) {
        return editorOptions.nodeSize;
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
                opacity
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

    function getNodeStyleElementId(mapId) {
        return `geoartist-node-size-${mapId}`;
    }

    function getPreviewStyleElementId(mapId) {
        return `geoartist-preview-style-${mapId}`;
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

    function applyNodeSize(editorState, nodeSize) {
        const mapId = editorState?.mapId;

        if (!mapId) {
            return;
        }

        const middleSize = Math.max(1, Math.round(nodeSize * 0.72));
        const half = Math.round(nodeSize / 2);
        const middleHalf = Math.round(middleSize / 2);
        const styleId = getNodeStyleElementId(mapId);
        const escapedMapId = escapeCssIdentifier(mapId);

        let styleElement = document.getElementById(styleId);

        if (!styleElement) {
            styleElement = document.createElement("style");
            styleElement.id = styleId;
            document.head.appendChild(styleElement);
        }

        styleElement.textContent =
            `#${escapedMapId} .marker-icon{width:${nodeSize}px!important;height:${nodeSize}px!important;margin:-${half}px 0 0 -${half}px!important;}` +
            `#${escapedMapId} .marker-icon-middle{width:${middleSize}px!important;height:${middleSize}px!important;margin:-${middleHalf}px 0 0 -${middleHalf}px!important;}`;
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

    function applyPreviewStyle(editorState, drawStyle) {
        const mapId = editorState?.mapId;

        if (!mapId) {
            return;
        }

        const color = drawStyle.color;
        const opacity = drawStyle.opacity;
        const styleId = getPreviewStyleElementId(mapId);
        const escapedMapId = escapeCssIdentifier(mapId);

        let styleElement = document.getElementById(styleId);

        if (!styleElement) {
            styleElement = document.createElement("style");
            styleElement.id = styleId;
            document.head.appendChild(styleElement);
        }

        styleElement.textContent =
            `#${escapedMapId} .leaflet-pm-templine,#${escapedMapId} .leaflet-pm-hintline{stroke:${color}!important;opacity:${opacity}!important;}` +
            `#${escapedMapId} .leaflet-pm-hintline{fill:${color}!important;fill-opacity:${opacity}!important;}`;
    }

    function clearPreviewStyle(editorState) {
        const mapId = editorState?.mapId;

        if (!mapId) {
            return;
        }

        const styleElement = document.getElementById(getPreviewStyleElementId(mapId));

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
            `#${escapedMapId} .leaflet-pm-actions-container{zoom:${uiScaleConfig.actionsScale};width:max-content!important;max-width:none!important;}` +
            `#${escapedMapId} .button-container.active .leaflet-pm-actions-container{display:inline-flex!important;flex-direction:${actionsDirection}!important;}` +
            `#${escapedMapId} .leaflet-pm-actions-container .leaflet-pm-action{white-space:nowrap;}` +
            `#${escapedMapId} .leaflet-tooltip{font-size:${tooltipFontSize}px!important;padding:${tooltipPaddingVertical}px ${tooltipPaddingHorizontal}px!important;line-height:1.2;max-width:320px;}`;
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

    function enableGeoman(editorState) {
        if (!editorState || !editorState.map) {
            return;
        }

        const map = editorState.map;
        const editorOptions = editorState.editorOptions ?? {};
        const mapOptions = editorState.options ?? {};
        const mapStyle = resolveMapStyle(mapOptions);
        const drawStyle = resolveDrawStyle(editorOptions, mapOptions);
        const pathOptions = buildPathOptions(mapStyle);
        const drawPreviewOptions = buildDrawPreviewOptions(drawStyle);
        const uiScaleConfig = resolveUiScaleConfig(editorOptions);

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
            layerGroup: editorState.featureGroup,
            snapDistance: editorOptions.snapSensitivity,
            pathOptions,
            templineStyle: drawPreviewOptions.templineStyle,
            hintlineStyle: drawPreviewOptions.hintlineStyle
        });
        invokeIfFunction(map.pm, "setPathOptions", [pathOptions]);

        applyNodeSize(editorState, resolveNodeSize(editorOptions));
        applyPreviewStyle(editorState, drawStyle);
        applyUiScale(editorState, uiScaleConfig);

        editorState.pmCreateHandler = function (e) {
            if (e && e.layer && !editorState.featureGroup.hasLayer(e.layer)) {
                editorState.featureGroup.addLayer(e.layer);
            }

            applyLayerStyle(e?.layer, pathOptions);

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

            clearNodeSize(editorState);
            clearPreviewStyle(editorState);
            clearUiScale(editorState);
        } catch (error) {
            console.error("GeoArtist.disableGeoman: failed to disable geoman controls.", error);
        }
    }

    return {
        enableGeoman,
        disableGeoman
    };
})();
