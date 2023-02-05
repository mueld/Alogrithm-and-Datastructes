using Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Helpers;
using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Test.Model.Traversal.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bruderer.Core.Domain.Test.Model.Visitors
{
    public class Twincat3MetaDataBuilderTest
    {
        [Fact]
        public void EnumerableModelKeyTest()
        {
            var testModel = new TraversalTestModel();

            var modelTraversal = new ModelComponentTraversal();
            var visitor = new Twincat3MetaDataBuilder();

            modelTraversal.TraversePreOrder(testModel, visitor);
            var childreenNumber = 1;
            Assert.Equal($"Service1.Childreens[{childreenNumber}].PLCVersion.".ToUpper(), testModel.Service1.Childreens[0].PLCVersion.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].IsEnabled".ToUpper(), testModel.Service1.Childreens[0].IsEnabled.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].IsMuted".ToUpper(), testModel.Service1.Childreens[0].IsMuted.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].TestRPC".ToUpper(), testModel.Service1.Childreens[0].TestRPC.TwinCAT3Links[0].SymbolKey.Key);

            childreenNumber = 2;

            Assert.Equal($"Service1.Childreens[{childreenNumber}].PLCVersion".ToUpper(), testModel.Service1.Childreens[1].PLCVersion.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].IsEnabled".ToUpper(), testModel.Service1.Childreens[1].IsEnabled.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].IsMuted".ToUpper(), testModel.Service1.Childreens[1].IsMuted.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].TestRPC".ToUpper(), testModel.Service1.Childreens[1].TestRPC.TwinCAT3Links[0].SymbolKey.Key);

            Assert.Equal($"Service1.State".ToUpper(), testModel.Service1.State.TwinCAT3Links[0].SymbolKey.Key);



            childreenNumber = 1;
            Assert.Equal($"Service2.Childreens[{childreenNumber}].PLCVersion".ToUpper(), testModel.Service2.Childreens[0].PLCVersion.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].IsEnabled".ToUpper(), testModel.Service2.Childreens[0].IsEnabled.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].IsMuted".ToUpper(), testModel.Service2.Childreens[0].IsMuted.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].TestRPC".ToUpper(), testModel.Service2.Childreens[0].TestRPC.TwinCAT3Links[0].SymbolKey.Key);

            childreenNumber = 2;

            Assert.Equal($"Service2.Childreens[{childreenNumber}].PLCVersion".ToUpper(), testModel.Service2.Childreens[1].PLCVersion.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].IsEnabled".ToUpper(), testModel.Service2.Childreens[1].IsEnabled.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].IsMuted".ToUpper(), testModel.Service2.Childreens[1].IsMuted.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].TestRPC".ToUpper(), testModel.Service2.Childreens[1].TestRPC.TwinCAT3Links[0].SymbolKey.Key);

            Assert.Equal($"Service2.ChildService.Childreens[{childreenNumber}].PLCVersion".ToUpper(), testModel.Service2.ChildService.Childreens[1].PLCVersion.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service2.ChildService.Childreens[{childreenNumber}].IsEnabled".ToUpper(), testModel.Service2.ChildService.Childreens[1].IsEnabled.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service2.ChildService.Childreens[{childreenNumber}].IsMuted".ToUpper(), testModel.Service2.ChildService.Childreens[1].IsMuted.TwinCAT3Links[0].SymbolKey.Key);
            Assert.Equal($"Service2.ChildService.Childreens[{childreenNumber}].TestRPC".ToUpper(), testModel.Service2.ChildService.Childreens[1].TestRPC.TwinCAT3Links[0].SymbolKey.Key);


            Assert.Equal($"Service2.State".ToUpper(), testModel.Service2.State.TwinCAT3Links[0].SymbolKey.Key);

        }
        [Fact]
        public void ModelPathAttributeTest()
        {
            var testModel = new TraversalTestModel();

            var modelTraversal = new ModelComponentTraversal();
            var visitor = new Twincat3MetaDataBuilder();

            modelTraversal.TraversePreOrder(testModel, visitor);
        }

        [Fact]
        public void ModelNameAttributeTest()
        {
            var testModel = new TraversalTestModel();

            var modelTraversal = new ModelComponentTraversal();
            var visitor = new Twincat3MetaDataBuilder();

            modelTraversal.TraversePreOrder(testModel, visitor);
        }

        [Fact]
        public void SamplingRateAttributeTest()
        {
            var testModel = new TraversalTestModel();

            var modelTraversal = new ModelComponentTraversal();
            var visitor = new Twincat3MetaDataBuilder();

            modelTraversal.TraversePreOrder(testModel, visitor);

            Assert.Equal(ModelVariableSamplingRateEnumeration.ms50, testModel.Service1.PLCVersion.TwinCAT3Links[0].SamplingRate);
            Assert.Equal(ModelVariableSamplingRateEnumeration.ms50, testModel.Service1.IsMuted.TwinCAT3Links[0].SamplingRate);
            Assert.Equal(ModelVariableSamplingRateEnumeration.ms200, testModel.Service1.Childreens[0].IsMuted.TwinCAT3Links[0].SamplingRate);
            Assert.Equal(ModelVariableSamplingRateEnumeration.None, testModel.Service1.Childreens[0].TestRPC.TwinCAT3Links[0].SamplingRate);


            Assert.Equal(ModelVariableSamplingRateEnumeration.ms1000, testModel.Service2.PLCVersion.TwinCAT3Links[0].SamplingRate);
            Assert.Equal(ModelVariableSamplingRateEnumeration.ms1000, testModel.Service2.IsMuted.TwinCAT3Links[0].SamplingRate);
            Assert.Equal(ModelVariableSamplingRateEnumeration.ms200, testModel.Service2.Childreens[0].IsMuted.TwinCAT3Links[0].SamplingRate);
            Assert.Equal(ModelVariableSamplingRateEnumeration.None, testModel.Service2.Childreens[0].TestRPC.TwinCAT3Links[0].SamplingRate);
            Assert.Equal(ModelVariableSamplingRateEnumeration.ms1000, testModel.Service2.State.TwinCAT3Links[0].SamplingRate);

            Assert.Equal(ModelVariableSamplingRateEnumeration.ms1000, testModel.Service2.ChildService.TwinCAT3Links[0].SamplingRate);
            Assert.Equal(ModelVariableSamplingRateEnumeration.ms1000, testModel.Service2.ChildService.IsMuted.TwinCAT3Links[0].SamplingRate);
            Assert.Equal(ModelVariableSamplingRateEnumeration.ms1000, testModel.Service2.ChildService.State.TwinCAT3Links[0].SamplingRate);
        }

    }
}
