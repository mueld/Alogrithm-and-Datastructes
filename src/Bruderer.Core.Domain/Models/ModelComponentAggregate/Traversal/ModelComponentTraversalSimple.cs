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
    public class ModelComponentTraversalSimple
    {
        public void PreOrder(object modelComponent, VisitorSimple vis)
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
                    elementPropertyValue = elementProperty.GetValue(modelComponent);
                    if (elementPropertyValue == null)
                        continue;
                }
                catch (Exception e)
                {
                    continue;
                }

                var elementPropertyValueType = elementPropertyValue.GetType();
                var implementedInterfaces = elementPropertyValueType.GetInterfaces();
                //update
                if (implementedInterfaces.Contains(typeof(IModelComponentContainer)))
                {
                    var modelcomponent = (ModelComponent)elementPropertyValue;
                    ModelComponentContainer modelComponentContainer = modelcomponent as ModelComponentContainer;
                    vis.VisitModelComponent(elementPropertyValueType, modelComponentContainer);
                    PreOrder(elementPropertyValue, vis);
                }

                if (implementedInterfaces.Contains(typeof(IModelComponentContainerCollection)) && elementPropertyValueType.IsGenericType)
                {
                    TraverseModelCompontContainerCollection(elementProperty, elementPropertyValue, vis);
                }
            }
        }
        private bool isRelatedType(PropertyInfo elementType)
        {
            if (elementType.Name == "ParentModelContainer")
                return false;

            var typeInterfaces = elementType.PropertyType.GetInterfaces();

            return typeInterfaces.Contains(typeof(IModelComponentContainer)) || (typeInterfaces.Contains(typeof(IModelComponentContainerCollection)) && elementType.PropertyType.IsGenericType);

        }

        private void TraverseModelCompontContainerCollection(PropertyInfo elementProperty, object elementValue, VisitorSimple vis)
        {
            var collection = elementValue as IModelComponentContainerCollection;
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
                    vis.VisitModelComponent(collectionItemType, collectionItem as ModelComponent);
                    PreOrder(collectionItem, vis);
                }
            }
        }
    }
}
