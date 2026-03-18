using GeoComponent.Contracts;

namespace GeoComponent.Abstractions;

public interface IGeoComponent
{
    GeoRenderResult RenderMap(
        string? geoJson,
        GeoMapOptions? mapOptions = null
    );

    GeoRenderResult RenderEditor(
        string? geoJson,
        GeoMapOptions? mapOptions = null,
        GeoEditorOptions? editorOptions = null
    );
}