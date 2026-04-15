# GeoArtist

A lightweight .NET component for rendering and editing **GeoJSON** on an **OpenStreetMap** map using **Leaflet**.

The project is built as a **plug'n'play component** with out-of-the-box support for:
- ASP.NET Core (`TagHelper` / `ViewComponent`)
- WinForms Desktop (`WebView2`)

The repository contains:

| Project | Description |
|-------|-------------|
| **GeoArtist** | Production module. Validation, normalization, transformation, rendering |
| **Demos/WebView** | ASP.NET Core demo host |
| **Demos/Desktop** | WinForms + WebView2 demo host |

#
 

# Features
- ASP.NET Core `TagHelper` and `ViewComponent`
- WinForms WebView2 adapter
- Map mode and editor mode
- GeoJSON normalization to `FeatureCollection`
- Optional SRID transformation support
- Leaflet + OpenStreetMap rendering
- Leaflet-Geoman editor integration
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
 

# Installation (ASP.NET Core)

Add **GeoArtist** to your ASP.NET Core application.

### 1. Add project/package reference

```csharp
<ProjectReference Include="..\GeoArtist\GeoArtist.csproj" />
```

### 2. Register services

`Program.cs`

```csharp
using GeoArtist;

builder.Services.AddGeoArtist();
```

### 3. Add TagHelper import

```cshtml
@addTagHelper *, GeoArtist
```

### 4. Render component

```cshtml
@using GeoArtist.Contracts

@{
    var mapOptions = new GeoMapOptions { MapId = "map-1", Height = "800px" };
    var editorOptions = new GeoEditorOptions();
}

<geo-map
    geo-json='{"type":"FeatureCollection","features":[]}'
    map-options="@mapOptions"
    include-assets="true" />

<geo-map geo-json="@Model.GeoJson" map-options="@mapOptions" include-assets="false" />

<geo-map geo-json="@Model.GeoJson" map-options="@mapOptions" editor-options="@editorOptions" mode="editor" include-assets="true" />
```

#
 

# Asset Loading
By default the component emits required CSS/JS assets:
- Leaflet CSS/JS
- GeoArtist CSS/JS
- Geoman CSS/JS (editor mode)

For multiple component instances on one page:
- It's safe to use `include-assets="true"` on all instances. Duplicates are filtered automatically per HTTP request.
- Note that asset tags are still taken from the render result of each instance. Map mode doesn't include Geoman paths, so at least one editor instance on the page must keep `include-assets="true"` to get Geoman loaded.

#
 

# JavaScript Runtime API
The component exposes runtime methods on `window.GeoArtist`:

```js
window.GeoArtist.UpdateGeoJson(geoJson);
window.GeoArtist.UpdateMapOptions(options);
window.GeoArtist.UpdateEditorOptions(options);
window.GeoArtist.ClearGeoJson();
```

The map/editor instance is reused between updates.

#
 

# Desktop Hosting
Desktop host uses:
- `GeoDesktopWebViewAdapter`
- `WebViewHostBridge`


`Desktop` project uses `GeoDesktopWebViewAdapter` + `WebViewHostBridge` from `GeoArtist.Hosting.Desktop`.
No adapter file copy is required in host apps that reference `GeoArtist`.

- `Map` button renders map mode
- `Editor` button renders editor mode

## Build

```bash
dotnet build GeoArtist.slnx
```

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

