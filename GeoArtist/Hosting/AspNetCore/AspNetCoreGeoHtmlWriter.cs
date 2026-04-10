using System.Text;
using GeoArtist.Abstractions;
using GeoArtist.Contracts;
using Microsoft.AspNetCore.Html;

namespace GeoArtist.Hosting.AspNetCore;

/// <summary>
/// Combines <see cref="GeoRenderResult"/> markup with optional <c>link</c> and <c>script</c> tags from <see cref="GeoRenderResult.StylePaths"/> and <see cref="GeoRenderResult.ScriptPaths"/>.
/// </summary>
public sealed class AspNetCoreGeoHtmlWriter
{
    /// <summary>
    /// Builds Razor-safe HTML that includes the rendered component and, when requested, dependency tags in list order.
    /// </summary>
    /// <param name="renderResult">Output from <see cref="IGeoArtist.RenderMap"/> or <see cref="IGeoArtist.RenderEditor"/>.</param>
    /// <param name="includeAssets">If <see langword="false"/>, only <see cref="GeoRenderResult.Html"/> is wrapped (no external CSS/JS tags).</param>
    /// <returns><see cref="IHtmlContent"/> for views or tag helpers.</returns>
    public IHtmlContent BuildHtmlContent(GeoRenderResult renderResult, bool includeAssets = true)
    {
        var sb = new StringBuilder();

        if (includeAssets)
        {
            AppendStyles(sb, renderResult);
        }

        sb.AppendLine(renderResult.Html);

        if (includeAssets)
        {
            AppendScripts(sb, renderResult);
        }

        return new HtmlString(sb.ToString());
    }

    private static void AppendStyles(StringBuilder sb, GeoRenderResult renderResult)
    {
        foreach (var stylePath in renderResult.StylePaths)
        {
            if (string.IsNullOrWhiteSpace(stylePath))
                continue;

            sb.AppendLine($"""<link rel="stylesheet" href="{stylePath}" />""");
        }
    }

    private static void AppendScripts(StringBuilder sb, GeoRenderResult renderResult)
    {
        foreach (var scriptPath in renderResult.ScriptPaths)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
                continue;

            sb.AppendLine($"""<script src="{scriptPath}"></script>""");
        }
    }
}

