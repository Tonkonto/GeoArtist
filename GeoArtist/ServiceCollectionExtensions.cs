using GeoArtist.Abstractions;
using GeoArtist.Contracts;
using GeoArtist.Core.Interfaces;
using GeoArtist.Core.Services;
using GeoArtist.Hosting.AspNetCore;
using GeoArtist.Hosting.Desktop;
using GeoArtist.Rendering;
using GeoArtist.Rendering.Html;
using GeoArtist.Rendering.Scripts;
using Microsoft.Extensions.DependencyInjection;

namespace GeoArtist;

/// <summary>
/// Extension methods to register GeoArtist services, asset options, and host helpers in <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    // ===== Public Methods =====
    /// <summary>
    /// Registers GeoArtist with default configuration.
    /// </summary>
    /// <param name="services">The application’s service collection.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddGeoArtist(this IServiceCollection services)
    {
        services.Configure<GeoArtistAssetOptions>(_ => { });
        return AddGeoArtistCore(services);
    }

    /// <summary>
    /// Registers GeoArtist with custom asset paths configuration.
    /// </summary>
    /// <param name="services">The application’s service collection.</param>
    /// <param name="configureAssets">Delegate that mutates <see cref="GeoArtistAssetOptions"/> (CDN URLs, static file paths).</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddGeoArtist(this IServiceCollection services, Action<GeoArtistAssetOptions> configureAssets)
    {
        services.Configure(configureAssets);
        return AddGeoArtistCore(services);
    }

    /// <summary>
    /// Registers GeoArtist with desktop host defaults.
    /// The desktop host uses a virtual HTTPS origin for local static assets.
    /// </summary>
    /// <param name="services">The application’s service collection.</param>
    /// <param name="configureDesktop">Optional delegate to set <see cref="GeoDesktopHostOptions"/> (virtual host name, origin).</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddGeoArtistDesktop(this IServiceCollection services, Action<GeoDesktopHostOptions>? configureDesktop = null)
    {
        var desktopOptions = new GeoDesktopHostOptions();
        configureDesktop?.Invoke(desktopOptions);

        services.AddSingleton(desktopOptions);

        services.Configure<GeoArtistAssetOptions>(assets =>
        {
            GeoDesktopAssetDefaults.Apply(assets, desktopOptions);
        });

        return AddGeoArtistCore(services);
    }

    // ===== Internal Funcs =====
    private static IServiceCollection AddGeoArtistCore(IServiceCollection services)
    {
        services.AddSingleton<IGeometryTransformService, GeometryTransformService>();
        services.AddSingleton<GeoJsonValidationService>();
        services.AddSingleton<IGeoDataSerializer, SystemTextJsonGeoDataSerializer>();

        services.AddSingleton<HtmlTemplateBuilder>();
        services.AddSingleton<HtmlDocumentBuilder>();
        services.AddSingleton<ScriptBootstrapBuilder>();

        services.AddSingleton<IGeoRendererBridge, GeoRendererBridge>();
        services.AddSingleton<IGeoArtist, GeoArtistService>();

        services.AddSingleton<AspNetCoreGeoHtmlWriter>();
        services.AddSingleton<WebViewHostBridge>();

        return services;
    }
}

