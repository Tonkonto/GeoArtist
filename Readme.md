# GeoArtist
A self-contained .NET component for rendering and editing **GeoJSON** on an **OpenStreetMap** map using **Leaflet**.

The project is designed as a plug&play component with out-of-the-box support for:
- ASP.NET Core (`TagHelper` / `ViewComponent`)
- WinForms Desktop (`WebView2`)
 
 
The repository contains:

| Project | Description |
|-------|-------------|
| **GeoArtist** | Production library: server-side GeoJSON handling, HTML/bootstrap rendering, modular JS runtime (Leaflet, Geoman) |
| **Demos/WebView** | ASP.NET Core demo host |
| **Demos/Desktop** | WinForms + WebView2 demo host |

#
 

# Features
- ASP.NET Core `TagHelper` and `ViewComponent`
- WinForms WebView2 adapter
- Shapes displaying and editing modes
- GeoJSON normalization to `FeatureCollection`
- SRID transformation support
- Leaflet + OpenStreetMap rendering
- Geoman integration
- Dynamic runtime updates from JavaScript API

#
 

# Project Structure
- GeoArtist
    - Abstractions
    - Contracts
    - Core
    - Hosting
    - Rendering
    - wwwroot
 
- Demos
    - WebView
    - Desktop

#
 

# Installation for ASP.NET Core

### 1. Add the component to your application as a project reference
```html
<ProjectReference Include="..\GeoArtist\GeoArtist.csproj" />
```

### 2. Register services
`Program.cs`

```csharp
using GeoArtist;

builder.Services.AddGeoArtist();
```

### 3. Add TagHelper import if you want to use the component as html tag
```cshtml
@addTagHelper *, GeoArtist
```

