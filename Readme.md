# GeoArtist

GeoArtist is a reusable geo-rendering component for .NET with two hosting adapters:
- ASP.NET Core (TagHelper / ViewComponent)
- Desktop WinForms (WebView2 demo host)

The rendering/editor runtime lives inside `GeoComponent`. Demo apps are thin hosts.

## Solution Layout

- `GeoComponent`
  - Core contracts and services
  - GeoJSON normalization and validation
  - SRID -> WGS84 transformation pipeline
  - HTML/script rendering bridge
  - ASP.NET Core + Desktop hosting adapters
  - Static assets (`geoArtist.*`)
- `Demos/WebView`
  - ASP.NET Core demo host
  - API endpoints for GeoJSON/WKT normalization samples
- `Desktop`
  - WinForms + WebView2 demo host

## Current Component Capabilities

- Map mode and editor mode via unified payload (`mode: map|editor`)
- GeoJSON input normalization to `FeatureCollection`
- Optional SRID conversion (via `GeoMapOptions.SourceSrid`)
- Editor text/map synchronization
- Leaflet-Geoman integration for draw/edit/delete in editor mode
- Event stream (`geoartist:ready`, `geoartist:geojsonChanged`, etc.)

## Installation (ASP.NET Core)

1. Add project/package reference to `GeoComponent`.
2. Register services:

```csharp
using GeoComponent;

builder.Services.AddGeoComponent();
```

3. Add TagHelper import:

```cshtml
@addTagHelper *, GeoComponent
```

4. Render map/editor:

```cshtml
@using GeoComponent.Contracts

@{
    var mapOptions = new GeoMapOptions { MapId = "map-1", Height = "480px" };
    var editorOptions = new GeoEditorOptions { Enabled = true };
}

<geo-map
    geo-json='{"type":"FeatureCollection","features":[]}'
    map-options="@mapOptions"
    include-assets="true">
</geo-map>

<geo-map
    geo-json='{"type":"FeatureCollection","features":[]}'
    map-options="@mapOptions"
    editor-options="@editorOptions"
    mode="editor"
    include-assets="false">
</geo-map>
```

## Asset Strategy

Default asset set is configured in `GeoComponentAssetOptions`:
- Leaflet CSS/JS (CDN)
- GeoArtist CSS/JS (static web assets)
- Leaflet-Geoman CSS/JS for editor mode (CDN)

If you have multiple components on one page, include assets once (`include-assets="true"`) and disable for subsequent instances (`include-assets="false"`).

## Desktop Demo

`Desktop` project uses `GeoDesktopWebViewAdapter` + `WebViewHostBridge` from `GeoComponent.Hosting.Desktop`.
No adapter file copy is required in host apps that reference `GeoComponent`.

- `Map` button renders map mode
- `Editor` button renders editor mode

## Build

```bash
dotnet build GeoArtist.slnx
```

## Notes

- `GeoComponent` is multi-targeted (`net8.0`, `net8.0-windows`).
- `Desktop` target is `net8.0-windows` and requires WebView2 runtime on the machine.
