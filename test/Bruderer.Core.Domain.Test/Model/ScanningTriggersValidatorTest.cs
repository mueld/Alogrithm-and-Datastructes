using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Test.TestFixtures;
using System.Collections.Generic;
using Xunit;

namespace Bruderer.Core.Domain.Test.Model
{
    public class ScanningTriggersValidatorTest : IClassFixture<CoreDomainTestFixture>
    {
        [Fact]
        public void CheckRepositroyName()
        {
            var _validator = new ModelScanningTriggerValidator();
            var triggersToTest = new ModelScanningTrigger();
            triggersToTest.RepositoryNames = new List<string> { "API.Services.Blowoutvalve.Valve" };

            var currentProcessedModelLink = "API.Services";
            var repositoryName = "API.Services";
            var serviceName = "";
            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.True(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);

            repositoryName = "API.Services.Blowoutvalve";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.True(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);


            repositoryName = "API.Services.Blowoutvalve.Valve";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.True(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);


            repositoryName = "API.Services.Blowoutvalve.Valve.Test";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.True(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);

        }

        [Fact]
        public void CheckServiceNameTest()
        {
            var _validator = new ModelScanningTriggerValidator();
            var triggersToTest = new ModelScanningTrigger();
            triggersToTest.ServiceNames = new List<string> { "BlowoutvalveService" };

            var currentProcessedModelLink = "API.Services";
            var repositoryName = "";
            var serviceName = "AirCircuitService";
            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.True(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);

            serviceName = "BlowoutvalveService";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.True(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);
        }

        [Fact]
        public void CheckModelLinkKeysTest()
        {
            var _validator = new ModelScanningTriggerValidator();
            var triggersToTest = new ModelScanningTrigger();
            triggersToTest.ModelKeys = new List<string> { "API.Services.ToolService.stNewToolData", "API.Services.ToolService.ProcessData.ActualToolData" };

            var currentProcessedModelLink = "API.Services_";
            var repositoryName = "";
            var serviceName = "";
            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.True(_validator.Breakpoint);

            currentProcessedModelLink = "API.Services.ToolService";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.True(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);


            currentProcessedModelLink = "API.Services.ToolService.stNewToolData";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.True(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);


            currentProcessedModelLink = "API.Services.ToolService.stNewToolData.Test";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.True(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);

            currentProcessedModelLink = "API.Services.ToolService.ProcessData";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.True(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);


            currentProcessedModelLink = "API.Services.ToolService.ProcessData.ActualToolData";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.True(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);

            currentProcessedModelLink = "API.Services.ToolService.ProcessData.ActualToolData.Valve";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.True(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);
        }

        [Fact]
        public void CheckBreakpoint()
        {
            var _validator = new ModelScanningTriggerValidator();
            var triggersToTest = new ModelScanningTrigger();
            triggersToTest.ModelKeys = new List<string> { "API.Services.Blowoutvalve.Valve" };

            var currentProcessedModelLink = "API.Services";
            var repositoryName = "";
            var serviceName = "";
            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.True(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);

            currentProcessedModelLink = "API.Services.Pressforce";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.True(_validator.Breakpoint);


            currentProcessedModelLink = "API.Services.Pressforce.Valve";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.True(_validator.Breakpoint);

            currentProcessedModelLink = "API.Services.Blowoutvalve";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.True(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);


            currentProcessedModelLink = "API.Services.Blowoutvalve.Valve.Test";

            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.True(_validator.ResetContainer);
            Assert.False(_validator.Breakpoint);

        }

        [Fact]
        public void CheckNamespaceTest()
        {
            var _validator = new ModelScanningTriggerValidator();
            var triggersToTest = new ModelScanningTrigger();
            triggersToTest.ModelKeys = new List<string> { "API.Services.Blowoutvalve.Valve" };

            var currentProcessedModelLink = "API.Services_";
            var repositoryName = "";
            var serviceName = "";
            _validator.CheckTiggers(triggersToTest, currentProcessedModelLink, repositoryName, serviceName);
            Assert.False(_validator.CheckNestedContainer);
            Assert.False(_validator.ResetContainer);
            Assert.True(_validator.Breakpoint);
        }
    }
}
