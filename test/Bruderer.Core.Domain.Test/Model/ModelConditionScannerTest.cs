
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Test.Model.TestModel;
using Bruderer.Core.Domain.Test.TestFixtures;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xunit;

namespace Bruderer.Core.Domain.Test.Model
{
    public class ModelConditionScannerTest : IClassFixture<CoreDomainTestFixture>
    {
        #region fields
        private CoreDomainTestFixture _CoreDomainTestFixture;
        #endregion

        public ModelConditionScannerTest(CoreDomainTestFixture testFixture)
        {
            _CoreDomainTestFixture = testFixture;
        }

        [Fact]
        public void ConditonSetupTest()
        {
            var testModel = new TestModelContainer();
            var modelScanner = new ModelComponentScanner();
            var serviceScopeFactory = _CoreDomainTestFixture.ServiceProvider.GetService<IServiceScopeFactory>();
            var testConditions = new List<ModelCondition>() { new TestCondition(serviceScopeFactory) };

            var scanner = new ModelConditionScanner(modelScanner, testConditions);

            modelScanner.AddFunctionality(scanner);
            modelScanner.Scan(testModel);
            Assert.Contains(testConditions[0].GetDependendModelComponentIds(), v => v.Equals(testModel.Service1.IsEnabled.Id));
            Assert.Equal(10, testConditions[0].GetDependendModelComponentIds().Count);
        }
    }
}
