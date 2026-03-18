using GeoComponent.Abstractions;
using GeoComponent.Contracts;
using GeoComponent.Rendering.Assets;
using GeoComponent.Rendering.Html;
using GeoComponent.Rendering.Scripts;

namespace GeoComponent.Rendering;

public sealed class GeoRendererBridge : IGeoRendererBridge
{
    private readonly HtmlTemplateBuilder _htmlTemplateBuilder;
    private readonly HtmlDocumentBuilder _htmlDocumentBuilder;
    private readonly ScriptBootstrapBuilder _scriptBootstrapBuilder;
    private readonly IGeoDataSerializer _serializer;
    private readonly GeoAssetManifest _assetManifest;

    public GeoRendererBridge(
        HtmlTemplateBuilder htmlTemplateBuilder,
        HtmlDocumentBuilder htmlDocumentBuilder,
        ScriptBootstrapBuilder scriptBootstrapBuilder,
        IGeoDataSerializer serializer,
        GeoAssetManifest assetManifest)
    {
        _htmlTemplateBuilder = htmlTemplateBuilder;
        _htmlDocumentBuilder = htmlDocumentBuilder;
        _scriptBootstrapBuilder = scriptBootstrapBuilder;
        _serializer = serializer;
        _assetManifest = assetManifest;
    }

    public GeoRenderResult Render(GeoComponentPayload payload)
    {
        var componentHtml = _htmlTemplateBuilder.BuildMapHost(payload);
        var bootstrapScript = _scriptBootstrapBuilder.BuildBootstrapScript(payload);
        var html = _htmlDocumentBuilder.BuildFragment(componentHtml, bootstrapScript);
        var serializedPayload = _serializer.Serialize(payload);

        return new GeoRenderResult
        {
            Html = html,
            BootstrapScript = bootstrapScript,
            MapId = payload.MapId,
            SerializedPayload = serializedPayload,
            StylePaths = _assetManifest.StylePaths,
            ScriptPaths = _assetManifest.ScriptPaths
        };
    }
}