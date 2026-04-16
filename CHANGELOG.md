# Changelog
All notable changes to this project are documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).


## [2.0.0] - 2026-04

### Added
- Modular library architecture split into `Abstractions`, `Contracts`, `Core`, `Rendering`, and host-specific integrations.
- WinForms (WebView2) support via `GeoDesktopWebViewAdapter` and `WebViewHostBridge`.
- New desktop demo application `Demos/Desktop` (WinForms + WebView2).
- ASP.NET Core TagHelper and ViewComponent for seamless web integration.
- Server-side rendering pipeline that returns normalized payloads, HTML fragments, and explicit asset dependency lists.
- Modular client runtime in `wwwroot/js` featuring specialized modules for maps, editors, and host-specific logic.
- Asset configuration model `GeoArtistAssetOptions` with automatic mode-based Leaflet/Geoman asset inclusion and ordering.
- Multi-targeting support for `net8.0` and `net8.0-windows` with platform-specific WebView2 references.
- Open-source project assets including `LICENSE.md`, `CONTRIBUTING.md`, and GitHub `issue`/`PR` templates.


### Changed
- Project renamed from `GeoComponent` to `GeoArtist` including all namespaces and DI registration methods.
- Rendering logic evolved from a monolithic service to a payload-based flow with dedicated validation and SRID transformation.
- Old demo (.net core) host moved to `Demos/WebView` and expanded with real-time update and export scenarios.
- Added NuGet metadata, symbol packages, Source Link, and XML documentation.

### Removed
- Legacy `GeoComponent` facade and `IGeoService` in favor of the new `IGeoArtist` bridge-based architecture.
- Obsolete root-level `WebView` application structure and legacy controllers.

[2.0.0]: https://github.com/Tonkonto/GeoArtist/releases/tag/v2.0.0


## [1.0.0] - 2026-03

### Added
- Initial `GeoComponent` release with ASP.NET Core integration via MVC `GeoMapViewComponent`.
- Core GeoJSON rendering and validation foundation.
- First component models/contracts for map rendering.
- GeoComponent DI registration extensions for host applications.
- Client-side `geoArtist.js` runtime shipped with the component.
- Demo ASP.NET Core host application (`WebView`) with API/controller endpoints and sample map page.

[1.0.0]: https://github.com/Tonkonto/GeoArtist/commit/d22fede061a9085b7d3aa1c627350cda5296860e
