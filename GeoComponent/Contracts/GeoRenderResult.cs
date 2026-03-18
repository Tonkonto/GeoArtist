namespace GeoComponent.Contracts;

public sealed class GeoRenderResult
{
    public string Html { get; set; } = "";
    public string BootstrapScript { get; set; } = "";

    public string MapId { get; set; } = "";
    public string SerializedPayload { get; set; } = "";

    public IReadOnlyList<string> StylePaths { get; init; } = [];
    public IReadOnlyList<string> ScriptPaths { get; init; } = [];
}