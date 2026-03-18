using GeoComponent.Abstractions;
using GeoComponent.Core.Services;
using GeoComponent.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace GeoComponent;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGeoComponent(this IServiceCollection services)
    {
        services.AddSingleton<IGeoDataSerializer, SystemTextJsonGeoDataSerializer>();
        services.AddSingleton<IGeoRendererBridge, StubGeoRendererBridge>();
        services.AddSingleton<IGeoComponent, GeoComponentService>();

        return services;
    }
}