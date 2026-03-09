using GeoComponent.Core.Interfaces;
using GeoComponent.Core.Services;
using GeoComponent.Facade.Interfaces;
using GeoComponent.Facade.Services;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.IO;

namespace GeoComponent;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGeoComponent(this IServiceCollection services)
    {
        services.AddSingleton<GeoJsonReader>();
        services.AddSingleton<GeoJsonWriter>();
        services.AddSingleton<WKTReader>();

        services.AddSingleton<IGeometryTransformService, GeometryTransformService>();
        services.AddSingleton<IGeoService, GeoService>();
        services.AddSingleton<IGeoComponentFacade, GeoComponentFacade>();

        return services;
    }
}