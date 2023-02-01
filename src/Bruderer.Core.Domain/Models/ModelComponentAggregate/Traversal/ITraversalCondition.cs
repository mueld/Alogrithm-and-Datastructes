using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal
{
    public interface ITraversalCondition
    {
        /// <summary>
        /// Determines whether the container should be traversed. 
        /// </summary>
        /// <returns>
        /// True if container sould be traversed, false otherwise .
        /// </returns>
        bool TraverseContainerModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer container);

        /// <summary>
        /// Determines whether the container collection should be traversed. 
        /// </summary>
        /// <returns>
        /// True if container collection should be traversed, false otherwise .
        /// </returns>
        bool TraverseModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable);

        /// <summary>
        /// Determines whether the service container should be traversed. 
        /// </summary>
        /// <returns>
        /// True if service container should be traversed, false otherwise .
        /// </returns>
        bool TraverseServiceContainer(PropertyInfo elementProperty, ModelComponentContainer container);
    }
}
