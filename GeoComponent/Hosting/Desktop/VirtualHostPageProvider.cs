using System.Net;
using System.Text;
using GeoComponent.Contracts;

namespace GeoComponent.Hosting.Desktop;

public sealed class VirtualHostPageProvider
{
    public string BuildPage(GeoRenderResult renderResult, string? title = null)
    {
        var safeTitle = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(title) ? "GeoArtist" : title);

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"en\">");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"utf-8\" />");
        sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />");
        sb.AppendLine($"  <title>{safeTitle}</title>");

        foreach (var stylePath in renderResult.StylePaths)
        {
            if (string.IsNullOrWhiteSpace(stylePath))
                continue;

            sb.AppendLine($"  <link rel=\"stylesheet\" href=\"{WebUtility.HtmlEncode(stylePath)}\" />");
        }

        sb.AppendLine("</head>");
        sb.AppendLine("<body style=\"margin:0;\">");
        sb.AppendLine(renderResult.Html);

        foreach (var scriptPath in renderResult.ScriptPaths)
        {
            if (string.IsNullOrWhiteSpace(scriptPath))
                continue;

            sb.AppendLine($"  <script src=\"{WebUtility.HtmlEncode(scriptPath)}\"></script>");
        }

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }
}
