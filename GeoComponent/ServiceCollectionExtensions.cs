using GeoComponent.Abstractions;
using GeoComponent.Contracts;
using GeoComponent.Core.Interfaces;
using GeoComponent.Core.Services;
using GeoComponent.Hosting.AspNetCore;
using GeoComponent.Hosting.Desktop;
using GeoComponent.Rendering;
using GeoComponent.Rendering.Html;
using GeoComponent.Rendering.Scripts;
using Microsoft.Extensions.DependencyInjection;

namespace GeoComponent;

public static class ServiceCollectionExtensions
{
    // ===== Public Methods =====
    /// <summary>
    /// Registers GeoComponent with default configuration.
    /// </summary>
    /// <param name="services">DI container</param>
    /// <returns>IServiceCollection for chaining</returns>
    public static IServiceCollection AddGeoComponent(this IServiceCollection services)
    {
        services.Configure<GeoComponentAssetOptions>(_ => { });
        return AddGeoComponentCore(services);
    }

    /// <summary>
    /// Registers GeoComponent with custom asset paths configuration.
    /// </summary>
    /// <param name="services">DI container</param>
    /// <param name="configureAssets">Asset configuration delegate</param>
    /// <returns>IServiceCollection for chaining</returns>
    public static IServiceCollection AddGeoComponent(this IServiceCollection services, Action<GeoComponentAssetOptions> configureAssets)
    {
        services.Configure(configureAssets);
        return AddGeoComponentCore(services);
    }

    // ===== Internal Funcs =====
    private static IServiceCollection AddGeoComponentCore(IServiceCollection services)
    {
        services.AddSingleton<IGeometryTransformService, GeometryTransformService>();
        services.AddSingleton<GeoJsonValidationService>();
        services.AddSingleton<IGeoDataSerializer, SystemTextJsonGeoDataSerializer>();

        services.AddSingleton<HtmlTemplateBuilder>();
        services.AddSingleton<HtmlDocumentBuilder>();
        services.AddSingleton<ScriptBootstrapBuilder>();

        services.AddSingleton<IGeoRendererBridge, GeoRendererBridge>();
        services.AddSingleton<IGeoComponent, GeoComponentService>();

        services.AddSingleton<AspNetCoreGeoHtmlWriter>();
        services.AddSingleton<VirtualHostPageProvider>();
        services.AddSingleton<WebViewHostBridge>();

        return services;
    }
}
