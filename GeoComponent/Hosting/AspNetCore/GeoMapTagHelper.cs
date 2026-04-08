using GeoComponent.Abstractions;
using GeoComponent.Contracts;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GeoComponent.Hosting.AspNetCore;

/// <summary>
/// ASP.NET Core TagHelper for rendering GeoArtist map or editor component.
/// </summary>
/// <remarks>
/// Renders a map or editor instance based on the specified mode and options.
/// Uses <see cref="IGeoComponent"/> to generate the rendering result and injects
/// the resulting HTML into the Razor output.
///
/// Supported modes:
/// <list type="bullet">
/// <item><description><c>map</c> — read-only map</description></item>
/// <item><description><c>editor</c> — interactive editor with drawing tools</description></item>
/// </list>
///
/// Example usage:
/// <code>
/// &lt;geo-map
///     geo-json="@Model.GeoJson"
///     mode="editor"
///     include-assets="true" /&gt;
/// </code>
/// </remarks>
/// <remarks>
/// Initializes a new instance of <see cref="GeoMapTagHelper"/>.
/// </remarks>
[HtmlTargetElement("geo-map")]
public sealed class GeoMapTagHelper(IGeoComponent geoComponent, AspNetCoreGeoHtmlWriter htmlWriter) : TagHelper
{
    private readonly IGeoComponent _geoComponent = geoComponent;
    private readonly AspNetCoreGeoHtmlWriter _htmlWriter = htmlWriter;


    /// <summary>
    /// Get/set GeoJSON string to be rendered on the map.
    /// </summary>
    /// <remarks>
    /// Can be null or empty. In this case, an empty map/editor will be rendered.
    /// </remarks>
    [HtmlAttributeName("geo-json")]
    public string? GeoJson { get; set; }


    /// <summary>
    /// Get/set map configuration options.
    /// </summary>
    /// <remarks>
    /// Includes map id, dimensions, initial coordinates, zoom level and tile settings.
    /// If not provided, default options will be used.
    /// </remarks>
    [HtmlAttributeName("map-options")]
    public GeoMapOptions? MapOptions { get; set; }


    /// <summary>
    /// Get/set editor configuration options.
    /// </summary>
    /// <remarks>
    /// Applies only when <see cref="Mode"/> is set to <c>editor</c>.
    /// Controls available drawing/editing tools and permissions.
    /// </remarks>
    [HtmlAttributeName("editor-options")]
    public GeoEditorOptions? EditorOptions { get; set; }


    /// <summary>
    /// Get/set rendering mode.
    /// </summary>
    /// <remarks>
    /// Supported values:
    /// <list type="bullet">
    /// <item><description><c>map</c> — read-only mode (default)</description></item>
    /// <item><description><c>editor</c> — interactive editing mode</description></item>
    /// </list>
    /// </remarks>
    [HtmlAttributeName("mode")]
    public string Mode { get; set; } = "map";


    /// <summary>
    /// Get/set a value indicating whether required CSS and JS assets should be included.
    /// </summary>
    /// <remarks>
    /// Set to <c>false</c> if assets are already included globally (e.g. in layout).
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
        GeoRenderResult renderResult = string.Equals(Mode, "editor", StringComparison.OrdinalIgnoreCase)
            ? _geoComponent.RenderEditor(GeoJson, MapOptions, EditorOptions)
            : _geoComponent.RenderMap(GeoJson, MapOptions);

        var htmlContent = _htmlWriter.BuildHtmlContent(renderResult, IncludeAssets);

        output.TagName = null;
        output.Content.SetHtmlContent(htmlContent);
    }
}
