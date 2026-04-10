using System.Text;

namespace GeoArtist.Rendering.Html;

/// <summary>
/// Wraps component HTML and an optional inline bootstrap script in a single fragment string.
/// </summary>
public sealed class HtmlDocumentBuilder
{
    /// <summary>
    /// Appends <paramref name="componentHtml"/> and <c>script</c> block containing <paramref name="bootstrapScript"/>.
    /// </summary>
    /// <param name="componentHtml">Markup from <see cref="HtmlTemplateBuilder.BuildMapHost"/>.</param>
    /// <param name="bootstrapScript">JavaScript source (not URL) that starts GeoArtist on the page.</param>
    /// <returns>Combined HTML fragment.</returns>
    public string BuildFragment(string componentHtml, string bootstrapScript)
    {
        var sb = new StringBuilder();

        sb.AppendLine(componentHtml);

        if (!string.IsNullOrWhiteSpace(bootstrapScript))
        {
            sb.AppendLine("<script>");
            sb.AppendLine(bootstrapScript);
            sb.AppendLine("</script>");
        }

        return sb.ToString();
    }
}
