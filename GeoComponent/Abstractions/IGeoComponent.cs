using GeoComponent.Contracts;

namespace GeoComponent.Abstractions;

/// <summary>
/// High-level API to produce HTML and scripts for a map or editor instance.
/// </summary>
public interface IGeoComponent
{
    /// <summary>
    /// Renders a read-only Leaflet map for the given GeoJSON.
    /// </summary>
    /// <param name="geoJson">Raw GeoJSON string, or <see langword="null"/> for an empty FeatureCollection.</param>
    /// <param name="mapOptions">Layout and tile options; defaults apply when <see langword="null"/>.</param>
    /// <returns>HTML fragment, asset lists, and serialized payload for optional host integration.</returns>
    GeoRenderResult RenderMap(
        string? geoJson,
        GeoMapOptions? mapOptions = null
    );

    /// <summary>
    /// Renders an interactive editor (Leaflet + Geoman) for the given GeoJSON.
    /// </summary>
    /// <param name="geoJson">Raw GeoJSON string, or <see langword="null"/> for an empty FeatureCollection.</param>
    /// <param name="mapOptions">Layout and tile options; defaults apply when <see langword="null"/>.</param>
    /// <param name="editorOptions">Geoman and textarea behavior; defaults apply when <see langword="null"/>.</param>
    /// <returns>HTML fragment, asset lists, and serialized payload for optional host integration.</returns>
    GeoRenderResult RenderEditor(
        string? geoJson,
        GeoMapOptions? mapOptions = null,
        GeoEditorOptions? editorOptions = null
    );
}
