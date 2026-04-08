using System.Text;
using GeoComponent.Contracts;

namespace GeoComponent.Rendering.Html;

public sealed class HtmlTemplateBuilder
{
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