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
    public interface ITraversalCondition
    {

        /// <summary>
        /// Determines whether the container should be visited. if further traversing is needed wihtout visit explicit container, to find a specific container in the root subtree. 
        /// To determines wheterh the container should be traversed further if the current node should not be visited this method will be invoked first.
        /// To determine whether the container should continue to be traversed if the current node is not to be visited, this method is called first
        /// </summary>
        /// <returns>
        /// True if container sould be traversed, false otherwise .
        /// </returns>
        bool VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer container);

        /// <summary>
        /// Determines whether the container should be traversed. 
        /// </summary>
        /// <returns>
        /// True if container sould be traversed, false otherwise .
        /// </returns>
        bool TraverseModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer container);

        /// <summary>
        /// Determines whether the container collection should be visited. if further traversing is needed wihtout visit explicit container, to find a specific container in the root subtree. 
        /// To determine whether the container should continue to be traversed if the current node is not to be visited, this method is called first
        /// </summary>
        /// <returns>
        /// True if container collection should be traversed, false otherwise .
        /// </returns>
        bool VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable);

        /// <summary>
        /// Determines whether the container collection should be traversed. 
        /// </summary>
        /// <returns>
        /// True if container collection should be traversed, false otherwise .
        /// </returns>
        bool TraverseModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable);

        /// <summary>
        /// Determines whether the container collection should be visited. if further traversing is needed wihtout visit explicit container, to find a specific container in the root subtree. 
        /// To determine whether the container should continue to be traversed if the current node is not to be visited, this method is called first
        /// </summary>
        /// <returns>
        /// True if container collection should be traversed, false otherwise .
        /// </returns>
        bool VisitModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer item, int index);

        /// <summary>
        /// Determines whether the container collection should be traversed. 
        /// </summary>
        /// <returns>
        /// True if container collection should be traversed, false otherwise .
        /// </returns>
        bool TraverseModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer item, int index);

        /// <summary>
        /// Determines whether the service container should be visited. if further traversing is needed wihtout visit explicit container, to find a specific container in the root subtree. 
        /// To determine whether the container should continue to be traversed if the current node is not to be visited, this method is called first
        /// </summary>
        /// <returns>
        /// True if service container should be visited, false otherwise .
        /// </returns>
        bool VisitServiceContainer(PropertyInfo elementProperty, ModelComponentContainer container);

        /// <summary>
        /// Determines whether the service container should be traversed. 
        /// </summary>
        /// <returns>
        /// True if service container should be traversed, false otherwise .
        /// </returns>
        bool TraverseServiceContainer(PropertyInfo elementProperty, ModelComponentContainer container);

        /// <summary>
        /// Determines whether the container collection should be visited. if further traversing is needed wihtout visit explicit container, to find a specific container in the root subtree. 
        /// To determine whether the container should continue to be traversed if the current node is not to be visited, this method is called first
        /// </summary>
        /// <returns>
        /// True if container collection should be traversed, false otherwise .
        /// </returns>
        bool VisitModelVariable(PropertyInfo elementProperty, ModelVariableBase variable);

        /// <summary>
        /// Determines whether the service container should be visited. if further traversing is needed wihtout visit explicit container, to find a specific container in the root subtree. 
        /// To determine whether the container should continue to be traversed if the current node is not to be visited, this method is called first
        /// </summary>
        /// <returns>
        /// True if service container should be visited, false otherwise .
        /// </returns>
        bool VisitModelRPC(PropertyInfo elementProperty, ModelRPCBase rpc);
    }
}
