using GeoArtist.Contracts;

namespace GeoArtist.Hosting.Desktop;

internal static class GeoDesktopAssetDefaults
{
    internal static void Apply(GeoArtistAssetOptions assets, GeoDesktopHostOptions hostOptions)
    {
        assets.CssPaths =
        [
            hostOptions.ToAssetUrl("css/geoArtist.css")
        ];

        assets.JsPaths =
        [
            hostOptions.ToAssetUrl("js/geoArtist.messages.js"),
            hostOptions.ToAssetUrl("js/geoArtist.state.js"),
            hostOptions.ToAssetUrl("js/geoArtist.events.js"),
            hostOptions.ToAssetUrl("js/geoArtist.geojson.js"),
            hostOptions.ToAssetUrl("js/geoArtist.map.js"),
            hostOptions.ToAssetUrl("js/geoArtist.geoman.js"),
            hostOptions.ToAssetUrl("js/geoArtist.editor.js"),
            hostOptions.ToAssetUrl("js/geoArtist.js")
        ];
    }
}
