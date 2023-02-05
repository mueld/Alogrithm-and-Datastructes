using Bruderer.Core.Domain.Attributes.Resolver;
using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Models.ModelComponentAggregate.Traversal;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Helpers
{
    public class ModelKeysResolver : RecursiveVistor
    {
        private Stack<ModelScanningKeys> modelkeys = new Stack<ModelScanningKeys>();
        private ModelScanningKeys currentKeys = new ModelScanningKeys();
        private string enumerableBaseModelPath = string.Empty;
        private string baseLocalizationPath = string.Empty;

        public ModelKeysResolver() {
        currentKeys = new ModelScanningKeys();
            modelkeys.Push(currentKeys); 
        }



        public ModelKeysResolver(ModelComponentContainer rootNode)
        {
            var rootNodeMetaData = new ModelScanningKeys();
            rootNodeMetaData.ModelPath = rootNode.ModelLink.Path;
            rootNodeMetaData.ModelKey = rootNode.ModelLink.Key;
            rootNodeMetaData.ServiceName = rootNode.ServiceName;
            rootNodeMetaData.ElementName = rootNode.ModelLink.Name;
            rootNodeMetaData.LocalizationPath = rootNode.Description.Link.Path;
            currentKeys = rootNodeMetaData;
            modelkeys.Push(currentKeys);
        }

        public override void LeaveModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            modelkeys.Pop();
            currentKeys = modelkeys.Peek();
        }

        public override void LeaveModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            modelkeys.Pop();
            currentKeys = modelkeys.Peek();
        }
        
        public override void LeaveModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer modelContainer, int index)
        {
            modelkeys.Pop();
            currentKeys = modelkeys.Peek();
        }

        public override void LeaveServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            modelkeys.Pop();
            currentKeys = modelkeys.Peek();
        }

        public override void VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            UpdatedModelKeys(elementProperty);
            FillContainerMetaData(elementProperty, modelComponentContainer);
            PostUpdateModelKeys(elementProperty);
        }

        public override void VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection collection)
        {
            UpdatedModelKeys(elementProperty);
            var collectionCount = (int)elementProperty.PropertyType.GetProperty("Count").GetValue(collection, null);
            //FillEnumerableModelContainerMetaData(elementProperty, collection, collectionCount);
            enumerableBaseModelPath = GetModelPath(elementProperty, currentKeys.ModelPath);
            baseLocalizationPath = currentKeys.LocalizationPath;
            modelkeys.Push(currentKeys);
        }

        public override void VisitModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer modelContainer, int index)
        {
            var updatedModelKeys = new ModelScanningKeys(currentKeys);
            updatedModelKeys.ElementName = currentKeys.ElementName + GetEnumerableString(index + 1);
            updatedModelKeys.ModelKey = BuildModelPath(enumerableBaseModelPath, updatedModelKeys.ElementName);
            //updatedModelKeys.LocalizationNamespace = UpdateLocalizationNamespace(elementProperty, currentKeys.LocalizationNamespace);
            updatedModelKeys.LocalizationPath = BuildLocalizationPath(currentKeys.LocalizationPath, elementProperty);

            currentKeys = updatedModelKeys;
            modelkeys.Push(currentKeys);

            FillContainerMetaData(elementProperty, modelContainer);

            modelContainer.EnumerationIndex = index;
            currentKeys.ModelPath = currentKeys.ModelKey;
        }

        public override void VisitModelRPC(PropertyInfo elementProperty, ModelRPCBase rpc)
        {
            UpdatedModelKeys(elementProperty);
            FillRPCMetaData(elementProperty, rpc);
            currentKeys = modelkeys.Peek();
            
        }

        public override void VisitModelVariable(PropertyInfo elementProperty, ModelVariableBase variable)
        {
            UpdatedModelKeys(elementProperty);
            FillVariableMetaData(elementProperty, variable);
            currentKeys = modelkeys.Peek();
        }

        public override void VisitServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            UpdatedModelKeys(elementProperty);
            currentKeys.ServiceName = elementProperty.Name;
            FillContainerMetaData(elementProperty, serviceContainer);
            PostUpdateModelKeys(elementProperty);
        }


        private void UpdatedModelKeys(PropertyInfo elementProperty)
        {
            var updatedModelKeys = new ModelScanningKeys(currentKeys);
            updatedModelKeys.ElementName = GetModelName(elementProperty);
            updatedModelKeys.ModelPath = GetModelPath(elementProperty, currentKeys.ModelPath);
            updatedModelKeys.ModelKey = BuildModelKey(updatedModelKeys.ModelPath, GetModelName(elementProperty));
            updatedModelKeys.LocalizationNamespace = UpdateLocalizationNamespace(elementProperty, currentKeys.LocalizationNamespace);
            updatedModelKeys.LocalizationPath = BuildLocalizationPath(currentKeys.LocalizationPath, elementProperty);
            currentKeys =  updatedModelKeys;
        }
        
        private void PostUpdateModelKeys(PropertyInfo elementProperty)
        {
            var updatedModelKeys = new ModelScanningKeys(currentKeys);
            updatedModelKeys.ModelPath = BuildModelPath(currentKeys.ModelPath, currentKeys.ElementName);
            modelkeys.Push(updatedModelKeys);
            currentKeys = updatedModelKeys;
        }


        private  static string GetEnumerableString(int index, string startArray = StringConstants.StartArray, string endArray = StringConstants.EndArray)
        {
            return startArray + index.ToString() + endArray;
        }
        private static string GetModelPath(PropertyInfo elementProperty, string currentPath)
        {
            var modelPath = AttributeResolver.GetModePathAttribute(elementProperty);
            if (string.IsNullOrEmpty(modelPath))
                modelPath = currentPath;

            return modelPath;

        }

        private static string GetModelName(PropertyInfo elementProperty)
        {
            var modelName = AttributeResolver.GetModeNameAttribute(elementProperty);
            if (string.IsNullOrEmpty(modelName))
            {
                if (elementProperty.Name == nameof(ScanningWrapperElement.ProxyElement))
                    modelName = string.Empty;
                else
                    modelName = elementProperty.Name;
            }

            return modelName;
        }

        private static string BuildModelPath(string currentPath, string elementName ,string seperator = StringConstants.Separator)
        {
            if (string.IsNullOrEmpty(currentPath))
                return elementName;
            if (string.IsNullOrEmpty(elementName))
                return currentPath;

            return currentPath + seperator + elementName;
        }


        private static string BuildModelKey(string currentPath, string elementName, string seperator = StringConstants.Separator)
        {
            if (string.IsNullOrEmpty(currentPath))
                return elementName;
            if (string.IsNullOrEmpty(elementName))
                return currentPath;

            return currentPath + seperator + elementName;
        }

        private static string UpdateLocalizationNamespace(PropertyInfo elementProperty, string currentLocalizationNamespace)
        {
            var newLocalizationNamespace = currentLocalizationNamespace;

            // Resolve attribute
            var elementLocalizationNamespace = AttributeResolver.GetLocalizationNamespaceAttribute(elementProperty);
            if (!string.IsNullOrEmpty(elementLocalizationNamespace))
                newLocalizationNamespace = elementLocalizationNamespace;

            if (string.IsNullOrEmpty(newLocalizationNamespace))
                newLocalizationNamespace = CoreLocalizationNamespaceConstants.CoreDefault;

            return newLocalizationNamespace;
        }
        
        private static string BuildLocalizationPath(string currentPath, PropertyInfo elementProperty, string seperator = StringConstants.Separator)
        {
            if (string.IsNullOrEmpty(currentPath))
            {
                return elementProperty.Name.ToLower();
            }
            else
            {
                return (currentPath + seperator + elementProperty.Name).ToLower();
            }
        }
        
        private void FillContainerMetaData(PropertyInfo elementProperty, ModelComponentContainer container)
        {
            container.ModelId = currentKeys.ModelId;

            container.ModelLink.Path = currentKeys.ModelPath;
            container.ModelLink.Name = currentKeys.ElementName;

            var localizationPath = BuildLocalizationPath(currentKeys.LocalizationPath, elementProperty);
            container.Display.Link.Path = localizationPath;
            container.Display.Link.Name = CoreLocalizationNameConstants.DisplayName;
            container.Display.Value = GetLocalizationDisplay(elementProperty);

            container.Display.KeyNamespace = currentKeys.LocalizationNamespace;
            container.Description.Link.Path = localizationPath;
            container.Description.Link.Name = CoreLocalizationNameConstants.Description;
            container.Description.Value = GetLocalizationDescription(elementProperty);

            container.ServiceName = currentKeys.ServiceName;
        }

        private void FillVariableMetaData(PropertyInfo elementProperty, ModelVariableBase modelVariableBase)
        {
            modelVariableBase.ModelId = currentKeys.ModelId;
            modelVariableBase.ModelLink.Path = currentKeys.ModelPath;
            modelVariableBase.ModelLink.Name = currentKeys.ElementName;

            modelVariableBase.Display.Link.Path = currentKeys.LocalizationPath;
            modelVariableBase.Display.Link.Name = CoreLocalizationNameConstants.DisplayName;
            modelVariableBase.Display.KeyNamespace = currentKeys.LocalizationNamespace;
            modelVariableBase.Display.Value = GetLocalizationDisplay(elementProperty);
            modelVariableBase.Description.Link.Path = currentKeys.LocalizationPath;
            modelVariableBase.Description.Link.Name = CoreLocalizationNameConstants.Description;
            modelVariableBase.Description.KeyNamespace = currentKeys.LocalizationNamespace;
            modelVariableBase.Description.Value = GetLocalizationDescription(elementProperty);

            modelVariableBase.ServiceName = currentKeys.ServiceName;
        }

        private void FillEnumerableModelContainerMetaData(PropertyInfo elementProperty, IModelComponentContainerCollection collectionModelContainer, int collectionCount)
        {
            
            collectionModelContainer.ModelLink.Path = currentKeys.ModelPath;
            collectionModelContainer.ModelLink.Name = currentKeys.ElementName;

            collectionModelContainer.Display.Link.Path = BuildLocalizationPath(currentKeys.LocalizationPath, elementProperty);
            collectionModelContainer.Display.Link.Name = CoreLocalizationNameConstants.DisplayName;
            collectionModelContainer.Display.KeyNamespace = currentKeys.LocalizationNamespace;
            collectionModelContainer.Display.Value = GetLocalizationDisplay(elementProperty);
            collectionModelContainer.Description.Link.Path = BuildLocalizationPath(currentKeys.LocalizationPath, elementProperty);
            collectionModelContainer.Description.Link.Name = CoreLocalizationNameConstants.Description;
            collectionModelContainer.Description.KeyNamespace = currentKeys.LocalizationNamespace;
            collectionModelContainer.Description.Value = GetLocalizationDescription(elementProperty);


            var modelContainer = collectionModelContainer as IModelComponentContainer;
            modelContainer.IsEnumerable = true;
            modelContainer.EnumerationCount = collectionCount;
            modelContainer.EnumerationIndex = -1;

            collectionModelContainer.ServiceName = currentKeys.ServiceName;
        }

        private void FillRPCMetaData(PropertyInfo elementProperty,ModelRPCBase modelRPCBase)
        {
            modelRPCBase.ModelId = currentKeys.ModelId;

            modelRPCBase.Display.Link.Path = currentKeys.LocalizationPath;
            modelRPCBase.Display.Link.Name = CoreLocalizationNameConstants.DisplayName;
            modelRPCBase.Display.KeyNamespace = currentKeys.LocalizationNamespace;
            modelRPCBase.Display.Value = GetLocalizationDisplay(elementProperty);
            modelRPCBase.Description.Link.Path = currentKeys.LocalizationPath;
            modelRPCBase.Description.Link.Name = CoreLocalizationNameConstants.Description;
            modelRPCBase.Description.KeyNamespace = currentKeys.LocalizationNamespace;
            modelRPCBase.Description.Value = GetLocalizationDescription(elementProperty);

            modelRPCBase.ModelLink.Path = currentKeys.ModelPath;
            modelRPCBase.ModelLink.Name = currentKeys.ElementName;

            modelRPCBase.ServiceName = currentKeys.ServiceName;
        }

        private static string GetLocalizationDescription(PropertyInfo elementProperty, bool useTypeAttributes = false)
        {
            var elementName = string.Empty; //elementProperty.Name;
            string attributeDescriptionName = string.Empty;

            // Resolve attribute
            if (useTypeAttributes)
                attributeDescriptionName = AttributeResolver.GetDescriptionAttribute(elementProperty.PropertyType);
            else
                attributeDescriptionName = AttributeResolver.GetDescriptionAttribute(elementProperty);

            if (!string.IsNullOrEmpty(attributeDescriptionName))
                elementName = attributeDescriptionName;

            return elementName;
                }
       
        private  static string GetLocalizationDisplay(PropertyInfo elementProperty, bool useTypeAttributes = false)
        {
            var elementName = string.Empty; //elementProperty.Name;
            string attributeDisplayName = string.Empty;

            // Resolve attribute
            if (useTypeAttributes)
                attributeDisplayName = AttributeResolver.GetDisplayNameAttribute(elementProperty.PropertyType);
            else
                attributeDisplayName = AttributeResolver.GetDisplayNameAttribute(elementProperty);

            if (!string.IsNullOrEmpty(attributeDisplayName))
                elementName = attributeDisplayName;

            return elementName;
        }

       
    }

    internal class ModelScanningKeys
    {

        public ModelScanningKeys() { }
        public ModelScanningKeys(ModelScanningKeys keys)
        {
            ModelId = keys.ModelId;
            ElementName = keys.ElementName;
            ModelPath = keys.ModelPath;
            LocalizationPath = keys.LocalizationPath;
            ServiceName = keys.ServiceName;
            LocalizationNamespace = keys.LocalizationNamespace;
        }

        public Guid ModelId;
        public string ElementName = string.Empty;
        public string ModelPath = string.Empty;
        public string ModelKey = string.Empty;
        public string LocalizationPath = string.Empty;
        public string ServiceName = string.Empty;
        public string LocalizationNamespace = string.Empty;
    }
}
