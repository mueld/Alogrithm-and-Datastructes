using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Test.TestComponents.Rockets;
using Bruderer.Core.Infrastructure.IoC;
using Bruderer.Core.Infrastructure.Test.TestComponents.Rockets;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace Bruderer.Core.Infrastructure.Test.TestFixtures
{
    public class CoreInfrastructureTestFixture : IAsyncLifetime
    {
        #region ctor

        public CoreInfrastructureTestFixture()
        {
            // Setup service collection
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(sp =>
            {
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                //configurationBuilder.AddInMemoryCollection();
                //configurationBuilder.AddJsonFile("appsettings.json");
                return configurationBuilder.Build();
            });
            services.AddMediatR(typeof(CoreInfrastructureTestFixture));
            CoreInfrastructureBootstrapper.RegisterServices(services);

            // Setup service provider
            ServiceProvider = services
                .AddLogging()
                .BuildServiceProvider();
        }

        #endregion
        #region props

        public ServiceProvider ServiceProvider { get; private set; }

        #endregion
        #region methods

        public T GetModel<T>()
            where T : Core.Domain.Models.ModelAggregate.Model
        {
            var serviceScopeFactory = ServiceProvider.GetService<IServiceScopeFactory>();
            var modelFactory = ServiceProvider.GetService<IModelFactory>();
            var loggerFactory = ServiceProvider.GetService<ILoggerFactory>();

            if (typeof(T) == typeof(RocketModel1))
                return new RocketModel1(nameof(RocketModel1), serviceScopeFactory, loggerFactory) as T;

            if (typeof(T) == typeof(Ariane5RocketModel))
                return new Ariane5RocketModel(nameof(Ariane5RocketModel), serviceScopeFactory, modelFactory, loggerFactory) as T;

            return default;
        }

        #endregion

        #region IAsyncLifetime

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
