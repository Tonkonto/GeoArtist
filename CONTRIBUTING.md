# Contributing

Thank you for your interest in GeoArtist. This repository ships a reusable GeoJSON map and editor component for .NET (`GeoArtist`) plus demo hosts.

## Getting started

- Install the [.NET 8 SDK](https://dotnet.microsoft.com/download).
- Clone the repository and work from the repository root.

## Build

```bash
dotnet build GeoArtist.slnx
```

## Tests

```bash
dotnet test GeoArtist.slnx
```

## Try the demos

- **ASP.NET Core**: `Demos/WebView`
- **WinForms + WebView2**: `Demos/Desktop` (requires the WebView2 runtime on Windows)

## Packaging (maintainers)

The library project is `GeoArtist/GeoArtist.csproj`. To produce NuGet outputs locally:

```bash
dotnet pack GeoArtist/GeoArtist.csproj -c Release -o artifacts
```

This writes `GeoArtist.*.nupkg` and `GeoArtist.*.snupkg` into `artifacts/`. The package id is `GeoArtist`; namespaces and the primary assembly are `GeoArtist`.

## Pull requests

- Keep changes focused on a single concern and match existing style and naming in touched files.
- Ensure `dotnet build` and `dotnet test` succeed before opening a PR.
- If you change behavior or public API surface, update `CHANGELOG.md` under **Unreleased** (maintainers may adjust the version section at release time).

## Code of conduct

Rules of the Internet
