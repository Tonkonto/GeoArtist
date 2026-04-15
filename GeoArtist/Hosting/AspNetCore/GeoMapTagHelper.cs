using GeoArtist.Abstractions;
using GeoArtist.Contracts;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GeoArtist.Hosting.AspNetCore;

/// <summary>
/// ASP.NET Core TagHelper for rendering GeoArtist component.
/// </summary>
/// <remarks>
/// <para>Renders a <c>Map</c> or <c>Editor</c> instance based on the specified mode and options.
/// Uses <see cref="IGeoArtist"/> to generate the rendering result and injects
/// the resulting HTML into the Razor output.</para>
/// 
/// <para>Supported modes:</para>
/// <list type="bullet">
/// <item><description><c>Map</c> — display-only map</description></item>
/// <item><description><c>Editor</c> — interactive editor with drawing tools</description></item>
/// </list>
/// <para>Example:</para>
/// 
/// <code>
/// &lt;geo-map
///     geo-json="@Model.GeoJson"
///     mode="editor"
///     include-assets="true" /&gt;
/// </code>
/// </remarks>
[HtmlTargetElement("geo-map")]
public sealed class GeoMapTagHelper(IGeoArtist geoComponent, AspNetCoreGeoHtmlWriter htmlWriter) : TagHelper
{
    private readonly IGeoArtist _geoComponent = geoComponent;
    private readonly AspNetCoreGeoHtmlWriter _htmlWriter = htmlWriter;


    /// <summary>
    /// Specifies the GeoJSON to render.
    /// </summary>
    [HtmlAttributeName("geo-json")]
    public string? GeoJson { get; set; }


    /// <summary>
    /// Specifies the map configuration options.
    /// </summary>
    /// <remarks>
    /// Includes map id, dimensions, initial coordinates, zoom level and tile settings.
    /// If not provided, default options will be used.
    /// </remarks>
    [HtmlAttributeName("map-options")]
    public GeoMapOptions? MapOptions { get; set; }

    /// <summary>
    /// Optional map id override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("map-id")]
    public string? MapId { get; set; }

    /// <summary>
    /// Optional map container height override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("height")]
    public string? Height { get; set; }

    /// <summary>
    /// Optional initial longitude override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("initial-lng")]
    public double? InitialLng { get; set; }

    /// <summary>
    /// Optional initial latitude override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("initial-lat")]
    public double? InitialLat { get; set; }

    /// <summary>
    /// Optional initial zoom override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("initial-zoom")]
    public int? InitialZoom { get; set; }

    /// <summary>
    /// Optional max zoom override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("max-zoom")]
    public int? MaxZoom { get; set; }

    /// <summary>
    /// Optional fit-bounds override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("fit-bounds")]
    public bool? FitBounds { get; set; }

    /// <summary>
    /// Optional polygon border color override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("polygon-border-color")]
    public string? PolygonBorderColor { get; set; }

    /// <summary>
    /// Optional polygon fill color override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("polygon-fill-color")]
    public string? PolygonFillColor { get; set; }

    /// <summary>
    /// Optional polygon border opacity override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("polygon-border-opacity")]
    public double? PolygonBorderOpacity { get; set; }

    /// <summary>
    /// Optional polygon fill opacity override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("polygon-fill-opacity")]
    public double? PolygonFillOpacity { get; set; }

    /// <summary>
    /// Optional tile layer visibility override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("show-tile-layer")]
    public bool? ShowTileLayer { get; set; }

    /// <summary>
    /// Optional tile layer URL override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("tile-layer-url")]
    public string? TileLayerUrl { get; set; }

    /// <summary>
    /// Optional source SRID override for inline map options syntax.
    /// </summary>
    [HtmlAttributeName("source-srid")]
    public int? SourceSrid { get; set; }


    /// <summary>
    /// Specifies the editor configuration options.
    /// </summary>
    /// <remarks>
    /// Applies only when <see cref="Mode"/> is set to <c>editor</c>.
    /// Controls available drawing/editing tools, their behavior and render.
    /// </remarks>
    [HtmlAttributeName("editor-options")]
    public GeoEditorOptions? EditorOptions { get; set; }


