using System.ComponentModel.DataAnnotations;
using GeoComponent.Abstractions;
using GeoComponent.Contracts;

namespace GeoComponent.Core.Services;

/// <summary>
/// Default implementation of <see cref="IGeoComponent"/> that validates options, normalizes GeoJSON, and delegates HTML generation to <see cref="IGeoRendererBridge"/>.
/// </summary>
public sealed class GeoComponentService(IGeoRendererBridge rendererBridge, GeoJsonValidationService geoJsonValidationService) : IGeoComponent
{
    private readonly IGeoRendererBridge _rendererBridge = rendererBridge;
    private readonly GeoJsonValidationService _geoJsonValidationService = geoJsonValidationService;

    /// <inheritdoc />
    public GeoRenderResult RenderMap(string? geoJson, GeoMapOptions? mapOptions = null)
    {
        var payload = BuildPayload(
            mode: "map",
            geoJson: geoJson,
            mapOptions: mapOptions,
            editorOptions: null);

        return _rendererBridge.Render(payload);
    }

    /// <inheritdoc />
    public GeoRenderResult RenderEditor(string? geoJson, GeoMapOptions? mapOptions = null, GeoEditorOptions? editorOptions = null)
    {
        var resolvedEditorOptions = editorOptions ?? new GeoEditorOptions();

        var payload = BuildPayload(
            mode: "editor",
            geoJson: geoJson,
            mapOptions: mapOptions,
            editorOptions: resolvedEditorOptions);

        return _rendererBridge.Render(payload);
    }

    private GeoComponentPayload BuildPayload(string mode, string? geoJson, GeoMapOptions? mapOptions, GeoEditorOptions? editorOptions)
    {
        var resolvedMapOptions = mapOptions ?? new GeoMapOptions();

        if (string.IsNullOrWhiteSpace(resolvedMapOptions.MapId))
            resolvedMapOptions.MapId = "geoartist-map";

        ValidateOptions(resolvedMapOptions);

        if (editorOptions is not null)
            ValidateOptions(editorOptions);

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

    private static void ValidateOptions(object options)
    {
        Validator.ValidateObject(
            options,
            new ValidationContext(options),
            validateAllProperties: true);
    }
}
