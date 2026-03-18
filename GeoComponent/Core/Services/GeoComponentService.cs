using GeoComponent.Abstractions;
using GeoComponent.Contracts;

namespace GeoComponent.Core.Services;

public sealed class GeoComponentService(IGeoRendererBridge rendererBridge) : IGeoComponent
{
    private readonly IGeoRendererBridge _rendererBridge = rendererBridge;

    public GeoRenderResult RenderMap(string? geoJson, GeoMapOptions? mapOptions = null)
    {
        var payload = BuildPayload(
            mode: "map",
            geoJson: geoJson,
            mapOptions: mapOptions,
            editorOptions: null);

        return _rendererBridge.Render(payload);
    }

    public GeoRenderResult RenderEditor(string? geoJson, GeoMapOptions? mapOptions = null, GeoEditorOptions? editorOptions = null)
    {
        var payload = BuildPayload(
            mode: "editor",
            geoJson: geoJson,
            mapOptions: mapOptions,
            editorOptions: editorOptions);

        return _rendererBridge.Render(payload);
    }

    private static GeoComponentPayload BuildPayload(string mode, string? geoJson, GeoMapOptions? mapOptions, GeoEditorOptions? editorOptions)
    {
        var resolvedMapOptions = mapOptions ?? new GeoMapOptions();

        if (string.IsNullOrWhiteSpace(resolvedMapOptions.MapId))
            resolvedMapOptions.MapId = "geoartist-map";    //♦todo♦ should return error instead of using hardcode

        return new GeoComponentPayload
        {
            Mode = mode,
            MapId = resolvedMapOptions.MapId,
            GeoJson = geoJson ?? "",
            MapOptions = resolvedMapOptions,
            EditorOptions = editorOptions
        };
    }
}