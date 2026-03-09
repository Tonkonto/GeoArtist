using GeoComponent.Core.Interfaces;
using GeoComponent.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GeoComponent;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGeoComponent(this IServiceCollection services)
    {
        services.AddSingleton<IGeoService, GeoService>();
        return services;
    }
}