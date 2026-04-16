using System.ComponentModel.DataAnnotations;

namespace GeoArtist.Contracts;

/// <summary>
/// Map layout, Leaflet view, styling, and optional coordinate reprojection for the GeoArtist component.
/// </summary>
/// <remarks>
/// String values that affect the DOM or Leaflet style APIs use CSS syntax where noted
/// </remarks>
public sealed class GeoMapOptions
{
    // ===== DOM data =====
    /// <summary>
    /// HTML <c>id</c> attrib of the map container element
    /// </summary>
    public string MapId { get; set; } = "geoartist-map";

    /// <summary>
    /// Specifies the map container height.
    /// </summary>
    /// <remarks>
    /// Applied as inline <c>style="height:n"</c> on the map container element. <br/>
    /// <c>Width</c> is not configured via <see cref="GeoMapOptions"/> and is controlled by CSS/layout (default would be 100% of the parent container).
    /// </remarks>
    /// <value>
    /// CSS value
    /// </value>
    public string Height { get; set; } = "600px";
    // ===== /DOM data =====

    // ===== Map behavior =====
    /// <summary>
    /// Specifies the Initial Latitude in degrees
    /// </summary>
    /// <value>
    /// Range: [-90.0, 90.0] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(-90.0, 90.0)]
    public double InitialLat { get; set; } = 42.8746;

    /// <summary>
    /// Specifies the Initial Longitude in degrees
    /// </summary>
    /// <value>
    /// Range: [-90.0, 90.0] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(-90.0, 90.0)]
    public double InitialLng { get; set; } = 74.5698;

    /// <summary>
    /// Specifies the Initial Zoom level
    /// </summary>
    /// <value>
    /// Range: [0, 24] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0, 24)]
    public int InitialZoom { get; set; } = 12;

    /// <summary>
    /// Maximum zoom level allowed on the map
    /// </summary>
    /// <value>
    /// Range: [0, 24] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0, 24)]
    public int? MaxZoom { get; set; } = 19;

    /// <summary>
    /// Enables automatic fitting of map bounds to loaded GeoJSON <br/>
    /// (auto focus on the loaded shape)
    /// </summary>
    public bool FitBounds { get; set; } = true;

    /// <summary>
    /// Enables zooming the map on double-click.
    /// </summary>
    public bool DoubleClickZoom { get; set; } = false;
    // ===== /Map behavior =====

    // ===== Polygon customization =====
    /// <summary>
    /// Specifies Border Color for polygons.
    /// </summary>
    public string PolygonBorderColor { get; set; } = "#3388ff";

    /// <summary>
    /// Specifies Fill Color for polygons.
    /// </summary>
    public string PolygonFillColor { get; set; } = "#3388ff";

    /// <summary>
    /// Specifies Border Opacity for polygons.
    /// </summary>
    /// <value>
    /// Range: [0.0, 1.0] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0.0, 1.0)]
    public double PolygonBorderOpacity { get; set; } = 0.5;

    /// <summary>
    /// Specifies Fill Opacity for polygons
    /// </summary>
    /// <value>
    /// Range: [0.0, 1.0] <br/>
    /// Enforced via <see cref="RangeAttribute"/>.
    /// </value>
    [Range(0.0, 1.0)]
    public double PolygonFillOpacity { get; set; } = 0.5;
    // ===== /Polygon customization =====

    // ===== System settings =====
    /// <summary>
    /// Controls visibility of the raster tile layer
    /// </summary>
    public bool ShowTileLayer { get; set; } = true;

    /// <summary>
    /// Specifies the URL template for the Leaflet tile layer
    /// </summary>
    public string TileLayerUrl { get; set; } = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";

    /// <summary>
    /// Specifies the attribution HTML for the tile layer. <br/>
    /// Inserted into Leaflet’s attribution control; may contain markup (e.g. links).
    /// </summary>
    public string TileLayerAttribution { get; init; } = "<a href=\"https://www.openstreetmap.org/copyright\" target=\"_blank\" rel=\"noopener noreferrer\">&copy; OpenStreetMap</a>";
    // ===== /System settings =====

    // ===== GeoJSON data =====
    /// <summary>
    /// Specifies the source SRID for input coordinates, transformed to WGS 84 if set. <br/>
    /// Omit or use <c>4326</c> when GeoJSON is already lon/lat in WGS 84.
    /// </summary>
    public int? SourceSrid { get; set; }
    // ===== /GeoJSON data =====
}
