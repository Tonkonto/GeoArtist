using GeoComponent;
using GeoComponent.Contracts;
using GeoComponent.Hosting.Desktop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Desktop;

public partial class Form1 : Form
{
    private const string DesktopAssetsHost = "geoartist.local";

    private readonly ServiceProvider _serviceProvider;
    private readonly WebViewHostBridge _webViewHostBridge;

    private readonly WebView2 _webView;
    private readonly Button _mapModeButton;
    private readonly Button _editorModeButton;

    private readonly GeoMapOptions _mapOptions = new()
    {
        MapId = "desktop-map",
        Height = "calc(100vh - 64px)",
        InitialLat = 42.8746,
        InitialLng = 74.5698,
        InitialZoom = 12,
        FitBounds = true
    };

    private readonly GeoEditorOptions _editorOptions = new()
    {
        Enabled = true,
        AllowPolygon = true,
        AllowRectangle = true,
        AllowPolyline = true,
        AllowMarker = true,
        AllowEdit = true,
        AllowDelete = true,
        AllowTextInputSync = true,
        AutoApplyTextChanges = true
    };

    private const string DemoGeoJson = """
{
  "type": "FeatureCollection",
  "features": [
    {
      "type": "Feature",
      "properties": {
        "source": "desktop"
      },
      "geometry": {
        "type": "Polygon",
        "coordinates": [
          [
            [74.60, 42.87],
            [74.62, 42.87],
            [74.62, 42.89],
            [74.60, 42.89],
            [74.60, 42.87]
          ]
        ]
      }
    }
  ]
}
""";

    public Form1()
    {
        InitializeComponent();

        var services = new ServiceCollection();
        services.AddGeoComponent(options =>
        {
            options.CssPaths =
            [
                $"https://{DesktopAssetsHost}/css/geoArtist.css"
            ];

            options.JsPaths =
            [
                $"https://{DesktopAssetsHost}/js/geoArtist.state.js",
                $"https://{DesktopAssetsHost}/js/geoArtist.events.js",
                $"https://{DesktopAssetsHost}/js/geoArtist.geojson.js",
                $"https://{DesktopAssetsHost}/js/geoArtist.map.js",
                $"https://{DesktopAssetsHost}/js/geoArtist.geoman.js",
                $"https://{DesktopAssetsHost}/js/geoArtist.editor.js",
                $"https://{DesktopAssetsHost}/js/geoArtist.js"
            ];
        });

        _serviceProvider = services.BuildServiceProvider();
        _webViewHostBridge = _serviceProvider.GetRequiredService<WebViewHostBridge>();

        _mapModeButton = new Button { Text = "Map", Width = 80, Height = 32 };
        _editorModeButton = new Button { Text = "Editor", Width = 80, Height = 32 };
        _webView = new WebView2 { Dock = DockStyle.Fill };

        BuildLayout();

        Shown += OnShown;
        _mapModeButton.Click += async (_, _) => await LoadMapModeAsync();
        _editorModeButton.Click += async (_, _) => await LoadEditorModeAsync();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _serviceProvider.Dispose();
        base.OnFormClosed(e);
    }

    private async void OnShown(object? sender, EventArgs e)
    {
        await EnsureWebViewReadyAsync();
        await LoadMapModeAsync();
    }

    private void BuildLayout()
    {
        Text = "GeoArtist Desktop Demo";
        Width = 1280;
        Height = 840;

        var topPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 48,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(8)
        };

        topPanel.Controls.Add(_mapModeButton);
        topPanel.Controls.Add(_editorModeButton);

        Controls.Add(_webView);
        Controls.Add(topPanel);
    }

    private async Task EnsureWebViewReadyAsync()
    {
        if (_webView.CoreWebView2 is not null)
            return;

        await _webView.EnsureCoreWebView2Async();
        var core = _webView.CoreWebView2;

        if (core is null)
            throw new InvalidOperationException("WebView2 initialization failed.");

        core.SetVirtualHostNameToFolderMapping(
            DesktopAssetsHost,
            ResolveDesktopWwwRootPath(),
            CoreWebView2HostResourceAccessKind.Allow);

        _webView.DefaultBackgroundColor = Color.White;
    }

    private static string ResolveDesktopWwwRootPath()
    {
        var outputRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");

        if (Directory.Exists(outputRoot))
            return outputRoot;

        var repoRootFallback = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "GeoComponent", "wwwroot"));

        if (Directory.Exists(repoRootFallback))
            return repoRootFallback;

        throw new DirectoryNotFoundException(
            $"GeoComponent web assets folder was not found. Checked: '{outputRoot}' and '{repoRootFallback}'.");
    }

    private async Task LoadMapModeAsync()
    {
        await EnsureWebViewReadyAsync();

        var html = _webViewHostBridge.BuildMapPage(
            geoJson: DemoGeoJson,
            mapOptions: _mapOptions,
            title: "GeoArtist Desktop - Map");

        _webView.NavigateToString(html);
    }

    private async Task LoadEditorModeAsync()
    {
        await EnsureWebViewReadyAsync();

        var html = _webViewHostBridge.BuildEditorPage(
            geoJson: DemoGeoJson,
            mapOptions: _mapOptions,
            editorOptions: _editorOptions,
            title: "GeoArtist Desktop - Editor");

        _webView.NavigateToString(html);
    }
}
