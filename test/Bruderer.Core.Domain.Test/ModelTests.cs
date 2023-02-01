using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Test.TestComponents.Rockets;
using Bruderer.Core.Domain.Test.TestFixtures;
using System.Threading;
using Xunit;

namespace Bruderer.Core.Domain.Test
{
    public class ModelTests : IClassFixture<CoreDomainTestFixture>
    {
        #region fields

        private CoreDomainTestFixture _CoreDomainTestFixture;

        #endregion
        #region ctor

        public ModelTests(CoreDomainTestFixture testFixture)
        {
            _CoreDomainTestFixture = testFixture;
        }

        #endregion

        [Fact]
        public async void ModelStateTest()
        {
            Assert.NotNull(_CoreDomainTestFixture);
            var rocketModel1 = _CoreDomainTestFixture.GetModel<RocketModel1>();

            Assert.True(rocketModel1.State == ModelStateEnumeration.Unknown);

            // Stopping model
            await rocketModel1.StopAsync(CancellationToken.None);
            Assert.True(rocketModel1.State == ModelStateEnumeration.Stopped);

            // Starting model
            await rocketModel1.StartAsync(CancellationToken.None);
            Assert.True(rocketModel1.State == ModelStateEnumeration.Started);
        }

        [Fact]
        public async void ConsumeModelDataTest()
        {
            Assert.NotNull(_CoreDomainTestFixture);
            var rocketModel1 = _CoreDomainTestFixture.GetModel<RocketModel1>();

            var consumeResult = await rocketModel1.ConsumeModelData(new ModelUpdateOptions(), CancellationToken.None);
            Assert.True(consumeResult);
        }

        [Fact]
        public async void ConsumedModelDataTest()
        {
            Assert.NotNull(_CoreDomainTestFixture);
            var rocketModel1 = _CoreDomainTestFixture.GetModel<RocketModel1>();

            var consumeResult = await rocketModel1.ConsumeModelData(new ModelUpdateOptions(), CancellationToken.None);
            Assert.True(consumeResult);

            Assert.True(rocketModel1.EnumerableModelContainers.Count != 0);
            Assert.True(rocketModel1.ServiceModelContainers.Count != 0);
            Assert.True(rocketModel1.RepositoryModelContainers.Count != 0);
            Assert.True(rocketModel1.Variables.Count != 0);
            Assert.True(rocketModel1.RPCs.Count != 0);

            Assert.Equal(7, rocketModel1.EnumerableModelContainers.Count);
            Assert.Equal(4, rocketModel1.ServiceModelContainers.Count);
            Assert.Equal(4, rocketModel1.RepositoryModelContainers.Count);
            Assert.Equal(58, rocketModel1.Variables.Count);
            Assert.Equal(5, rocketModel1.RPCs.Count);
        }

        [Fact]
        public async void DynamicModelCosumeModelDataTest()
        {
            Assert.NotNull(_CoreDomainTestFixture);
            var rocketModel1 = _CoreDomainTestFixture.GetModel<RocketModel1>();

            var consumeResult = await rocketModel1.ConsumeModelData(new ModelUpdateOptions(), CancellationToken.None);
            Assert.True(consumeResult);

            Assert.True(rocketModel1.EnumerableModelContainers.Count != 0);
            Assert.True(rocketModel1.ServiceModelContainers.Count != 0);
            Assert.True(rocketModel1.RepositoryModelContainers.Count != 0);
            Assert.True(rocketModel1.Variables.Count != 0);
            Assert.True(rocketModel1.RPCs.Count != 0);

            Assert.Equal(58, rocketModel1.Variables.Count);

            //add 2 ModelContainer to the model
            rocketModel1.UpperCompositeService.DynPayloadAdaptors.Add(new TestComponents.Rockets.Components.PayloadAdaptorType());
            rocketModel1.UpperCompositeService.DynPayloadAdaptors.Add(new TestComponents.Rockets.Components.PayloadAdaptorType());
            var modelUpdateOptions = new ModelUpdateOptions();
            modelUpdateOptions.OptionServiceNames.Add(rocketModel1.LowerCompositeService.ServiceName);

            consumeResult = await rocketModel1.ConsumeModelData(modelUpdateOptions, CancellationToken.None);
            Assert.True(consumeResult);

            Assert.Equal(58, rocketModel1.Variables.Count);

            modelUpdateOptions.OptionServiceNames.Clear();
            modelUpdateOptions.OptionServiceNames.Add(rocketModel1.UpperCompositeService.ServiceName);
            consumeResult = await rocketModel1.ConsumeModelData(new ModelUpdateOptions(), CancellationToken.None);
            Assert.True(consumeResult);

            Assert.Equal(64, rocketModel1.Variables.Count);

            rocketModel1.UpperCompositeService.DynPayloadAdaptors.Clear();

            consumeResult = await rocketModel1.ConsumeModelData(new ModelUpdateOptions(), CancellationToken.None);
            Assert.True(consumeResult);

            Assert.Equal(58, rocketModel1.Variables.Count);
        }
    }
}
