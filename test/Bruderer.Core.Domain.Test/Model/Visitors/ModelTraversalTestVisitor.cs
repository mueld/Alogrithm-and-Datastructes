using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Test.Model.Visitors
{
    internal class ModelTraversalTestVisitor : Visitor, ITraversalCondition
    {
        public override void VisitModelVariable(PropertyInfo elementProperty, ModelVariableBase variable)
        {
            Variables.Add(variable);
        }

        public override void VisitModelRPC(PropertyInfo elementProperty, ModelRPCBase rpc)
        {
            RPCs.Add(rpc);
        }

        public override void VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer variable)
        {
            Containers.Add(variable);
        }

        public override void VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {

        }
        public override void VisitModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer serviceContainer, int index)
        {

        }

        public override void VisitServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            ServiceContainer.Add(serviceContainer as IServiceModelContainer);
        }

        public bool TraverseContainerModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            return false;
        }

        public bool TraverseModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            return true;
        }

        public bool TraverseServiceContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            return false;
        }



        public List<ModelVariableBase> Variables { get; private set; } = new();
        public List<ModelComponentContainer> Containers { get; private set; } = new();
        public List<ModelRPCBase> RPCs { get; private set; } = new();
        public List<IServiceModelContainer> ServiceContainer { get; private set; } = new();
    }
}
