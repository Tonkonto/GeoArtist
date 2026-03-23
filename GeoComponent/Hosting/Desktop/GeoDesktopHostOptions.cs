namespace GeoComponent.Hosting.Desktop;

public sealed class GeoDesktopHostOptions
{
    public string VirtualHostName { get; set; } = "geoartist.local";
    public string HostName => NormalizeHost(VirtualHostName);

    public string VirtualHostOrigin
    {
        get { return $"https://{HostName}"; }
    }

    public string VirtualHostReferer => $"{VirtualHostOrigin}/";

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
