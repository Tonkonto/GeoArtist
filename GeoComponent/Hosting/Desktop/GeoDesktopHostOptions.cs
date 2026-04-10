namespace GeoComponent.Hosting.Desktop;

/// <summary>
/// Virtual host settings used by the WinForms WebView2 adapter to map static assets to a stable HTTPS origin.
/// </summary>
public sealed class GeoDesktopHostOptions
{
    /// <summary>
    /// Host name passed to <c>SetVirtualHostNameToFolderMapping</c> (may include or omit a scheme; it is normalized).
    /// </summary>
    public string VirtualHostName { get; set; } = "geoartist.local";

    /// <summary>
    /// Normalized host name suitable for virtual host mapping and URL construction.
    /// </summary>
    public string HostName => NormalizeHost(VirtualHostName);

    /// <summary>
    /// Base origin (<c>https://{HostName}</c>) for resolving packaged static assets.
    /// </summary>
    public string VirtualHostOrigin
    {
        get { return $"https://{HostName}"; }
    }

    /// <summary>
    /// Referer header value used when emulating browser navigation to the virtual host root.
    /// </summary>
    public string VirtualHostReferer => $"{VirtualHostOrigin}/";

    /// <summary>
    /// Builds an absolute URL for a path under the virtual host origin.
    /// </summary>
    /// <param name="relativePath">Path relative to the mapped wwwroot folder (for example <c>host.html</c>).</param>
    public string ToAssetUrl(string relativePath)
    {
        var path = relativePath.TrimStart('/');
        return $"{VirtualHostOrigin}/{path}";
    }

    private static string NormalizeHost(string host)
    {
        var normalized = host.Trim().TrimEnd('/');

        if (normalized.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            normalized = normalized[7..];
        else if (normalized.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            normalized = normalized[8..];

        return string.IsNullOrWhiteSpace(normalized) ? "geoartist.local" : normalized;
    }
}
