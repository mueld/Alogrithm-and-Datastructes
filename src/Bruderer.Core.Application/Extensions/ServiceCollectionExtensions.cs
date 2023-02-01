using Bruderer.Core.Application.Interfaces;
using Bruderer.Core.Application.IoC;
using Bruderer.Core.Infrastructure.IoC;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bruderer.Core.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBrudererCoreComponents(this IServiceCollection services)
        {
            return AddBrudererCoreComponents(services, options => { });
        }

        public static IServiceCollection AddBrudererCoreComponents(this IServiceCollection services, Action<IModelBuilderOptions> options)
        {
            // Register core infrastructure services
            CoreInfrastructureBootstrapper.RegisterServices(services);

            // Register core application services
            CoreApplicationBootstrapper.RegisterServices(services);

            // Setup options
            options(new ModelBuilderOptions(services));

            return services;
        }
    }
}
