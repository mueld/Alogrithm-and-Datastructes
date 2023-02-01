using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Infrastructure.Test.TestComponents.Rockets;
using Bruderer.Core.Infrastructure.Test.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bruderer.Core.Infrastructure.Test
{
    public class ModelFactoryTest : IClassFixture<CoreInfrastructureTestFixture>
    {
        #region fields

        private CoreInfrastructureTestFixture _CoreInfrastructureTestFixture;

        #endregion
        #region ctor

        public ModelFactoryTest(CoreInfrastructureTestFixture testFixture)
        {
            _CoreInfrastructureTestFixture = testFixture;
        }

        #endregion

        [Fact]
        public void AddingModelTest()
        {
            Assert.NotNull(_CoreInfrastructureTestFixture);
            var modelFactory = _CoreInfrastructureTestFixture.ServiceProvider.GetService<IModelFactory>();

            modelFactory.AddModel<IAriane5RocketModel, Ariane5RocketModel>();

            var model = modelFactory.GetModel<IAriane5RocketModel>();
            Assert.NotNull(model);
            Assert.NotNull(modelFactory.GetModel(model.Id));
        }
    }
}
