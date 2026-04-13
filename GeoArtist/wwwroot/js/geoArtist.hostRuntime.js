window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.hostRuntime = (function () {
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

        invokeIfFunction(window.GeoArtist.editorRuntime, "disposeEditor", [mapId]);
    }

    function clonePayload(payload) {
        return JSON.parse(JSON.stringify(payload));
    }

    function shallowMerge(base, patch) {
        if (!patch) {
            return base ? { ...base } : {};
        }

        return {
            ...(base ?? {}),
            ...patch
        };
    }

    function normalizePayload(payload) {
        if (!payload) {
            return null;
        }

        if (!payload.mapOptions) {
            console.error("GeoArtist: payload.mapOptions is required.");
            return null;
        }

        if (!payload.mapOptions.mapId && payload.mapId) {
            payload.mapOptions.mapId = payload.mapId;
        }

        if (!payload.mapOptions.mapId) {
            console.error("GeoArtist: mapOptions.mapId is required.");
            return null;
        }

        return payload;
    }

    function applyBootstrapPayload(payload) {
        const normalized = normalizePayload(payload);

        if (!normalized) {
            return null;
        }

        switch ((normalized.mode ?? "map").toLowerCase()) {
            case "map":
                disposeEditorIfAny(normalized);
                return window.GeoArtist.mapRuntime.renderMapFromPayload(normalized);

            case "editor":
                return window.GeoArtist.editorRuntime.renderEditorFromPayload(normalized);

            default:
                console.error("GeoArtist: unsupported mode:", normalized.mode);
                return null;
        }
    }

    const instances = new Map();
    const registrationOrder = [];

    function rememberInstance(mapId) {
        if (!instances.has(mapId)) {
            registrationOrder.push(mapId);
        }
    }

    function forgetInstance(mapId) {
        instances.delete(mapId);
        const index = registrationOrder.indexOf(mapId);

        if (index >= 0) {
            registrationOrder.splice(index, 1);
        }
    }

    function resolveTargetMapIds(mapId) {
        if (mapId) {
            return instances.has(mapId) ? [mapId] : [];
        }

        return registrationOrder.filter(function (id) {
            return instances.has(id);
        });
    }

    function getEntry(mapId) {
        return instances.get(mapId);
    }

    function initialize(payload) {
        if (!payload) {
            console.error("GeoArtist.initialize: payload is required.");
            return null;
        }

        const working = clonePayload(payload);
        const normalized = normalizePayload(working);

        if (!normalized) {
            return null;
        }

        const snapshot = clonePayload(normalized);
        const mapId = snapshot.mapOptions.mapId;
        const mode = (snapshot.mode ?? "map").toLowerCase();

        rememberInstance(mapId);
        instances.set(mapId, {
            mode,
            lastPayload: snapshot
        });

        return applyBootstrapPayload(snapshot);
    }

    function disposeInstance(mapId) {
        if (!mapId) {
            return;
        }

        const entry = getEntry(mapId);

        if (entry && entry.mode === "editor") {
            invokeIfFunction(window.GeoArtist.editorRuntime, "disposeEditor", [mapId]);
        }

        forgetInstance(mapId);
    }

    function updateGeoJson(geoJson, mapId) {
        const targets = resolveTargetMapIds(mapId);

        if (!targets.length) {
            console.warn("GeoArtist.updateGeoJson: no managed map instances. Call GeoArtist.initialize first.");
            return;
        }

        for (const id of targets) {
            const entry = getEntry(id);

            if (!entry) {
                continue;
            }

            const next = clonePayload(entry.lastPayload);
            next.geoJson = geoJson;
            entry.lastPayload = clonePayload(next);
            applyBootstrapPayload(next);
        }
    }

    function updateMapOptions(mapOptions, mapId) {
        const targets = resolveTargetMapIds(mapId);

        if (!targets.length) {
            console.warn("GeoArtist.updateMapOptions: no managed map instances. Call GeoArtist.initialize first.");
            return;
        }

        for (const id of targets) {
            const entry = getEntry(id);

            if (!entry) {
                continue;
            }

            const next = clonePayload(entry.lastPayload);
            next.mapOptions = shallowMerge(next.mapOptions, mapOptions);
            next.mapOptions.mapId = id;
            entry.lastPayload = clonePayload(next);
            applyBootstrapPayload(next);
        }
    }

    function updateEditorOptions(editorOptions, mapId) {
        const targets = resolveTargetMapIds(mapId);

        if (!targets.length) {
            console.warn("GeoArtist.updateEditorOptions: no managed map instances. Call GeoArtist.initialize first.");
            return;
        }

        for (const id of targets) {
            const entry = getEntry(id);

            if (!entry || entry.mode !== "editor") {
                continue;
            }

            const next = clonePayload(entry.lastPayload);
            next.editorOptions = shallowMerge(next.editorOptions, editorOptions);
            entry.lastPayload = clonePayload(next);
            applyBootstrapPayload(next);
        }
    }

    function updateHostOptions(mapOptions, editorOptions, mapId) {
        updateMapOptions(mapOptions ?? {}, mapId);
        updateEditorOptions(editorOptions ?? {}, mapId);
    }

    function clearGeoJson(mapId) {
        const empty = {
            type: "FeatureCollection",
            features: []
        };

        updateGeoJson(empty, mapId);
    }

    function getManagedMapIds() {
        return registrationOrder.filter(function (id) {
            return instances.has(id);
        });
    }

    function bootstrap(payload) {
        return initialize(payload);
    }

    function install() {
        window.GeoArtist.initialize = initialize;
        window.GeoArtist.disposeInstance = disposeInstance;
        window.GeoArtist.updateGeoJson = updateGeoJson;
        window.GeoArtist.updateMapOptions = updateMapOptions;
        window.GeoArtist.updateEditorOptions = updateEditorOptions;
        window.GeoArtist.updateHostOptions = updateHostOptions;
        window.GeoArtist.clearGeoJson = clearGeoJson;
        window.GeoArtist.getManagedMapIds = getManagedMapIds;

        window.GeoArtist.UpdateGeoJson = updateGeoJson;
        window.GeoArtist.UpdateMapOptions = updateMapOptions;
        window.GeoArtist.UpdateEditorOptions = updateEditorOptions;
        window.GeoArtist.UpdateHostOptions = updateHostOptions;
        window.GeoArtist.ClearGeoJson = clearGeoJson;
        window.GeoArtist.Initialize = initialize;

        window.GeoArtist.bootstrap = bootstrap;
    }

    return {
        install
    };
})();

window.GeoArtist.hostRuntime.install();
