using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Test.Model.TestModel;
using Bruderer.Core.Domain.Test.TestComponents.Rockets;
using Bruderer.Core.Domain.Test.TestFixtures;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bruderer.Core.Domain.Test.Model
{
    public class ModelComponentScannerTest
    {
        [Fact]
        public void ScanningTriggersTest()
        {
            var testModel = new TestModelContainer();
            ModelScanningProps modelScanningProps = default;

            var countOfVariables = testModel.Service1.FindComponents<ModelVariableBase>(modelScanningProps);

            var modelScanner = new ModelComponentScanner();
            var scanningTriggers = new ModelScanningTrigger();
            scanningTriggers.ServiceNames = new List<string>() { "Service3", "Service4" };

            modelScanner.ScanningTriggers = scanningTriggers;
            modelScanner.Scan(testModel);

            Assert.Equal(scanningTriggers.ServiceNames.Count, modelScanner.ServiceModelContainers.Count);
            Assert.Equal(8, modelScanner.Variables.Count);
            Assert.Equal(scanningTriggers.ServiceNames.Count, modelScanner.ServiceModelContainers.Where(container => scanningTriggers.ServiceNames.Contains(container.ModelLink.Name)).ToList().Count);
        }

        [Fact]
        public void ScannModelVariablesCount()
        {
            var testModel = new TestModelContainer();
                    

            var modelScanner = new ModelComponentScanner();

            modelScanner.Scan(testModel);

            Assert.Equal(16, modelScanner.Variables.Count);
        }

        [Fact]
        public void ScanningTriggersForNestedSeriveTest()
        {
            CoreDomainTestFixture testModelFactory = new CoreDomainTestFixture();
            var testModel = testModelFactory.GetModel<RocketModel1>();
            var modelScanner = new ModelComponentScanner();

            var scanningTriggers = new ModelScanningTrigger();
            scanningTriggers.ServiceNames = new List<string>() { "NestedService" };
            modelScanner.ScanningTriggers = scanningTriggers;

            modelScanner.Scan(testModel);

            Assert.Single(modelScanner.ServiceModelContainers);
            Assert.Equal(14, modelScanner.Variables.Count);
            Assert.Equal("UpperCompositeService.NestedService", modelScanner.ServiceModelContainers[0].ModelLink.Key);
        }
    }     
}
