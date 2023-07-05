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
    public interface IVisitor
    {
        public void VisitModelVariable(PropertyInfo elementProperty, ModelVariableBase variable);
        public void VisitServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer);
        public void VisitModelRPC(PropertyInfo elementProperty, ModelRPCBase rpc);
        public void VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer);
        public void VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable);
        public void VisitModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer serviceContainer, int index);
    }
}
