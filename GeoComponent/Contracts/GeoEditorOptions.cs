using System.ComponentModel.DataAnnotations;

namespace GeoComponent.Contracts;

public sealed class GeoEditorOptions
{
    // Allow Modes 1
    public bool AllowMarker { get; set; } = true;
    public bool AllowPolyline { get; set; } = true;
    public bool AllowPolygon { get; set; } = true;
    public bool AllowRectangle { get; set; } = true;
    public bool AllowCircle { get; set; } = true;
    public bool AllowCircleMarker { get; set; } = false;

    // Allow Modes 2
    public bool AllowEdit { get; set; } = true;
    public bool AllowDelete { get; set; } = true;
    public bool AllowCut { get; set; } = true;
    public bool AllowDrag { get; set; } = true;
    public bool AllowRotate { get; set; } = true;

    // Nodes customization
    public int SnapSensitivity { get; set; } = 20;

    [Range(6, 64)]
    public int NodeSize { get; set; } = 14;

    public string? DrawColor { get; set; } = "tomato";

    [Range(0.0, 1.0)]
    public double? DrawOpacity { get; set; } = 1.0;


    // System
    public bool Enabled { get; set; } = true;
    public bool AllowTextInputSync { get; set; } = true;
    public bool AutoApplyTextChanges { get; set; } = true;
}
