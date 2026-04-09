using GeoComponent.Abstractions;
using GeoComponent.Contracts;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GeoComponent.Hosting.AspNetCore;

/// <summary>
/// ASP.NET Core TagHelper for rendering GeoArtist component.
/// </summary>
/// <remarks>
/// <para>Renders a <c>Map</c> or <c>Editor</c> instance based on the specified mode and options.
/// Uses <see cref="IGeoComponent"/> to generate the rendering result and injects
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
public sealed class GeoMapTagHelper(IGeoComponent geoComponent, AspNetCoreGeoHtmlWriter htmlWriter) : TagHelper
{
    private readonly IGeoComponent _geoComponent = geoComponent;
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
    /// <item><description><c>map</c> (default): <see cref="IGeoComponent.RenderMap"/>.</description></item>
    /// <item><description><c>editor</c>: <see cref="IGeoComponent.RenderEditor"/>.</description></item>
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
