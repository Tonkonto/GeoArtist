namespace GeoComponent.Contracts;

public sealed class GeoEditorOptions
{
    public bool Enabled { get; set; } = true;
    public bool AllowPolygon { get; set; } = true;
    public bool AllowRectangle { get; set; } = true;
    public bool AllowPolyline { get; set; } = true;
    public bool AllowMarker { get; set; } = true;
    public bool AllowCircleMarker { get; set; } = false;
    public bool AllowCircle { get; set; } = false;

    public bool AllowEdit { get; set; } = true;
    public bool AllowDelete { get; set; } = true;
    public bool AllowCut { get; set; } = false;
    public bool AllowDrag { get; set; } = false;
    public bool AllowRotate { get; set; } = false;
}