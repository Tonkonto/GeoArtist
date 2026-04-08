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
    [Range(0, 64)]
    public int SnapSensitivity { get; set; } = 20;

    [Range(6, 64)]
    public int NodeSize { get; set; } = 14;

    public string NodeColor { get; set; } = "#000";

    public string DrawColor { get; set; } = "#0f766e";

    [Range(0.0, 1.0)]
    public double DrawOpacity { get; set; } = 0.75;

    /// <summary>
    /// Overrides Leaflet drag click tolerance in pixels.
    /// Higher values reduce sensitivity making speed drawing easier.
    /// </summary>
    [Range(0, 64)]
    public int DragClickTolerance { get; set; } = 3;

    //UI Scale
    /// <summary>
    /// Global UI scale multiplier for Geoman Controls.
    /// </summary>
    [Range(0.4, 2.5)]
    public double UiScale { get; set; } = 1.0;

    /// <summary>
    /// Additional scale multiplier for the geoman Toolbar.
    /// </summary>
    [Range(0.4, 2.5)]
    public double ToolbarScale { get; set; } = 1.0;

    /// <summary>
    /// Additional scale multiplier for geoman Action Menus.
    /// </summary>
    [Range(0.4, 2.5)]
    public double ActionsScale { get; set; } = 1.0;

    /// <summary>
    /// Additional scale multiplier for geoman Tooltips.
    /// </summary>
    [Range(0.4, 2.5)]
    public double TooltipScale { get; set; } = 1.0;

    /// <summary>
    /// Renders geoman Action Menus vertically when true.
    /// </summary>
    public bool ActionsVertical { get; set; } = false;

    // System
    public bool Enabled { get; set; } = true;
    public bool UseGeoJsonTextArea { get; set; } = false;
    public bool AllowJsonEditing { get; set; } = true;
    public bool AutoApplyJsonChanges { get; set; } = true;
}
