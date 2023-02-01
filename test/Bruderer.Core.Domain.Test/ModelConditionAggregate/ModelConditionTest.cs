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
    public class ModelConditionTest : IClassFixture<CoreDomainTestFixture>
    {
        #region fields

        private CoreDomainTestFixture _CoreDomainTestFixture;

        #endregion

        #region ctor

        public ModelConditionTest(CoreDomainTestFixture testFixture)
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
            var foundCoditions = testConditions;

            Assert.False((bool)testModel.Service1.IsEnabled.GetValue());
            Assert.False(foundCoditions[0].IsFulfilled);

            testModel.Service1.IsEnabled.SetValue(true);
            foundCoditions[0].ModelComponentChanged(testModel.Service1.IsEnabled);

            Assert.True(foundCoditions[0].IsFulfilled);
            Assert.True(testModel.Service1.IsEnabled.IsVisible);

            testModel.Service1.IsEnabled.SetValue(false);
            foundCoditions[0].ModelComponentChanged(testModel.Service1.IsEnabled);

            Assert.False(foundCoditions[0].IsFulfilled);
            Assert.False(testModel.Service1.IsEnabled.IsVisible);

        }
    }
}
