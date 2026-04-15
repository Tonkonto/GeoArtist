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

3. Add TagHelper import:

```cshtml
@addTagHelper *, GeoArtist
```

4. Render map/editor:

```cshtml
@using GeoArtist.Contracts

@{
    var mapOptions = new GeoMapOptions { MapId = "map-1", Height = "480px" };
    var editorOptions = new GeoEditorOptions { Enabled = true };
}

<geo-map
    geo-json='{"type":"FeatureCollection","features":[]}'
    map-options="@mapOptions"
    include-assets="true" />

<geo-map geo-json="@Model.GeoJson" map-options="@mapOptions" include-assets="true" />

<geo-map geo-json="@Model.GeoJson" map-options="@mapOptions" editor-options="@editorOptions" mode="editor" include-assets="true" />
```

## Asset Strategy

Default asset set is configured in `GeoArtistAssetOptions`:
- Leaflet CSS/JS (CDN)
- GeoArtist CSS/JS (static web assets)
- Leaflet-Geoman CSS/JS for editor mode (CDN)


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

## Notes

- `GeoArtist` is multi-targeted (`net8.0`, `net8.0-windows`).
- `Desktop` target is `net8.0-windows` and requires WebView2 runtime on the machine.

