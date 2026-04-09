using System.Text;
using GeoComponent.Contracts;

namespace GeoComponent.Rendering.Html;

/// <summary>
/// Builds the static HTML shell (map container and optional GeoJSON textarea) for a payload.
/// </summary>
public sealed class HtmlTemplateBuilder
{
    /// <summary>
    /// Creates the <c>geoartist-shell</c> wrapper, map <c>div</c> with inline height from <see cref="GeoMapOptions.Height"/>, and optional textarea when editor mode and <see cref="GeoEditorOptions.UseGeoJsonTextArea"/> allow it.
    /// </summary>
    /// <param name="payload">Runtime payload after normalization.</param>
    /// <returns>HTML fragment (no <c>script</c> tags).</returns>
    public string BuildMapHost(GeoComponentPayload payload)
    {
        var mapId = payload.MapId;
        var height = payload.MapOptions.Height;

        var sb = new StringBuilder();

        sb.AppendLine($"""<div class="geoartist-shell" data-geoartist-mode="{payload.Mode}">""");
        sb.AppendLine($"""  <div id="{mapId}" class="geoartist-map" style="height: {height};"></div>""");

        if (payload.IsEditable && (payload.EditorOptions?.UseGeoJsonTextArea ?? true))
        {
            sb.AppendLine($"""  <textarea id="{mapId}-geojson-output" class="geoartist-geojson-output" spellcheck="false"></textarea>""");
        }

        sb.AppendLine("</div>");

        return sb.ToString();
    }
}