    /// <summary>
    /// Specifies map mode: display-only <c>map</c> or geoman-based <c>editor</c>.
    /// </summary>
    /// <remarks>
    /// <para><b>Server</b></para>
    /// <list type="bullet">
    /// <item><description><c>map</c> (default): <see cref="IGeoArtist.RenderMap"/>.</description></item>
    /// <item><description><c>editor</c>: <see cref="IGeoArtist.RenderEditor"/>.</description></item>
    /// </list>
    ///
    /// <para><b>Assets (<c>include-assets</c> = true)</b></para>
    /// <list type="bullet">
    /// <item><description><b>Both:</b> Leaflet + GeoArtist scripts/styles.</description></item>
    /// <item><description><b>editor:</b> additionally includes Geoman CSS/JS.</description></item>
    /// </list>
    ///
    /// <para><b>Client runtime</b></para>
    /// <list type="bullet">
    /// <item><description><c>map</c>: map-only pipeline, no Geoman initialization.</description></item>
    /// <item><description><c>editor</c>: Geoman enabled, editing pipeline and layer sync.</description></item>
    /// </list>
    ///
    /// <para><b>Notes</b></para>
    /// <list type="bullet">
    /// <item><description><c>editor</c> has higher cost due to Geoman assets and runtime.</description></item>
    /// </list>
    /// </remarks>
    /// <value>
    /// <list type="bullet">
    /// <item><description><c>map</c> — display-only mode (default)</description></item>
    /// <item><description><c>editor</c> — interactive editing mode</description></item>
    /// </list>
    /// </value>
    [HtmlAttributeName("mode")]
    public string Mode { get; set; } = "map";


    /// <summary>
    /// Specifies whether required CSS and JS assets are included.
    /// </summary>
    /// <remarks>
    /// Set to <c>false</c> when assets are already included globally, for example in a layout.
    /// When <c>true</c> on several components in the same request, each distinct asset URL is still emitted only once.
    /// </remarks>
    [HtmlAttributeName("include-assets")]
    public bool IncludeAssets { get; set; } = true;


    /// <summary>
    /// Processes the TagHelper and renders the GeoArtist component.
    /// </summary>
    /// <param name="context">TagHelper context.</param>
    /// <param name="output">TagHelper output.</param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var resolvedMapOptions = ResolveMapOptions();

        GeoRenderResult renderResult = string.Equals(Mode, "editor", StringComparison.OrdinalIgnoreCase)
            ? _geoComponent.RenderEditor(GeoJson, resolvedMapOptions, EditorOptions)
            : _geoComponent.RenderMap(GeoJson, resolvedMapOptions);

        var htmlContent = _htmlWriter.BuildHtmlContent(renderResult, IncludeAssets);

        output.TagName = null;
        output.Content.SetHtmlContent(htmlContent);
    }

    private GeoMapOptions ResolveMapOptions()
    {
        var options = MapOptions ?? new GeoMapOptions();

        if (!string.IsNullOrWhiteSpace(MapId))
            options.MapId = MapId;

        if (!string.IsNullOrWhiteSpace(Height))
            options.Height = Height;

        if (InitialLat.HasValue)
            options.InitialLat = InitialLat.Value;

        if (InitialLng.HasValue)
            options.InitialLng = InitialLng.Value;

        if (InitialZoom.HasValue)
            options.InitialZoom = InitialZoom.Value;

        if (MaxZoom.HasValue)
            options.MaxZoom = MaxZoom.Value;

        if (FitBounds.HasValue)
            options.FitBounds = FitBounds.Value;

        if (!string.IsNullOrWhiteSpace(PolygonBorderColor))
            options.PolygonBorderColor = PolygonBorderColor;

        if (!string.IsNullOrWhiteSpace(PolygonFillColor))
            options.PolygonFillColor = PolygonFillColor;

        if (PolygonBorderOpacity.HasValue)
            options.PolygonBorderOpacity = PolygonBorderOpacity.Value;

        if (PolygonFillOpacity.HasValue)
            options.PolygonFillOpacity = PolygonFillOpacity.Value;

        if (ShowTileLayer.HasValue)
            options.ShowTileLayer = ShowTileLayer.Value;

        if (!string.IsNullOrWhiteSpace(TileLayerUrl))
            options.TileLayerUrl = TileLayerUrl;

        if (SourceSrid.HasValue)
            options.SourceSrid = SourceSrid.Value;

        return options;
    }
}

