using GeoComponent.Abstractions;
using GeoComponent.Contracts;

namespace GeoComponent.Rendering;

public sealed class StubGeoRendererBridge : IGeoRendererBridge
{
    public GeoRenderResult Render(GeoComponentPayload payload)
    {
        return new GeoRenderResult
        {
            MapId = payload.MapId,
            SerializedPayload = "{}",
            Html = $"<div id=\"{payload.MapId}\"></div>",
            BootstrapScript = string.Empty,
            StylePaths = Array.Empty<string>(),
            ScriptPaths = Array.Empty<string>()
        };
    }
}