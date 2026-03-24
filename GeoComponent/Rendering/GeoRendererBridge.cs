using GeoComponent.Abstractions;
using GeoComponent.Contracts;
using GeoComponent.Rendering.Html;
using GeoComponent.Rendering.Scripts;
using Microsoft.Extensions.Options;

namespace GeoComponent.Rendering;

public sealed class GeoRendererBridge(  HtmlTemplateBuilder htmlTemplateBuilder, HtmlDocumentBuilder htmlDocumentBuilder,
                                        ScriptBootstrapBuilder scriptBootstrapBuilder,
                                        IGeoDataSerializer serializer, IOptions<GeoComponentAssetOptions> assetOptions  ) : IGeoRendererBridge
{
    private readonly HtmlTemplateBuilder _htmlTemplateBuilder = htmlTemplateBuilder;
    private readonly HtmlDocumentBuilder _htmlDocumentBuilder = htmlDocumentBuilder;
    private readonly ScriptBootstrapBuilder _scriptBootstrapBuilder = scriptBootstrapBuilder;
    private readonly IGeoDataSerializer _serializer = serializer;
    private readonly GeoComponentAssetOptions _assetOptions = assetOptions.Value;

    public GeoRenderResult Render(GeoComponentPayload payload)
    {
        var componentHtml = _htmlTemplateBuilder.BuildMapHost(payload);
        var bootstrapScript = _scriptBootstrapBuilder.BuildBootstrapScript(payload);
        var html = _htmlDocumentBuilder.BuildFragment(componentHtml, bootstrapScript);
        var serializedPayload = _serializer.Serialize(payload);

        var stylePaths = BuildStylePaths(payload);
        var scriptPaths = BuildScriptPaths(payload);

        return new GeoRenderResult
        {
            Html = html,
            BootstrapScript = bootstrapScript,
            MapId = payload.MapId,
            SerializedPayload = serializedPayload,
            StylePaths = stylePaths,
            ScriptPaths = scriptPaths
        };
    }

    private List<string> BuildStylePaths(GeoComponentPayload payload)
    {
        List<string> result = [];
        HashSet<string> seen = new(StringComparer.Ordinal);

        AddRangeIfSet(result, _assetOptions.LeafletCssPaths, seen);
        AddRangeIfSet(result, _assetOptions.CssPaths, seen);

        if (payload.IsEditable)
            AddRangeIfSet(result, _assetOptions.GeomanCssPaths, seen);

        return result;
    }

    private List<string> BuildScriptPaths(GeoComponentPayload payload)
    {
        List<string> result = [];
        HashSet<string> seen = new(StringComparer.Ordinal);

        AddRangeIfSet(result, _assetOptions.LeafletJsPaths, seen);

        if (payload.IsEditable)
            AddRangeIfSet(result, _assetOptions.GeomanJsPaths, seen);

        AddRangeIfSet(result, _assetOptions.JsPaths, seen);

        return result;
    }

    private static void AddRangeIfSet(List<string> target, IEnumerable<string>? values, HashSet<string> seen)
    {
        if (values is null)
            return;

        foreach (var value in values)
        {
            if (string.IsNullOrWhiteSpace(value))
                continue;

            if (seen.Add(value))
                target.Add(value);
        }
    }
}
