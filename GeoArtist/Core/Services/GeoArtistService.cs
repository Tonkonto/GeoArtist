using System.ComponentModel.DataAnnotations;
using GeoArtist.Abstractions;
using GeoArtist.Contracts;

namespace GeoArtist.Core.Services;

/// <summary>
/// Default implementation of <see cref="IGeoArtist"/> that validates options, normalizes GeoJSON, and delegates HTML generation to <see cref="IGeoRendererBridge"/>.
/// </summary>
public sealed class GeoArtistService(IGeoRendererBridge rendererBridge, GeoJsonValidationService geoJsonValidationService) : IGeoArtist
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

    private GeoArtistPayload BuildPayload(string mode, string? geoJson, GeoMapOptions? mapOptions, GeoEditorOptions? editorOptions)
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

        return new GeoArtistPayload
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
