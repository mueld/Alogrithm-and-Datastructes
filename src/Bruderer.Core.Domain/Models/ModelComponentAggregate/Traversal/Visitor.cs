using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal
{
    public abstract class Visitor
    {
        public abstract void VisitModelVariable(PropertyInfo elementProperty, ModelVariableBase variable);
        public abstract void VisitServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer);
        public abstract void VisitModelRPC(PropertyInfo elementProperty, ModelRPCBase rpc);
        public abstract void VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer);
        public abstract void VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable);
        public abstract void VisitModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer serviceContainer, int index);
    }
}
