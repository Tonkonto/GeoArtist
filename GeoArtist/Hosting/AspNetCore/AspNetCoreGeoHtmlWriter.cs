using System.Text;
using GeoArtist.Abstractions;
using GeoArtist.Contracts;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;

namespace GeoArtist.Hosting.AspNetCore;

/// <summary>
/// Combines <see cref="GeoRenderResult"/> markup with optional <c>link</c> and <c>script</c> tags from <see cref="GeoRenderResult.StylePaths"/> and <see cref="GeoRenderResult.ScriptPaths"/>.
/// </summary>
public sealed class AspNetCoreGeoHtmlWriter(IHttpContextAccessor httpContextAccessor)
{
    private const string EmittedAssetUrlsItemKey = "GeoArtist.EmittedAssetUrls";

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Builds Razor-safe HTML that includes the rendered component and, when requested, dependency tags in list order.
    /// </summary>
    /// <param name="renderResult">Output from <see cref="IGeoArtist.RenderMap"/> or <see cref="IGeoArtist.RenderEditor"/>.</param>
    /// <param name="includeAssets">If <see langword="false"/>, only <see cref="GeoRenderResult.Html"/> is wrapped (no external CSS/JS tags).</param>
    /// <returns><see cref="IHtmlContent"/> for views or tag helpers.</returns>
    /// <remarks>
    /// When <paramref name="includeAssets"/> is <see langword="true"/> and an <see cref="HttpContext"/> is available,
    /// each stylesheet and script <c>href</c>/<c>src</c> is emitted at most once per HTTP request so multiple
    /// <c>geo-map</c> (or view components) with <c>include-assets="true"</c> do not duplicate tags.
    /// </remarks>
    public IHtmlContent BuildHtmlContent(GeoRenderResult renderResult, bool includeAssets = true)
    {
        var sb = new StringBuilder();

        HashSet<string>? emitted = null;
        if (includeAssets && _httpContextAccessor.HttpContext is { } http)
            emitted = GetOrCreateEmittedSet(http);

        if (includeAssets)
            AppendStyles(sb, renderResult, emitted);

        sb.AppendLine(renderResult.Html);

        if (includeAssets)
            AppendScripts(sb, renderResult, emitted);

        return new HtmlString(sb.ToString());
    }

    private static HashSet<string> GetOrCreateEmittedSet(HttpContext http)
    {
        if (http.Items[EmittedAssetUrlsItemKey] is HashSet<string> existing)
            return existing;

        var created = new HashSet<string>(StringComparer.Ordinal);
        http.Items[EmittedAssetUrlsItemKey] = created;
        return created;
    }

    private static void AppendStyles(StringBuilder sb, GeoRenderResult renderResult, HashSet<string>? emitted)
    {
        foreach (var stylePath in renderResult.StylePaths)
        {
            if (string.IsNullOrWhiteSpace(stylePath))
                continue;

            if (emitted is not null && !emitted.Add(stylePath))
                continue;

            sb.AppendLine($"""<link rel="stylesheet" href="{stylePath}" />""");
        }
    }

    private static void AppendScripts(StringBuilder sb, GeoRenderResult renderResult, HashSet<string>? emitted)
    {
        foreach (var scriptPath in renderResult.ScriptPaths)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
                continue;

            if (emitted is not null && !emitted.Add(scriptPath))
                continue;

            sb.AppendLine($"""<script src="{scriptPath}"></script>""");
        }
    }
}
