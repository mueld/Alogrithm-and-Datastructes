using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Test.TestComponents.Rockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace Bruderer.Core.Domain.Test.TestFixtures
{
    public class CoreDomainTestFixture : IAsyncLifetime
    {
        #region ctor

        public CoreDomainTestFixture()
        {
            // Setup service provider
            var services = new ServiceCollection();
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
            var loggerFactory = ServiceProvider.GetService<ILoggerFactory>();
            var serviceScopeFactory = ServiceProvider.GetService<IServiceScopeFactory>();

            if (typeof(T) == typeof(RocketModel1))
                return new RocketModel1(nameof(RocketModel1), serviceScopeFactory, loggerFactory) as T;

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
