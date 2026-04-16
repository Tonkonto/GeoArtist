using GeoArtist.Contracts;

namespace GeoArtist.Abstractions;

/// <summary>
/// Builds <see cref="GeoRenderResult"/> from a normalized <see cref="GeoArtistPayload"/>.
/// </summary>
public interface IGeoRendererBridge
{
    /// <summary>
    /// Produces HTML, bootstrap script, dependency URLs, and serialized payload for the given runtime payload.
    /// </summary>
    /// <param name="payload">Normalized map or editor payload.</param>
    /// <returns>Combined render output for ASP.NET or desktop hosts.</returns>
    GeoRenderResult Render(GeoArtistPayload payload);
}
