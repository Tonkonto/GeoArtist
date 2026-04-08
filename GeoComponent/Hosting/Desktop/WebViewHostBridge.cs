using GeoComponent.Abstractions;
using GeoComponent.Contracts;
using GeoComponent.Rendering.Scripts;

namespace GeoComponent.Hosting.Desktop;

public sealed class WebViewHostBridge(IGeoComponent geoComponent)
{
    private readonly IGeoComponent _geoComponent = geoComponent;

    public string BuildHostPageUrl(GeoDesktopHostOptions hostOptions)
    {
        return hostOptions.ToAssetUrl("host.html");
    }

    public string BuildMapRenderMessage(string? geoJson, GeoMapOptions? mapOptions = null)
    {
        var renderResult = _geoComponent.RenderMap(geoJson, mapOptions);
        return BuildRenderMessage(renderResult.SerializedPayload);
    }

    public string BuildEditorRenderMessage(
        string? geoJson,
        GeoMapOptions? mapOptions = null,
        GeoEditorOptions? editorOptions = null)
    {
        var renderResult = _geoComponent.RenderEditor(geoJson, mapOptions, editorOptions);
        return BuildRenderMessage(renderResult.SerializedPayload);
    }

    private static string BuildRenderMessage(string serializedPayload)
    {
        return $$"""{"type":"{{ScriptMessageNames.Render}}","payload":{{serializedPayload}}}""";
    }
}
