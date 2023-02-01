using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Test.TestComponents.Rockets;
using Bruderer.Core.Domain.Test.TestComponents.Rockets.Components;
using Bruderer.Core.Domain.Test.TestFixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Bruderer.Core.Domain.Test.ModelComponentContainerAggregate
{
    public class ModelComponentContainerTest: IClassFixture<CoreDomainTestFixture>
    {
        #region fields

        private CoreDomainTestFixture _CoreDomainTestFixture;

        #endregion
        #region ctor

        public ModelComponentContainerTest(CoreDomainTestFixture testFixture)
        {
            _CoreDomainTestFixture = testFixture;
        }
        #endregion

        [Fact]
        public void findGenericModelComponentsTest()
        {
            var rocketModel = _CoreDomainTestFixture.GetModel<RocketModel1>();
             rocketModel.ConsumeModelData(new ModelUpdateOptions(), CancellationToken.None).GetAwaiter().GetResult();

            var foundComponents = rocketModel.UpperCompositeService.FindExplicitContainers<IModelDependency>();
            Assert.Equal(3,foundComponents.Count);
        }
    }
}
