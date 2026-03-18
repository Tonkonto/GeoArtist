namespace GeoComponent.Rendering.Assets;

public sealed class GeoAssetManifest
{
    public IReadOnlyList<string> StylePaths { get; } =
        ["/_content/GeoComponent/css/geoartist.css"];

    public IReadOnlyList<string> ScriptPaths { get; } =
        ["/_content/GeoComponent/js/geoartist.js"];
}