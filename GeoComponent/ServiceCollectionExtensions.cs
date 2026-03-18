using GeoComponent.Abstractions;
using GeoComponent.Core.Services;
using GeoComponent.Rendering;
using GeoComponent.Rendering.Assets;
using GeoComponent.Rendering.Html;
using GeoComponent.Rendering.Scripts;
using Microsoft.Extensions.DependencyInjection;

namespace GeoComponent;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGeoComponent(this IServiceCollection services)
    {
        services.AddSingleton<IGeoDataSerializer, SystemTextJsonGeoDataSerializer>();

        services.AddSingleton<GeoAssetManifest>();

        services.AddSingleton<HtmlTemplateBuilder>();
        services.AddSingleton<HtmlDocumentBuilder>();
        services.AddSingleton<ScriptBootstrapBuilder>();

        services.AddSingleton<IGeoRendererBridge, GeoRendererBridge>();
        services.AddSingleton<IGeoComponent, GeoComponentService>();

        return services;
    }
}