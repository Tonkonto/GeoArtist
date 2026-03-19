using GeoComponent.Abstractions;
using GeoComponent.Contracts;

namespace GeoComponent.Core.Services;

public sealed class GeoComponentService(IGeoRendererBridge rendererBridge, GeoJsonValidationService geoJsonValidationService) : IGeoComponent
{
    private readonly IGeoRendererBridge _rendererBridge = rendererBridge;
    private readonly GeoJsonValidationService _geoJsonValidationService = geoJsonValidationService;

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

    private GeoComponentPayload BuildPayload(string mode, string? geoJson, GeoMapOptions? mapOptions, GeoEditorOptions? editorOptions)
    {
        var resolvedMapOptions = mapOptions ?? new GeoMapOptions();

        if (string.IsNullOrWhiteSpace(resolvedMapOptions.MapId))
            resolvedMapOptions.MapId = "geoartist-map";

        var normalizedGeoJson = _geoJsonValidationService.NormalizeForRender(
            geoJson,
            resolvedMapOptions.SourceSrid);

        return new GeoComponentPayload
        {
            Mode = mode,
            MapId = resolvedMapOptions.MapId,
            GeoJson = normalizedGeoJson,
            MapOptions = resolvedMapOptions,
            EditorOptions = editorOptions
        };
    }
}
