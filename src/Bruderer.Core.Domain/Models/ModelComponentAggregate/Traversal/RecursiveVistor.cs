using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal
{
    /// <summary>
    /// Vistor that needs recursive bahviour. 
    /// For example the visitor needs to know when the model container will be leaved in recusive traversation.(Generating of model link or something else...)
    /// </summary>
    public abstract class RecursiveVistor : Visitor
    {
        public abstract void LeaveServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer);
        public abstract void LeaveModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer);
        public abstract void LeaveModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable);
    }
}
