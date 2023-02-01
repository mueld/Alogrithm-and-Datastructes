using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Test.Model.TestModel;
using Bruderer.Core.Domain.Test.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bruderer.Core.Domain.Test.ModelConditionAggregate
{
    public class ModelComponentContainerConditionPropsTest : IClassFixture<CoreDomainTestFixture>
    {
        #region fields

        private CoreDomainTestFixture _CoreDomainTestFixture;

        #endregion

        #region ctor

        public ModelComponentContainerConditionPropsTest(CoreDomainTestFixture testFixture)
        {
            _CoreDomainTestFixture = testFixture;
        }

        #endregion

        [Fact]
        public void ModelConditionDepedencieChangedTest()
        {
            var testModel = new TestModelContainer();
            var modelScanner = new ModelComponentScanner();

            var serviceScopeFactory = _CoreDomainTestFixture.ServiceProvider.GetService<IServiceScopeFactory>();
            var testConditions = new List<ModelCondition>() { new TestCondition(serviceScopeFactory) };
            var scanner = new ModelConditionScanner(modelScanner, testConditions);

            modelScanner.AddFunctionality(scanner);
            modelScanner.Scan(testModel);

            var modelComponents = testConditions[0].GetDependendModelComponentIds();
            Assert.Equal(10, modelComponents.Count);
            Assert.Contains(modelComponents, m => m.Equals(testModel.Service2.IsEnabled.Id));
            Assert.Contains(modelComponents, m => m.Equals(testModel.Service2.PLCVersion.Id));
            Assert.Contains(modelComponents, m => m.Equals(testModel.Service2.IsMuted.Id));
            Assert.Contains(modelComponents, m => m.Equals(testModel.Service2.Test.Id));
        }
    }
}
