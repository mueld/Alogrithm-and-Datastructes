using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Test.Model.TestModel;
using Bruderer.Core.Domain.Test.Model.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Bruderer.Core.Domain.Test.Model
{
    public class ModelComponentTraversalTest
    {
        [Fact]
        public void VisitAllModelVariableTest()
        {
            var testModel = new TestModelContainer();
            
            var modelTraversal = new ModelComponentTraversal();
            var visitor = new ModelTraversalTestVisitor();
            
            modelTraversal.PreOrder(testModel, visitor);
           
            Assert.Equal(16, visitor.Variables.Count);
            Assert.Equal(4, visitor.ServiceContainer.Count);
            Assert.Empty(visitor.Containers);
            Assert.NotEmpty(visitor.RPCs);
            Assert.Equal(4, visitor.RPCs.Count);

            visitor = new ModelTraversalTestVisitor();
            modelTraversal.PreOrder(testModel, visitor, visitor);
            Assert.Empty(visitor.Variables);
            Assert.Empty(visitor.ServiceContainer);
            Assert.Empty(visitor.RPCs);
        }
    }
}
