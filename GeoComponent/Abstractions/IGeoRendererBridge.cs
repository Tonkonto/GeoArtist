using GeoComponent.Contracts;

namespace GeoComponent.Abstractions;

public interface IGeoRendererBridge
{
    GeoRenderResult Render(GeoComponentPayload payload);
}