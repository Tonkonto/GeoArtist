namespace GeoComponent.Contracts;

public sealed class GeoComponentPayload
{
    public string Mode { get; set; } = "map";
    public string MapId { get; set; } = "geoartist-map";
    public string GeoJson { get; set; } = "";

    public GeoMapOptions MapOptions { get; set; } = new();
    public GeoEditorOptions? EditorOptions { get; set; }

    public bool IsEditable => string.Equals(Mode, "editor", StringComparison.OrdinalIgnoreCase);
}