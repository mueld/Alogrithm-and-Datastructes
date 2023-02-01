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
            
        }

        public override void LeaveModelComponentContainerCollection(PropertyInfo elementProperty, IModelComponentContainerCollection variable)
        {
            throw new NotImplementedException();
        }

        public override void LeaveServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            throw new NotImplementedException();
        }

        public override void VisitModelComponentContainer(PropertyInfo elementProperty, ModelComponentContainer modelComponentContainer)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public override void VisitModelVariable(PropertyInfo elementProperty, ModelVariableBase variable)
        {
            throw new NotImplementedException();
        }

        public override void VisitServiceContainer(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            var updatedModelKeys = UpdatedModelKeys(elementProperty, serviceContainer);
            
        }
        private ModelScanningKeys UpdatedModelKeys(PropertyInfo elementProperty, ModelComponentContainer serviceContainer)
        {
            var updatedModelKeys = new ModelScanningKeys();
            updatedModelKeys.ModelPath = GetModelPath(elementProperty, currentKeys.ModelPath);
            updatedModelKeys.ModelKey = BuildModelKey(updatedModelKeys.ModelPath, GetModelName(elementProperty));
            updatedModelKeys.LocalizationNamespace = UpdateLocalizationNamespace(elementProperty, currentKeys.LocalizationNamespace);
            updatedModelKeys.LocalizationPath = BuildLocalizationPath(currentKeys.LocalizationPath, elementProperty);
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

        private static string BuildModelPath(string currentPath, string key ,string seperator = StringConstants.Separator)
        {
            if (string.IsNullOrEmpty(currentPath))
                return key;
            if (string.IsNullOrEmpty(key))
                return currentPath;

            return currentPath + seperator + key;
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
    }

    internal class ModelScanningKeys
    {
        public Guid ModelId;
        public string ModelPath = string.Empty;
        public string ModelKey = string.Empty;
        public string LocalizationPath = string.Empty;
        public string ServiceName = string.Empty;
        public string LocalizationNamespace = string.Empty;
    }
}
