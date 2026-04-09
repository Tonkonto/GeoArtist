using GeoComponent.Abstractions;
using GeoComponent.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace GeoComponent.Hosting.AspNetCore;

/// <summary>
/// MVC ViewComponent alternative to <see cref="GeoMapTagHelper"/> for map or editor rendering.
/// </summary>
public sealed class GeoMapViewComponent(IGeoComponent geoComponent, AspNetCoreGeoHtmlWriter htmlWriter) : ViewComponent
{
    private readonly IGeoComponent _geoComponent = geoComponent;
    private readonly AspNetCoreGeoHtmlWriter _htmlWriter = htmlWriter;


    /// <summary>
    /// Renders display-only map mode (Leaflet only).
    /// </summary>
    /// <param name="geoJson">GeoJSON string for the map, or <see langword="null"/> for empty data.</param>
    /// <param name="mapOptions">Map layout and styling; defaults when <see langword="null"/>.</param>
    /// <param name="includeAssets">When <see langword="true"/>, emits <c>link</c> and <c>script</c> tags for Leaflet and GeoArtist.</param>
    /// <returns>HTML content suitable for embedding in a view.</returns>
    public IViewComponentResult Invoke(string? geoJson = null, GeoMapOptions? mapOptions = null, bool includeAssets = true)
    {
        var renderResult = _geoComponent.RenderMap(geoJson, mapOptions);
        var htmlContent = _htmlWriter.BuildHtmlContent(renderResult, includeAssets);

        return new HtmlContentViewComponentResult(htmlContent);
    }

    /// <summary>
    /// Renders editor mode (Leaflet + Geoman).
    /// </summary>
    /// <param name="geoJson">GeoJSON string for the editor, or <see langword="null"/> for empty data.</param>
    /// <param name="mapOptions">Map layout and styling; defaults when <see langword="null"/>.</param>
    /// <param name="editorOptions">Geoman and textarea options; defaults when <see langword="null"/>.</param>
    /// <param name="includeAssets">When <see langword="true"/>, emits styles and scripts including Geoman.</param>
    /// <returns>HTML content suitable for embedding in a view.</returns>
    public IViewComponentResult InvokeEditor(string? geoJson = null, bool includeAssets = true,
                                             GeoMapOptions? mapOptions = null, GeoEditorOptions? editorOptions = null)
    {
        var renderResult = _geoComponent.RenderEditor(geoJson, mapOptions, editorOptions);
        var htmlContent = _htmlWriter.BuildHtmlContent(renderResult, includeAssets);

        return new HtmlContentViewComponentResult(htmlContent);
    }
}
