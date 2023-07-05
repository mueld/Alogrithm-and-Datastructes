using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal
{
    public interface IRecursiveVisitor: IVisitor
    {
       public void LeaveServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer);
       public void LeaveModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer);
       public void LeaveModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable);
       public void LeaveModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer modelContainer, int index);
    }
}
