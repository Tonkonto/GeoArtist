using System.ComponentModel.DataAnnotations;

namespace GeoComponent.Contracts;

/// <summary>
/// Defines Geoman editor settings, available tools, edit modes, <br/>
/// drawing behavior, UI scaling, GeoJSON textarea.
/// </summary>
/// <remarks>
/// See <see href="https://geoman.io/docs/leaflet" target="_blank">Geoman Documentation</see> for detailed descriptions of the available tools, modes and options.
/// </remarks>
public sealed class GeoEditorOptions
{
    //  ===== Geoman Tools =====
    //  == Toolbar 1 – Drawing tools ==
    /// <summary>
    /// Adds button to draw Markers
    /// </summary>
    public bool AllowMarker { get; set; } = true;

    /// <summary>
    /// Adds button to draw Line
    /// </summary>
    public bool AllowPolyline { get; set; } = true;

    /// <summary>
    /// Adds button to draw Polygon
    /// </summary>
    public bool AllowPolygon { get; set; } = true;

    /// <summary>
    /// Adds button to draw Rectangle
    /// </summary>
    public bool AllowRectangle { get; set; } = true;

    /// <summary>
    /// Adds button to draw Circle
    /// </summary>
    public bool AllowCircle { get; set; } = true;

    /// <summary>
    /// Adds button to draw CircleMarkers <br/>
    /// (a circle with a fixed scaled pixel radius)
    /// </summary>
    /// <remarks>
    /// Geoman-only feature, not standard GeoJSON geometry.
    /// </remarks>
    public bool AllowCircleMarker { get; set; } = false;

    /// <summary>
    /// Adds button to draw Text layers
    /// </summary>
    /// <remarks>
    /// Geoman-only feature, not standard GeoJSON geometry.
    /// </remarks>
    public bool AllowText { get; set; } = false;

    //  == Toolbar 2 – Editing tools ==
    /// <summary>
    /// Enables Editing of existing shapes (nodes dragging)
    /// </summary>
    public bool AllowEdit { get; set; } = true;

    /// <summary>
    /// Enables Removal of shapes.
    /// </summary>
    public bool AllowDelete { get; set; } = true;

    /// <summary>
    /// Enables Cutting of existing shapes
    /// </summary>
    /// <remarks>
    /// <see href="https://geoman.io/docs/leaflet/modes/cut-mode" target="_blank">Geoman Docs</see>: <br/>
    /// Enables drawing for the shape "Cut" to draw a polygon that gets subtracted from all underlying polygons. <br/>
    /// This way you can create holes, cut polygons or polylines in half or remove parts of it.
    /// </remarks>
    public bool AllowCut { get; set; } = false;

    /// <summary>
    /// Enables Dragging of shapes.
    /// </summary>
    public bool AllowDrag { get; set; } = true;

    /// <summary>
    /// Enables Rotation of shapes
    /// </summary>
    public bool AllowRotate { get; set; } = false;
    // ===== /Geoman Tools =====

    // ===== Nodes, Polygons customization =====
    /// <summary>
    /// Specifies Snap distance in pixels while drawing or editing
    /// </summary>
    /// <value>
    /// Range: [0, 64] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0, 64)]
    public int SnapSensitivity { get; set; } = 20;

    /// <summary>
    /// Specifies Size in pixels for Vertex handlers (while drawing, editing)
    /// </summary>
    /// <value>
    /// Range: [0, 64] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(6, 64)]
    public int NodeSize { get; set; } = 14;

    /// <summary>
    /// Specifies Color of vertex handles (CSS color)
    /// </summary>
    /// <value>
    /// CSS color
    /// </value>
    public string NodeColor { get; set; } = "#000";

    /// <summary>
    /// Specifies drawing preview Color
    /// </summary>
    /// <value>
    /// CSS color
    /// </value>
    public string DrawColor { get; set; } = "#0f766e";

    /// <summary>
    /// Opacity for drawing preview
    /// </summary>
    /// <value>
    /// Range: [0.0, 1.0] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0.0, 1.0)]
    public double DrawOpacity { get; set; } = 0.75;

    /// <summary>
    /// Overrides Leaflet drag click tolerance in pixels.
    /// </summary>
    /// <remarks>
    /// Higher values reduce sensitivity making speed drawing easier.
    /// </remarks>
    /// <value>
    /// Range: [0, 64] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0, 64)]
    public int DragClickTolerance { get; set; } = 3;
    // ===== /Nodes, Polygons customization =====

    // ===== UI customization =====
    /// <summary>
    /// Global UI scale multiplier for geoman controls.
    /// </summary>
    /// <value>
    /// Range: [0.4, 2.5] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0.4, 2.5)]
    public double UiScale { get; set; } = 1.0;

    /// <summary>
    /// Additional scale multiplier for the geoman Toolbar.
    /// </summary>
    /// <value>
    /// Range: [0.4, 2.5] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0.4, 2.5)]
    public double ToolbarScale { get; set; } = 1.0;

    /// <summary>
    /// Additional scale multiplier for geoman Action menus.
    /// </summary>
    /// <value>
    /// Range: [0, 64] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0.4, 2.5)]
    public double ActionsScale { get; set; } = 1.0;

    /// <summary>
    /// Additional scale multiplier for geoman Tooltips.
    /// </summary>
    /// <value>
    /// Range: [0.4, 2.5] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0.4, 2.5)]
    public double TooltipScale { get; set; } = 1.0;

    /// <summary>
    /// Renders geoman Action Menus vertically when true.
    /// </summary>
    public bool ActionsVertical { get; set; } = true;
    // ===== /UI customization =====

    // ===== System settings =====
    /// <summary>
    /// Toggles Geoman Tools entirely
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// <see langword="True"/>: renders a GeoJSON textarea below the map and keeps it in sync if <see cref="AutoApplyJsonChanges"/> is <see langword="true"/>. <br/>
    /// <see langword="False"/>: only the map UI is used; changes still surface via client events.
    /// </summary>
    public bool UseGeoJsonTextArea { get; set; } = false;

    /// <summary>
    /// Enables editing of GeoJSON textarea. Updates the map if <see cref="AutoApplyJsonChanges"/> is <see langword="true"/>.
    /// </summary>
    public bool AllowJsonEditing { get; set; } = true;

    /// <summary>
    /// Enables auto shapes update on textarea editing.
    /// </summary>
    public bool AutoApplyJsonChanges { get; set; } = true;
    // ===== /System settings =====
}
