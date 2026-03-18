using GeoComponent.Abstractions;
using GeoComponent.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace GeoComponent.Hosting.AspNetCore;

public sealed class GeoMapViewComponent(IGeoComponent geoComponent, AspNetCoreGeoHtmlWriter htmlWriter) : ViewComponent
{
    private readonly IGeoComponent _geoComponent = geoComponent;
    private readonly AspNetCoreGeoHtmlWriter _htmlWriter = htmlWriter;


    public IViewComponentResult Invoke(
        string? geoJson = null,
        GeoMapOptions? mapOptions = null,
        bool includeAssets = true )
    {
        var renderResult = _geoComponent.RenderMap(geoJson, mapOptions);
        var htmlContent = _htmlWriter.BuildHtmlContent(renderResult, includeAssets);

        return new HtmlContentViewComponentResult(htmlContent);
    }

    public IViewComponentResult InvokeEditor(
        string? geoJson = null,
        GeoMapOptions? mapOptions = null,
        GeoEditorOptions? editorOptions = null,
        bool includeAssets = true )
    {
        var renderResult = _geoComponent.RenderEditor(geoJson, mapOptions, editorOptions);
        var htmlContent = _htmlWriter.BuildHtmlContent(renderResult, includeAssets);

        return new HtmlContentViewComponentResult(htmlContent);
    }
}