### 4. Render the component
```html
@using GeoArtist.Contracts

<!-- Map 1. Using inline TagHelper attribs, geoJson from vm -->
<geo-map
    map-id   = "map-1"
    geo-json = "@Model.GeoJson"
	height   = "400px"
    include-assets = "true" />

<!-- Map 2. Using explicit contract options objects -->
@{  var mapOptions = new GeoMapOptions { MapId = "map-2", Height = "400px" };
    var editorOptions = new GeoEditorOptions { SnapSensitivity = 10, UseGeoJsonTextArea = true }; }
<geo-map
	mode           = "editor"
	map-options    = "@mapOptions"
	editor-options = "@editorOptions"
	include-assets = "true" />

<!-- Map 3. Using default options -->
<geo-map mode="editor" />
```
`mode="editor"` is used to enable interactive editing (Leaflet-Geoman tools + component's editor pipeline).

#
 

# Asset Loading
By default the component automatically emits the required CSS/JS assets:
- Leaflet CSS/JS
- GeoArtist CSS/JS
- Geoman CSS/JS  (if editor mode)

Multiple component instances on one page:
- It's safe to use `include-assets="true"` on all instances. Duplicates are filtered automatically per HTTP request.
- Note that asset tags are still taken from the render result of each instance. Map mode doesn't include Geoman paths, so at least one editor instance on the page must keep `include-assets="true"` to get Geoman loaded.

#
 

# Map and editor modes
| Mode | Use case | Runtime/asset profile |
| - | - | - |
| **Map** | Display-only GeoJSON visualization | Lightweight profile. Leaflet + GeoArtist map runtime |
| **Editor** | Interactive drawing and geometry editing | Heavier profile. Adds Geoman CSS, JS and editor-specific pipeline |

#
 
 
# GeoMapOptions
GeoMapOptions control map layout, Leaflet view, basic styling, and optional coordinate reprojection for the GeoArtist component.
Providing options is not strictly required. Default values will be used for omitted options.
#
 
 
# GeoEditorOptions
GeoEditorOptions define Geoman editor settings, available tools, edit modes, map/drawing behavior, UI scaling, synced GeoJSON textarea control.
Providing options is not strictly required. Default values will be used for omitted options. These settings apply when the component runs in editor mode (`mode="editor"` or `RenderEditorAsync`).
#
 
 
# JavaScript Runtime API
The component exposes runtime methods on `window.GeoArtist`:

```js
window.GeoArtist.UpdateGeoJson(geoJson);
window.GeoArtist.UpdateMapOptions(options);
window.GeoArtist.UpdateEditorOptions(options);
window.GeoArtist.ClearGeoJson();
```
The map instance is reused between updates.

#
 

# Desktop Hosting

GeoArtist provides `GeoDesktopWebViewAdapter` and `WebViewHostBridge` in `GeoArtist.Hosting.Desktop` so a desktop host can drive WebView2 without reimplementing virtual-host setup, `host.html` navigation, or render messaging.

## Installation
### 1. Add project reference
```xml
<ProjectReference Include="..\GeoArtist\GeoArtist.csproj" />
```

### 2. Register services
```csharp
using GeoArtist;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddGeoArtistDesktop(options =>
{
    // Optional host customization
    // options.HostName = "geoartist.local";
});
using var serviceProvider = services.BuildServiceProvider();
```

### 3. Create the adapter on your main form
Inject the IServiceProvider into your main window and initialize the GeoDesktopWebViewAdapter using your WebView2 control instance.
```csharp
using GeoArtist.Contracts;
using GeoArtist.Hosting.Desktop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.WinForms;

public partial class Form : Form
{
    private readonly GeoDesktopWebViewAdapter _geoAdapter;

    public Form(IServiceProvider services)
    {
        InitializeComponent();

        // Resolve dependencies and bind to the WebView2 control
        var bridge = services.GetRequiredService<WebViewHostBridge>();
        var hostOptions = services.GetRequiredService<GeoDesktopHostOptions>();
        _geoAdapter = new GeoDesktopWebViewAdapter(webView, bridge, hostOptions);
    }
}
```

### 4. Initialize the host page
After the form is constructed, before the first render:
```csharp
private async void Form_Load(object? sender, EventArgs e)
{
    await _geoAdapter.EnsureReadyAsync();
}
```

### 5. Render the map
The adapter provides two methods to update the UI. Each call sends a fresh payload to the existing `host.html` and triggers `GeoArtist.initialize`, which rebuilds the layers or editor state from the supplied GeoJSON.
```csharp
private async Task ShowMapAsync()
{
    var mapOptions = new GeoMapOptions { MapId = "map-1", Height = "100%" };
    await _geoAdapter.RenderMapAsync("""{"type":"FeatureCollection","features":[]}""", mapOptions);
}

private async Task ShowEditorAsync()
{
    var mapOptions = new GeoMapOptions();
    var editorOptions = new GeoEditorOptions { SnapSensitivity = 10, UseGeoJsonTextArea = true };
    await _geoAdapter.RenderEditorAsync("""{"type":"FeatureCollection","features":[]}""", mapOptions, editorOptions);
}
```

> The WebView document stays loaded for the lifetime of the host page. Each call refreshes the Leaflet layers from the payload and, in editor mode, rebuilds editor state—without navigating away from `host.html`.

### 6. Disposal
```csharp
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Cleanup the adapter and its internal WebView2 hooks
        _geoAdapter.Dispose();

        // Standard WinForms container for non-visual components (Timer, ImageList, etc.)
        components?.Dispose();
    }

    base.Dispose(disposing);
}
```
#

## Asset management & build workflow

The desktop adapter maps a virtual HTTPS origin to the `wwwroot` folder next to the **host** executable (`AppContext.BaseDirectory`).

#### How assets are served

- **Source:** static files ship with the `GeoArtist` project under `GeoArtist/wwwroot/`.
- **Deployment:** building the host copies them into its output directory, usually `bin/<Configuration>/<TargetFramework>/wwwroot/` (for WebView2 on Windows, `<TargetFramework>` is a `*-windows` TFM).
- **Runtime:** WebView2 loads `host.html` and dependencies from that output folder, not from the GeoArtist source tree.

#### Data flow
Server-side C# builds a `GeoArtistPayload` JSON in memory. `WebViewHostBridge` wraps it; `GeoDesktopWebViewAdapter` sends it with `CoreWebView2.PostWebMessageAsJson` to the page loaded from `host.html` when `RenderMapAsync` or `RenderEditorAsync` runs.
#
 

# Used Frameworks and Libraries
- ASP.NET Core
- Razor (`TagHelper`, `ViewComponent`)
- NetTopologySuite
- ProjNET
- Leaflet
- Leaflet-Geoman
- OpenStreetMap
#
