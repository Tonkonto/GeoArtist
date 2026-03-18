using System.Text;

namespace GeoComponent.Rendering.Html;

public sealed class HtmlDocumentBuilder
{
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