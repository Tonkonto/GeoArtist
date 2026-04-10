using GeoComponent.Abstractions;
using GeoComponent.Contracts;
using GeoComponent.Rendering.Scripts;

namespace GeoComponent.Hosting.Desktop;

/// <summary>
/// Builds JSON messages for the desktop <c>host.html</c> bootstrap using <see cref="IGeoComponent"/>.
/// </summary>
/// <param name="geoComponent">Server-side renderer used to produce normalized payloads.</param>
public sealed class WebViewHostBridge(IGeoComponent geoComponent)
{
    private readonly IGeoComponent _geoComponent = geoComponent;

    /// <summary>
    /// Returns the absolute virtual-host URL for <c>host.html</c>.
    /// </summary>
    public string BuildHostPageUrl(GeoDesktopHostOptions hostOptions)
    {
        return hostOptions.ToAssetUrl("host.html");
    }

    /// <summary>
    /// Serializes a map-mode render instruction for <c>postMessage</c> to <c>host.html</c>.
    /// </summary>
    public string BuildMapRenderMessage(string? geoJson, GeoMapOptions? mapOptions = null)
    {
        var renderResult = _geoComponent.RenderMap(geoJson, mapOptions);
        return BuildRenderMessage(renderResult.SerializedPayload);
    }

    /// <summary>
    /// Serializes an editor-mode render instruction for <c>postMessage</c> to <c>host.html</c>.
    /// </summary>
    public string BuildEditorRenderMessage(string? geoJson, GeoMapOptions? mapOptions = null, GeoEditorOptions? editorOptions = null)
    {
        var renderResult = _geoComponent.RenderEditor(geoJson, mapOptions, editorOptions);
        return BuildRenderMessage(renderResult.SerializedPayload);
    }

    private static string BuildRenderMessage(string serializedPayload)
    {
        return $$"""{"type":"{{ScriptMessageNames.Render}}","payload":{{serializedPayload}}}""";
    }
}
