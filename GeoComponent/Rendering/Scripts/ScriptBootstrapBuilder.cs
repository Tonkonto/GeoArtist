using System.Text;
using GeoComponent.Abstractions;
using GeoComponent.Contracts;

namespace GeoComponent.Rendering.Scripts;

public sealed class ScriptBootstrapBuilder(IGeoDataSerializer serializer)
{
    private readonly IGeoDataSerializer _serializer = serializer;

    public string BuildBootstrapScript(GeoComponentPayload payload)
    {
        var serializedPayload = _serializer.Serialize(payload);
        var serializedMapId = _serializer.Serialize(payload.MapId);

        var sb = new StringBuilder();

        sb.AppendLine("(function () {");
        sb.AppendLine("  const payload = " + serializedPayload + ";");
        sb.AppendLine("  const mapElementId = " + serializedMapId + ";");
        sb.AppendLine();
        sb.AppendLine("  function start() {");
        sb.AppendLine("    if (!window.GeoArtist || typeof window.GeoArtist.bootstrap !== 'function') {");
        sb.AppendLine("      console.error('GeoArtist bootstrap API is unavailable.');");
        sb.AppendLine("      return;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    const mapElement = document.getElementById(mapElementId);");
        sb.AppendLine("    if (!mapElement) {");
        sb.AppendLine("      console.error('GeoArtist map container not found:', mapElementId);");
        sb.AppendLine("      return;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    window.GeoArtist.bootstrap(payload);");
        sb.AppendLine("  }");
        sb.AppendLine();
        sb.AppendLine("  if (document.readyState === 'loading') {");
        sb.AppendLine("    document.addEventListener('DOMContentLoaded', start, { once: true });");
        sb.AppendLine("  } else {");
        sb.AppendLine("    start();");
        sb.AppendLine("  }");
        sb.AppendLine("})();");

        return sb.ToString();
    }
}