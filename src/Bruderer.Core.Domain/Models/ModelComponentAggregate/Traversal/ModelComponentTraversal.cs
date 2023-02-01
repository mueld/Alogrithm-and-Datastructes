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
    public class ModelComponentTraversal
    {

        /// <summary>
        /// Traverse passed object by preorder. 
        /// </summary>
        /// <param name = "modelComponent" > model component to traverse </param>
        /// /// <param name = "visitor" > vistor </param>
        public void PreOrder(object modelComponent, Visitor visitor)
        {
            PreOrder(modelComponent, visitor, new ProxyTraversalCondition());
        }

        /// <summary>
        /// traverses the passed object , using <typeparamref name="ITraversalCondition"/>  to determine whether the subsequent node should be traversed.
        /// </summary>
        /// <param name = "modelComponent" > model component to traverse </param>
        /// <param name = "visitor" > vistor </param>
        /// <param name = "condition" > Condition to determine whether the subsequent node should be traversed. Will be invoked for the corresponding type of the node befor its visit </param>
        public void PreOrder(object modelComponent, Visitor vis, ITraversalCondition condition)
        {
            var elementType = modelComponent.GetType();
            var elementProperties = elementType.GetProperties();

            for (int i = 0; i < elementProperties.Length; i++)
            {
                var elementProperty = elementProperties[i];
                if (!isRelatedType(elementProperty))
                {
                    continue;
                }

                object elementPropertyValue = null;

                try
                {
                    elementPropertyValue = GetPropertyValue(modelComponent, elementProperty);
                }
                catch (Exception e)
                {
                    continue;
                }

                var elementPropertyValueType = elementPropertyValue.GetType();
                var implementedInterfaces = elementPropertyValueType.GetInterfaces();
                //update
                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelComponentContainer)))
                {
                    var modelcomponent = (ModelComponent)elementPropertyValue;
                    ModelComponentContainer modelComponentContainer = modelcomponent as ModelComponentContainer;

                    if(implementedInterfaces.Contains(typeof(IServiceModelContainer)))
                    {
                        if (condition.TraverseServiceContainer(elementProperty, modelComponentContainer))
                        {
                            vis.VisitServiceContainer(elementProperty, modelComponentContainer);
                            PreOrder(modelcomponent, vis, condition);
                        }
                    }
                    else
                    {
                        if (condition.TraverseContainerModelComponentContainer(elementProperty, modelComponentContainer))
                        {
                            vis.VisitModelComponentContainer(elementProperty, modelComponentContainer);
                            PreOrder(modelcomponent, vis, condition);
                        }
                    }   
                }

                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelComponentContainerCollection)) && elementPropertyValueType.IsGenericType)
                {
                    if(condition.TraverseModelComponentContainerCollection(elementProperty, elementPropertyValue as IModelComponentContainerCollection))
                    {
                        TraverseModelCompontContainerCollection(elementProperty, elementPropertyValue, vis, condition);
                    }
                }

                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelVariable)))
                {
                    var modelcomponent = (ModelComponent)elementPropertyValue;
                   ModelVariableBase modeVariabel = modelcomponent as ModelVariableBase;

                    vis.VisitModelVariable(elementProperty, modelcomponent as ModelVariableBase);
                }

                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelRPC)))
                {
                    var modelcomponent = (ModelComponent)elementPropertyValue;
                    vis.VisitModelRPC(elementProperty, modelcomponent as ModelRPCBase);
                }
            }
        }

        /// <summary>
        /// traverses the passed object , using <typeparamref name="ITraversalCondition"/>  to determine whether the subsequent node should be traversed.
        /// </summary>
        /// <param name = "modelComponent" > model component to traverse </param>
        /// <param name = "visitor" > visitor that needs recursive behavior </param>
        /// <param name = "condition" > Condition to determine whether the subsequent node should be traversed. Will be invoked for the corresponding type of the node befor its visit </param>
        public void PreOrder(object modelComponent, RecursiveVistor vis, ITraversalCondition condition)
        {
            var elementType = modelComponent.GetType();
            var elementProperties = elementType.GetProperties();

            for (int i = 0; i < elementProperties.Length; i++)
            {
                var elementProperty = elementProperties[i];
                if (!isRelatedType(elementProperty))
                {
                    continue;
                }

                object elementPropertyValue = null;

                try
                {
                    elementPropertyValue = GetPropertyValue(modelComponent, elementProperty);
                }
                catch (Exception e)
                {
                    continue;
                }

                var elementPropertyValueType = elementPropertyValue.GetType();
                var implementedInterfaces = elementPropertyValueType.GetInterfaces();
                //update
                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelComponentContainer)))
                {
                    var modelcomponent = (ModelComponent)elementPropertyValue;
                    ModelComponentContainer modelComponentContainer = modelcomponent as ModelComponentContainer;

                    if (implementedInterfaces.Contains(typeof(IServiceModelContainer)))
                    {
                        if (condition.TraverseServiceContainer(elementProperty, modelComponentContainer))
                        {
                            vis.VisitServiceContainer(elementProperty, modelComponentContainer);
                            PreOrder(modelcomponent, vis, condition);
                            vis.LeaveServiceContainer(elementProperty, modelComponentContainer);
                        }
                    }
                    else
                    {
                        if (condition.TraverseContainerModelComponentContainer(elementProperty, modelComponentContainer))
                        {
                            vis.VisitModelComponentContainer(elementProperty, modelComponentContainer);
                            PreOrder(modelcomponent, vis, condition);
                            vis.LeaveModelComponentContainer(elementProperty, modelComponentContainer);
                        }
                    }
                }

                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelComponentContainerCollection)) && elementPropertyValueType.IsGenericType)
                {
                    if (condition.TraverseModelComponentContainerCollection(elementProperty, elementPropertyValue as IModelComponentContainerCollection))
                    {
                        TraverseModelCompontContainerCollection(elementProperty, elementPropertyValue, vis, condition);
                        vis.LeaveModelComponentContainerCollection(elementProperty, elementPropertyValue as IModelComponentContainerCollection);
                    }
                }

                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelVariable)))
                {
                    var modelcomponent = (ModelComponent)elementPropertyValue;
                    ModelVariableBase modeVariabel = modelcomponent as ModelVariableBase;

                    vis.VisitModelVariable(elementProperty, modelcomponent as ModelVariableBase);
                }

                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelRPC)))
                {
                    var modelcomponent = (ModelComponent)elementPropertyValue;
                    vis.VisitModelRPC(elementProperty, modelcomponent as ModelRPCBase);
                }
            }
        }

        private object GetPropertyValue(object parentElement, PropertyInfo elementProperty, object[] index = null)
        {
            object elementPropertyValue = null;
            try
            {
                 elementPropertyValue = elementProperty.GetValue(parentElement, index);
                 if (elementPropertyValue != null)
                    return elementPropertyValue;
                 else
                    return TryActivateValue(parentElement, elementProperty);
            }
            catch(Exception e)
            {
                return TryActivateValue(parentElement, elementProperty);
            }
        }

        private object TryActivateValue(object parentElement, PropertyInfo elementProperty)
        {
            var elementPropertyType = elementProperty.PropertyType;
            var activatedValue = Activator.CreateInstance(elementPropertyType);
            if (activatedValue == null)
            {
                throw new Exception($"Value for [{parentElement.GetType().Name}] could not be activated.");
            }

            return TryActivateValue(parentElement, activatedValue, elementProperty);
        }

        private object TryActivateValue(object parentElement, object value, PropertyInfo elementProperty)
        {
            elementProperty.SetValue(parentElement, value);
            return value;
        }

        private bool isRelatedType(PropertyInfo elementType)
        {
            if (elementType.Name == "ParentModelContainer")
                return false;

            var typeInterfaces = elementType.PropertyType.GetInterfaces();
            var IsModelComponentContainer = typeInterfaces.Contains(typeof(IModelComponentContainer));
            var IsModelVariable = typeInterfaces.Contains(typeof(IModelVariable));
            var IsModelContainerCollection = (typeInterfaces.Contains(typeof(IModelComponentContainerCollection)) && elementType.PropertyType.IsGenericType);
            var IsModelRPC = typeInterfaces.Contains(typeof(IModelRPC));
            return IsModelContainerCollection || IsModelVariable || IsModelRPC || IsModelComponentContainer;
        }

        private void TraverseModelCompontContainerCollection(PropertyInfo elementProperty, object elementValue, Visitor vis, ITraversalCondition condition)
        {
            var collection = elementValue as IModelComponentContainerCollection;

            vis.VisitModelComponentContainerCollection(elementProperty, collection);

            // Resolve "Count" prop of the collection
            int collectionCount = collection.Count;
  
            for (int collectionIndex = 0; collectionIndex < collectionCount; collectionIndex++)
            {
                // Get the collection item
                var collectionItemProperty = elementProperty.PropertyType.GetProperty("Item");
                object[] index = { collectionIndex };
                object collectionItem = elementProperty.PropertyType.GetProperty("Item").GetValue(elementValue, index);
                var collectionItemType = collectionItem.GetType();

                if (collectionItemType.GetInterfaces().Contains(typeof(IModelComponentContainer)))
                {
                    vis.VisitModelComponentContainerCollectionItem(collectionItemProperty, collectionItem as ModelComponentContainer, collectionIndex);
                    PreOrder(collectionItem, vis, condition);
                }
            }
        }
    }

    internal class ProxyTraversalCondition : ITraversalCondition
    {
        public bool TraverseContainerModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            return true;
        }

        public bool TraverseModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            return true;
        }

        public bool TraverseServiceContainer(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            return true;
        }
    }
}


