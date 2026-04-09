namespace GeoComponent.Rendering.Scripts;

/// <summary>
/// JSON <c>type</c> discriminator strings for WebView2 host page messaging (<c>host.html</c> and <see cref="Hosting.Desktop.GeoDesktopWebViewAdapter"/>).
/// </summary>
public static class ScriptMessageNames
{
    /// <summary>
    /// Message type for a render request: payload is the serialized <see cref="Contracts.GeoComponentPayload"/>.
    /// </summary>
    public const string Render = "geoartist.render";

    /// <summary>
    /// Message type posted by <c>host.html</c> when the page is ready to receive <see cref="Render"/> (handled in <see cref="Hosting.Desktop.GeoDesktopWebViewAdapter"/>).
    /// </summary>
    public const string HostReady = "geoartist.host.ready";
}
