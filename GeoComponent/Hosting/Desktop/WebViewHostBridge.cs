using GeoComponent.Abstractions;
using GeoComponent.Contracts;

namespace GeoComponent.Hosting.Desktop;

public sealed class WebViewHostBridge(
    IGeoComponent geoComponent,
    VirtualHostPageProvider pageProvider,
    IGeoDataSerializer serializer)
{
    private readonly IGeoComponent _geoComponent = geoComponent;
    private readonly VirtualHostPageProvider _pageProvider = pageProvider;
    private readonly IGeoDataSerializer _serializer = serializer;

    public string BuildMapPage(string? geoJson, GeoMapOptions? mapOptions = null, string? title = null)
    {
        var renderResult = _geoComponent.RenderMap(geoJson, mapOptions);
        return _pageProvider.BuildPage(renderResult, title);
    }

    public string BuildEditorPage(
        string? geoJson,
        GeoMapOptions? mapOptions = null,
        GeoEditorOptions? editorOptions = null,
        string? title = null)
    {
        var renderResult = _geoComponent.RenderEditor(geoJson, mapOptions, editorOptions);
        return _pageProvider.BuildPage(renderResult, title);
    }

    public string BuildBootstrapScript(GeoComponentPayload payload)
    {
        var serializedPayload = _serializer.Serialize(payload);
        return $"window.GeoArtist.bootstrap({serializedPayload});";
    }

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
        return $$"""{"type":"geoartist.render","payload":{{serializedPayload}}}""";
    }
}
