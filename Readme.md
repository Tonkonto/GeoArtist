# GeoArtist

A lightweight ASP.NET Core module for rendering **geometric shapes (GeoJSON / WKT)** on an **OpenStreetMap** map using **Leaflet**.

The project is designed as a **plug'n'play component** that can be embedded into other ASP.NET Core applications.

The repository contains:

| Project | Description |
|-------|-------------|
| **GeoComponent** | Production-ready module. Geometry parsing, transformation, map rendering |
| **WebView** | Demo / test host for demonstration and testing |

#
 

# Features
- ASP.NET Core **Razor ViewComponent**
- Render **Polygon**, **MultiPolygon**
- Accept **GeoJSON**, **WKT**
- Batch processing
- SRID transformation support
- Leaflet + OpenStreetMap rendering
- Dynamic map update

#
 

# Project Structure
- GeoArtist
    - GeoComponent               ← Production module)
        - Core                          ← Geometry parsing & transformations
         - Facade                       ← Host integration API
         - Models                      ← DTO, Map contracts
         - ViewComponents      ← Razor map component
         - Views                         ← Component view
         - wwwroot                    ← Leaflet integration
 
     - WebView                           ← Demo / test host
          - Controllers
          - Middleware
          - Models
          - Views
          - wwwroot


#
 

# Installation

Add **GeoComponent** to your ASP.NET Core application.

### 1. Add project reference

```csharp
<ProjectReference Include="..\GeoComponent\GeoComponent.csproj" />
```


### 2. Register services

`Program.cs`

```csharp
using GeoComponent;
...
builder.Services.AddGeoComponent();
```

### 3. Add required resources to Layout
```html
<link rel="stylesheet" href="https://unpkg.com/leaflet/dist/leaflet.css" />
<script src="https://unpkg.com/leaflet/dist/leaflet.js"></script>
<script src="~/_content/GeoComponent/js/geoArtist.js"></script>
```
```plain
leaflet.css – Leaflet map styling.
leaflet.js – Leaflet JavaScript map engine used for rendering and interaction.
geoArtist.js – GeoComponent rendering layer. Map initialization, geometry rendering, dynamic updates.
```
#
 

# Rendering a Map
Import component models in the Razor view:

```java
@using GeoComponent.Models
```

Render map component:
```java
@await Component.InvokeAsync("GeoMap", new {
    model = new GeoMap()
})
```
Map container will be created automatically by the component with default *options*.
#
 

# GeoMapOptions
**GeoMapOptions** controls map appearance and behavior.
Providing options is not strictly required. Default values will be used for omitted options.

#### Available options
| option | desciption |
| - | - |
| MapId | Uninque HTML id for the map container. UUIDv4 by default. |
| Height | CSS height of the map container. |
| InitialLat | Initial latitude used when the map is created. |
| InitialLng | Initial longitude used when the map is created.|
| InitialZoom | Initial zoom level. |
| FitBounds | Automatically adjusts map view to fit rendered geometry. |
| PolygonColor | Polygon border color. |
| PolygonOpacity | Polygon opacity. |
| ShowTileLayer | Enables or disables the OpenStreetMap tile layer. |
| TileLayerUrl | Tile provider URL template. |
| TileLayerAttribution | Tile provider attribution text. |
#
 

# Using Geometry Facade
**GeoComponent** provides a **Facade API** that simplifies geometry parsing and map creation.

Example usage inside a controller:
```csharp
using GeoComponent.Facade.Interfaces;
public class HomeController : Controller
{
    private readonly IGeoComponentFacade geo;

    public HomeController(IGeoComponentFacade geo)
    {
        this.geo = geo;
    }

    public IActionResult Index()
    {
        var map = geo.MapFromGeoJson(geoJson);
        return View(map);
    }
}
```

The facade handles:
- Geometry parsing
- SRID transformation
- GeoJSON generation
- Map model creation
#
 

# API Usage
GeoComponent can also be used through API endpoints.

### Request Models
```js
// Parse a single GeoJSON geometry
POST /api/geo/geojson
{
    "geoJson": "{ \"type\":\"Polygon\", \"coordinates\":[...] }"
}

// Parse a batch of GeoJSON geometries
POST /api/geo/geojson/batch
{
    "geoJsonList":
    [
         "{ \"type\":\"Polygon\", \"coordinates\":[...] }",
         "{ \"type\":\"Polygon\", \"coordinates\":[...] }"
    ]
}

// Parse a single WKT geometry
POST /api/geo/wkt
{
    "wkt": "MULTIPOLYGON(((...)))",
    "srid": int
}

// Parse a batch of WKT geometries
POST /api/geo/wkt/batch
{
    "wkt":
    [
        "POLYGON((...))",
        "MULTIPOLYGON(((...)))",
    ]
    "srid": int
}
```

### Response Models
```js
// GeoDataResponse
{
    "geometryType": string,
    "coordinateCount": number,
    "geoJson": string,
    "isValid": boolean
}

// ApiErrorResponse
{
    "error": string,
    "details": string
}
```
#
 

# JavaScript Rendering
Geometry rendering is performed on the client side.

The component exposes a JavaScript API:
```js
window.geoComponent.renderMap(geoArray, options)
```

Example structure of geoArray:
```js
geoArray = [
    {
        geoJson: "{ ... }"
    }
]
```

The map instance is created once and reused for subsequent rendering calls.
New geometries replace the existing layer without recreating the map.
#
 
# Demo App
The repository includes a demo application called WebView.

It demonstrates:
- API endpoints
- Geometry processing
- Map rendering
- Interactive demo tools
#
 
# Used Frameworks and Libraries
- ASP.NET Core
- Razor ViewComponents
- NetTopologySuite
- Leaflet
- OpenStreetMap
#
 