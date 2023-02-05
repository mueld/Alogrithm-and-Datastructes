using Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Helpers;
using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Test.Model.TestModel;
using Bruderer.Core.Domain.Test.Model.Traversal.TestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bruderer.Core.Domain.Test.Model.Traversal
{
    public class ModelKeyResolverTraversalTest
    {
        [Fact]
        public void ServcieNameAssignmentTest()
        {
            var testModel = new TraversalTestModel();

            var modelTraversal = new ModelComponentTraversal();
            var visitor = new ModelKeysResolver();

            modelTraversal.TraversePreOrder(testModel, visitor);



            Assert.Equal("Service1", testModel.Service1.ServiceName);
            Assert.Equal("Service2", testModel.Service2.ServiceName);

            Assert.Equal("Service1", testModel.Service1.Childreens[0].ServiceName);
            Assert.Equal("Service1", testModel.Service1.Childreens[0].ServiceName);
            Assert.Equal("Service1", testModel.Service1.Childreens[2].ServiceName);
            Assert.Equal("Service1", testModel.Service1.Childreens[2].IsEnabled.ServiceName);
            Assert.Equal("Service2", testModel.Service2.Childreens[2].IsEnabled.ServiceName);
            Assert.Equal("Service2", testModel.Service2.Childreens[2].TestRPC.ServiceName);


            Assert.Equal("ChildService", testModel.Service1.ChildService.ServiceName);
            Assert.Equal("ChildService", testModel.Service1.ChildService.Childreens[0].ServiceName);

        }

        [Fact]
        public void ModelKeyAssignmentTest()
        {
            var testModel = new TraversalTestModel();

            var modelTraversal = new ModelComponentTraversal();
            var visitor = new ModelKeysResolver();

            modelTraversal.TraversePreOrder(testModel, visitor);

            Assert.Equal("Service1", testModel.Service1.ModelLink.Key);
            Assert.Equal("Service2", testModel.Service2.ModelLink.Key);

            Assert.Equal("Service1.IsMuted", testModel.Service1.IsMuted.ModelLink.Key);
            Assert.Equal("Service1.PLCVersion", testModel.Service1.PLCVersion.ModelLink.Key);


        }

        [Fact]
        public void EnumerableModelKeyTest()
        {
            var testModel = new TraversalTestModel();

            var modelTraversal = new ModelComponentTraversal();
            var visitor = new ModelKeysResolver();

            modelTraversal.TraversePreOrder(testModel, visitor);
            var childreenNumber = 1;
            Assert.Equal($"Service1.Childreens[{childreenNumber}].PLCVersion", testModel.Service1.Childreens[0].PLCVersion.ModelLink.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].IsEnabled", testModel.Service1.Childreens[0].IsEnabled.ModelLink.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].IsMuted", testModel.Service1.Childreens[0].IsMuted.ModelLink.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].TestRPC", testModel.Service1.Childreens[0].TestRPC.ModelLink.Key);

            childreenNumber = 2;

            Assert.Equal($"Service1.Childreens[{childreenNumber}].PLCVersion", testModel.Service1.Childreens[1].PLCVersion.ModelLink.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].IsEnabled", testModel.Service1.Childreens[1].IsEnabled.ModelLink.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].IsMuted", testModel.Service1.Childreens[1].IsMuted.ModelLink.Key);
            Assert.Equal($"Service1.Childreens[{childreenNumber}].TestRPC", testModel.Service1.Childreens[1].TestRPC.ModelLink.Key);

            Assert.Equal($"Service1.State", testModel.Service1.State.ModelLink.Key);



            childreenNumber = 1;
            Assert.Equal($"Service2.Childreens[{childreenNumber}].PLCVersion", testModel.Service2.Childreens[0].PLCVersion.ModelLink.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].IsEnabled", testModel.Service2.Childreens[0].IsEnabled.ModelLink.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].IsMuted", testModel.Service2.Childreens[0].IsMuted.ModelLink.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].TestRPC", testModel.Service2.Childreens[0].TestRPC.ModelLink.Key);

            childreenNumber = 2;

            Assert.Equal($"Service2.Childreens[{childreenNumber}].PLCVersion", testModel.Service2.Childreens[1].PLCVersion.ModelLink.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].IsEnabled", testModel.Service2.Childreens[1].IsEnabled.ModelLink.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].IsMuted", testModel.Service2.Childreens[1].IsMuted.ModelLink.Key);
            Assert.Equal($"Service2.Childreens[{childreenNumber}].TestRPC", testModel.Service2.Childreens[1].TestRPC.ModelLink.Key);

            Assert.Equal($"Service2.ChildService.Childreens[{childreenNumber}].PLCVersion", testModel.Service2.ChildService.Childreens[1].PLCVersion.ModelLink.Key);
            Assert.Equal($"Service2.ChildService.Childreens[{childreenNumber}].IsEnabled", testModel.Service2.ChildService.Childreens[1].IsEnabled.ModelLink.Key);
            Assert.Equal($"Service2.ChildService.Childreens[{childreenNumber}].IsMuted", testModel.Service2.ChildService.Childreens[1].IsMuted.ModelLink.Key);
            Assert.Equal($"Service2.ChildService.Childreens[{childreenNumber}].TestRPC", testModel.Service2.ChildService.Childreens[1].TestRPC.ModelLink.Key);


            Assert.Equal($"Service2.State", testModel.Service2.State.ModelLink.Key);

        }

        [Fact]
        public void LocalisationPathTest()
        {
            var testModel = new TraversalTestModel();

            var modelTraversal = new ModelComponentTraversal();
            var visitor = new ModelKeysResolver();

            modelTraversal.TraversePreOrder(testModel, visitor);

            Assert.Equal($"service1.childreens.item.plcversion.description".ToLower(), testModel.Service1.Childreens[0].PLCVersion.Description.Link.Key);
            Assert.Equal($"service1.childreens.item.Isenabled.description".ToLower(), testModel.Service1.Childreens[0].IsEnabled.Description.Link.Key);
            Assert.Equal($"service1.childreens.item.IsMuted.description".ToLower(), testModel.Service1.Childreens[0].IsMuted.Description.Link.Key);
            Assert.Equal($"service1.childreens.item.TestRPC.description".ToLower(), testModel.Service1.Childreens[0].TestRPC.Description.Link.Key);

            Assert.Equal($"Service2.Childreens.item.plcversion.description".ToLower(), testModel.Service2.Childreens[0].PLCVersion.Description.Link.Key);
            Assert.Equal($"Service2.Childreens.item.IsEnabled.description".ToLower(), testModel.Service2.Childreens[0].IsEnabled.Description.Link.Key);
            Assert.Equal($"Service2.Childreens.item.IsMuted.description".ToLower(), testModel.Service2.Childreens[0].IsMuted.Description.Link.Key);
            Assert.Equal($"Service2.Childreens.item.TestRPC.description".ToLower(), testModel.Service2.Childreens[0].TestRPC.Description.Link.Key);

            Assert.Equal($"Service1.State.description".ToLower(), testModel.Service1.State.Description.Link.Key);

            Assert.Equal($"Service2.ChildService.Childreens.item.PLCVersion.description".ToLower(), testModel.Service2.ChildService.Childreens[1].PLCVersion.Description.Link.Key);
            Assert.Equal($"Service2.ChildService.Childreens.item.IsEnabled.description".ToLower(), testModel.Service2.ChildService.Childreens[1].IsEnabled.Description.Link.Key);
            Assert.Equal($"Service2.ChildService.Childreens.item.IsMuted.description".ToLower(), testModel.Service2.ChildService.Childreens[1].IsMuted.Description.Link.Key);
            Assert.Equal($"Service2.ChildService.Childreens.item.TestRPC.description".ToLower(), testModel.Service2.ChildService.Childreens[1].TestRPC.Description.Link.Key);


            Assert.Equal($"Service2.State.description".ToLower(), testModel.Service2.State.Description.Link.Key);

        }

    }
}
