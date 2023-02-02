using Bruderer.Core.Domain.Attributes.Resolver;
using Bruderer.Core.Domain.Constants;
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

namespace Bruderer.Core.Domain.Models.ModelAggregate.ModelComponentScannerV2.Helpers
{
    public class ModelKeysResolver : RecursiveVistor
    {
        private Stack<ModelScanningKeys> modelkeys = new Stack<ModelScanningKeys>();
        private ModelScanningKeys currentKeys = new ModelScanningKeys();
        public ModelKeysResolver()
        {
            
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

        public override void VisitModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            throw new NotImplementedException();
        }

        public override void VisitModelComponentContainerCollectionItem(PropertyInfo elementProperty, ModelComponentContainer serviceContainer, int index)
        {
            throw new NotImplementedException();
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
                // Special case when scanning option "ModelComponentScanner.WrapSourceObject" is activated
                // Building for the localization key will then be skipped. This assumes that a path for the localization is already given when the scan process is started.
                if (elementProperty.Name == nameof(ScanningWrapperElement.ProxyElement))
                    return (currentPath).ToLower();

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

            modelVariableBase.Display.Link.Path = BuildLocalizationPath(currentKeys.LocalizationPath, elementProperty);
            modelVariableBase.Display.Link.Name = CoreLocalizationNameConstants.DisplayName;
            modelVariableBase.Display.KeyNamespace = currentKeys.LocalizationNamespace;
            modelVariableBase.Display.Value = GetLocalizationDisplay(elementProperty);
            modelVariableBase.Description.Link.Path = BuildLocalizationPath(currentKeys.LocalizationPath, elementProperty);
            modelVariableBase.Description.Link.Name = CoreLocalizationNameConstants.Description;
            modelVariableBase.Description.KeyNamespace = currentKeys.LocalizationNamespace;
            modelVariableBase.Description.Value = GetLocalizationDescription(elementProperty);

            modelVariableBase.ServiceName = currentKeys.ServiceName;
        }

        private void FillEnumerableModelContainerMetaData(PropertyInfo elementProperty, IModelComponentContainerCollection collectionModelContainer)
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
