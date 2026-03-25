using GeoComponent;
using GeoComponent.Contracts;
using GeoComponent.Hosting.Desktop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.WinForms;

namespace Desktop;

public partial class Form1 : Form
{
    private readonly ServiceProvider _serviceProvider;
    private readonly GeoDesktopWebViewAdapter _desktopAdapter;

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
        NodeColor = "#fa580c",
        DragClickTolerance = 15,
        SnapSensitivity = 10,
        NodeSize = 6,
        UiScale = 1.0,
        ActionsVertical = true,
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
        services.AddGeoComponentDesktop();
        _serviceProvider = services.BuildServiceProvider();
        var webViewHostBridge = _serviceProvider.GetRequiredService<WebViewHostBridge>();
        var desktopHostOptions = _serviceProvider.GetRequiredService<GeoDesktopHostOptions>();

        _mapModeButton = new Button { Text = "Map", Width = 80, Height = 32 };
        _editorModeButton = new Button { Text = "Editor", Width = 80, Height = 32 };
        _webView = new WebView2 { Dock = DockStyle.Fill };
        _desktopAdapter = new GeoDesktopWebViewAdapter(_webView, webViewHostBridge, desktopHostOptions);
				BuildLayout();

        Shown += OnShown;
        _mapModeButton.Click += async (_, _) => await LoadMapModeAsync();
        _editorModeButton.Click += async (_, _) => await LoadEditorModeAsync();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _desktopAdapter.Dispose();
        _serviceProvider.Dispose();
        base.OnFormClosed(e);
    }

    private async void OnShown(object? sender, EventArgs e)
    {
        await _desktopAdapter.EnsureReadyAsync();
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

    private async Task LoadMapModeAsync()
    {
        await _desktopAdapter.RenderMapAsync(
            /*geoJson: DemoGeoJson,*/
            mapOptions: _mapOptions);
    }

    private async Task LoadEditorModeAsync()
    {
        await _desktopAdapter.RenderEditorAsync(
            geoJson: DemoGeoJson,
            mapOptions: _mapOptions,
            editorOptions: _editorOptions);
    }
}
