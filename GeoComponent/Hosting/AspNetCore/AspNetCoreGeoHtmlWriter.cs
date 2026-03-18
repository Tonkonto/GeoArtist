using System.Text;
using GeoComponent.Contracts;
using Microsoft.AspNetCore.Html;

namespace GeoComponent.Hosting.AspNetCore;

public sealed class AspNetCoreGeoHtmlWriter
{
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