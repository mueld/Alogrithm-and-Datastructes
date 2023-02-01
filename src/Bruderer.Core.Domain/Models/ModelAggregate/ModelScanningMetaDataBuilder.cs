using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Attributes.Resolver;
using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public abstract class ModelScanningMetaDataBuilderBase
    {
        public static string UpdateLocalizationNamespace(PropertyInfo elementProperty, string currentLocalizationNamespace)
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

        protected static ModelIdentityUserRoleEnumeration UpdateVisibilityAuthorization(PropertyInfo elementProperty, ModelIdentityUserRoleEnumeration currentVisibilityAuthorization)
        {
            var newVisibilityAuthorization = currentVisibilityAuthorization;

            // Resolve attribute
            var attributeVisibilityAuthorization = AttributeResolver.GetVisibilityAuthorizationAttribute(elementProperty);
            if (attributeVisibilityAuthorization != ModelIdentityUserRoleEnumeration.Undefined)
                newVisibilityAuthorization = attributeVisibilityAuthorization;

            return newVisibilityAuthorization;
        }

        protected static ModelIdentityUserRoleEnumeration UpdateManipulationAuthorization(PropertyInfo elementProperty, ModelIdentityUserRoleEnumeration currentManipulationAuthorization)
        {
            var newManipulationAuthorization = currentManipulationAuthorization;

            // Resolve attribute
            var attributeManipulationAuthorization = AttributeResolver.GetManipulationAuthorizationAttribute(elementProperty);
            if (attributeManipulationAuthorization != ModelIdentityUserRoleEnumeration.Undefined)
                newManipulationAuthorization = attributeManipulationAuthorization;

            return newManipulationAuthorization;
        }

        protected static ModelRepositoryDataFlags? UpdateRepositoryDataFlags(PropertyInfo elementProperty, ModelRepositoryDataFlags? currentRepositoryDataFlags)
        {
            var newRepositoryDataFlags = currentRepositoryDataFlags;

            // Resolve attribute
            var attributeRepositoryData = AttributeResolver.GetRepositoryDataAttribute(elementProperty);
            if (attributeRepositoryData != null)
            {
                var attributeRepositoryDataResolved = (ModelRepositoryDataFlags)attributeRepositoryData;
                if (attributeRepositoryDataResolved.HasFlag(ModelRepositoryDataFlags.None))
                    newRepositoryDataFlags = ModelRepositoryDataFlags.None;
                else
                    newRepositoryDataFlags = attributeRepositoryDataResolved;

                // Extend Flags?
                //foreach (ModelRepositoryDataFlags flag in Enum.GetValues(typeof(ModelRepositoryDataFlags)))
                //    if (!newRepositoryDataFlags.HasFlag(flag))
                //        newRepositoryDataFlags |= flag;
            }

            return newRepositoryDataFlags;
        }

        protected static bool UpdateIsReadOnly(PropertyInfo elementProperty, ModelScanningProps modelScanningProps)
        {
            var newIsReadOnly = modelScanningProps.Attributes.IsReadOnly;

            // Resolve attribute
            var elementIsReadOnly = AttributeResolver.GetIsReadyOnlyAttribute(elementProperty);
            if (elementIsReadOnly != null)
                newIsReadOnly = (bool)elementIsReadOnly;

            return newIsReadOnly;
        }

        private static List<ScanningTwinCAT3AttrubuteProps> UpdateTwinCAT3Links(PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps)
        {
            var newTwinCAT3Links = new List<ScanningTwinCAT3AttrubuteProps>(currentModelScanningProps.TwinCAT3Links);

            // Resolve "TwinCAT3EntryPointAttribute"
            var twinCAT3EntryPointAttribute = AttributeResolver.GetTwinCAT3EntryPointAttribute(elementProperty);
            if (twinCAT3EntryPointAttribute != null)
            {
                var index = newTwinCAT3Links.FindIndex(link => link.Name == twinCAT3EntryPointAttribute.ModelUserName);
                if (index < 0)
                    newTwinCAT3Links.Add(new ScanningTwinCAT3AttrubuteProps()
                    {
                        Name = twinCAT3EntryPointAttribute.ModelUserName,
                    });
            }

            // Resolve "TwinCAT3SymbolLinkAttribute"
            var twinCAT3SymbolLinkAttribute = AttributeResolver.GetTwinCAT3SymbolLinkAttribute(elementProperty);
            if (twinCAT3SymbolLinkAttribute != null)
            {
                var index = newTwinCAT3Links.FindIndex(link => link.Name == twinCAT3SymbolLinkAttribute.ModelUserName);
                if (index >= 0)
                {
                    var linkStruct = newTwinCAT3Links[index];
                    linkStruct.SymbolPath = twinCAT3SymbolLinkAttribute.SymbolLink;
                    newTwinCAT3Links[index] = linkStruct;
                }
                else
                    newTwinCAT3Links.Add(new ScanningTwinCAT3AttrubuteProps()
                    {
                        Name = twinCAT3SymbolLinkAttribute.ModelUserName,
                        SymbolPath = twinCAT3SymbolLinkAttribute.SymbolLink
                    });
            }

            // Resolve "TwinCAT3SamplingRateAttribute"
            var twinCAT3SamplingRateAttribute = AttributeResolver.GetTwinCAT3SamplingRateAttribute(elementProperty);
            if (twinCAT3SamplingRateAttribute != null)
            {
                var index = newTwinCAT3Links.FindIndex(link => link.Name == twinCAT3SamplingRateAttribute.ModelUserName);
                if (index >= 0)
                {
                    var linkStruct = newTwinCAT3Links[index];
                    linkStruct.SamplingRate = twinCAT3SamplingRateAttribute.SamplingRate;
                    newTwinCAT3Links[index] = linkStruct;
                }
                else
                    newTwinCAT3Links.Add(new ScanningTwinCAT3AttrubuteProps()
                    {
                        Name = twinCAT3SamplingRateAttribute.ModelUserName,
                        SamplingRate = twinCAT3SamplingRateAttribute.SamplingRate
                    });
            }

            // Resolve "TwinCAT3IgnoreAttribute"
            var twinCAT3IgnoreAttribute = AttributeResolver.GetTwinCAT3IgnoreAttribute(elementProperty);
            if (twinCAT3IgnoreAttribute != null)
            {
                var index = newTwinCAT3Links.FindIndex(link => link.Name == twinCAT3IgnoreAttribute.ModelUserName);
                if (index >= 0)
                {
                    var linkStruct = newTwinCAT3Links[index];
                    linkStruct.IsIgnored = twinCAT3IgnoreAttribute.IsEnabled;
                    newTwinCAT3Links[index] = linkStruct;
                }
                else
                    newTwinCAT3Links.Add(new ScanningTwinCAT3AttrubuteProps()
                    {
                        Name = twinCAT3IgnoreAttribute.ModelUserName,
                        IsIgnored = twinCAT3IgnoreAttribute.IsEnabled
                    });
            }

            return newTwinCAT3Links;
        }

        protected static List<ModelTwinCAT3Link> GetModelTwinCAT3Link(PropertyInfo elementProperty, ModelScanningProps modelScanningProps)
        {
            var twinCAT3Links = new List<ModelTwinCAT3Link>();
            modelScanningProps.TwinCAT3Links.ForEach(link => {

                var twinCAT3Link = new ModelTwinCAT3Link()
                {
                    Name = link.Name,
                    Platform = ModelUserPlatformEnumeration.TwinCAT3,
                    Ignore = link.IsIgnored,
                    SamplingRate = link.SamplingRate,
                };
                twinCAT3Link.SymbolKey.Path = link.SymbolPath;
                twinCAT3Link.SymbolKey.Name = elementProperty.Name.ToUpper();
                var twinCAT3SymbolNameAttribute = AttributeResolver.GetTwinCAT3SymbolNameAttribute(elementProperty);
                if (twinCAT3SymbolNameAttribute != null)
                    if (twinCAT3SymbolNameAttribute.ModelUserName == link.Name)
                        twinCAT3Link.SymbolKey.Name = twinCAT3SymbolNameAttribute.SymbolName.ToUpper();

                twinCAT3Links.Add(twinCAT3Link);
            });
            return twinCAT3Links;
        }

        protected static string GetLocalizationDisplay(PropertyInfo elementProperty, bool useTypeAttributes = false)
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

        protected static string GetLocalizationDescription(PropertyInfo elementProperty, bool useTypeAttributes = false)
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

        public static ModelScanningProps UpdatePreElementScanningProps(PropertyInfo elementProperty, ModelScanningProps modelScanningProps)
        {
            modelScanningProps.TwinCAT3Links = UpdateTwinCAT3Links(elementProperty, modelScanningProps);
            modelScanningProps.Attributes.LocalizationNamespace = UpdateLocalizationNamespace(elementProperty, modelScanningProps.Attributes.LocalizationNamespace);
            modelScanningProps.Attributes.VisibilityAuthorization = UpdateVisibilityAuthorization(elementProperty, modelScanningProps.Attributes.VisibilityAuthorization);
            modelScanningProps.Attributes.ManipulationAuthorization = UpdateManipulationAuthorization(elementProperty, modelScanningProps.Attributes.ManipulationAuthorization);
            modelScanningProps.Attributes.RepositoryDataFlags = UpdateRepositoryDataFlags(elementProperty, modelScanningProps.Attributes.RepositoryDataFlags);
            modelScanningProps.Attributes.IsReadOnly = UpdateIsReadOnly(elementProperty, modelScanningProps);
            modelScanningProps.Attributes.VariableValuePrecision = UpdateValuePrecision(elementProperty, modelScanningProps.Attributes.VariableValuePrecision);
            modelScanningProps.Attributes.EngineeringUnit = UpdateEngineeringUnit(elementProperty, modelScanningProps.Attributes.EngineeringUnit);
            modelScanningProps.Attributes.Filters = UpdateFilter(elementProperty, modelScanningProps);
            modelScanningProps.Attributes.FilterTriggers = UpdateFilterTriggers(elementProperty, modelScanningProps);

            return modelScanningProps;
        }

        protected static List<string> UpdateFilter(PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps)
        {
            var newFilters = new List<string>(currentModelScanningProps.Attributes.Filters);

            // Resolve attribute
            var elementFilters = AttributeResolver.GetFilterAttribute(elementProperty);
            if (elementFilters != null)
                newFilters.AddRange(elementFilters.Distinct());

            return newFilters;
        }

        protected static UnitValue? UpdateEngineeringUnit(PropertyInfo elementProperty, UnitValue? currentUnitValue)
        {
            var newUnitValue = currentUnitValue;

            // Resolve attribute
            var attributeEngineeringUnit = AttributeResolver.GetEngineeringUnit(elementProperty);
            if (attributeEngineeringUnit != null)
                newUnitValue = attributeEngineeringUnit;

            return newUnitValue;
        }

        protected static int UpdateValuePrecision(PropertyInfo elementProperty, int currentValuePrecision)
        {
            var newValuePrecision = currentValuePrecision;

            // Resolve attribute
            var elementValuePrecision = AttributeResolver.GetValuePrecision(elementProperty);
            if (elementValuePrecision != null)
                newValuePrecision = (int)elementValuePrecision;

            return newValuePrecision;
        }

        public static string GetModelPath(PropertyInfo elementProperty, string currentPath)
        {
            var modelPath = AttributeResolver.GetModePathAttribute(elementProperty);
            if (string.IsNullOrEmpty(modelPath))
                modelPath = currentPath;

            return modelPath;

        }

        protected static string GetModelName(PropertyInfo elementProperty)
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

        protected static string BuildModelPath(
            string currentPath,
            string linkName,
            string seperator = StringConstants.Separator)
        {
            if (string.IsNullOrEmpty(currentPath))
                return linkName;
            if (string.IsNullOrEmpty(linkName))
                return currentPath;

            return currentPath + seperator + linkName;
        }

        protected static string BuildLocalizationPath(
           string currentPath,
           PropertyInfo elementProperty,
           string seperator = StringConstants.Separator)
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

        protected static string BuildRepositoryPath(
          string currentPath,
          ModelComponentContainer modelContainer,
          PropertyInfo elementProperty,
          string seperator = StringConstants.Separator)
        {
            if (string.IsNullOrEmpty(currentPath))
                return modelContainer.GetType().Name;
            else
                return (currentPath + seperator + elementProperty.Name);
        }

        protected static string BuildTwinCAT3SymbolPath(
            string currentPath,
            string symbolName,
            string seperator = StringConstants.Separator)
        {

            if (string.IsNullOrEmpty(currentPath))
                return symbolName.ToUpper();
            else
                return (currentPath + seperator + symbolName).ToUpper();
        }

        protected static List<FilterTrigger> UpdateFilterTriggers(PropertyInfo elementProperty, ModelScanningProps currentModelScanningProps)
        {
            var newFilterTriggers = new List<FilterTrigger>(currentModelScanningProps.Attributes.FilterTriggers);

            // Resolve attribute
            var elementFilterTriggers = AttributeResolver.GetFilterTriggerAttributes(elementProperty);
            if (elementFilterTriggers != null && elementFilterTriggers.Count() > 0)
                newFilterTriggers.AddRange(elementFilterTriggers);

            return newFilterTriggers
                .Distinct()
                .ToList();
        }
    }

    public class ModelScanningMetaDataBuilder : ModelScanningMetaDataBuilderBase
    {
        public static ModelScanningProps IntializeModelProps(ModelScanningProps scannerProps)
        {
            if(scannerProps.ModelId.Equals(Guid.Empty))
            {   
                if(scannerProps.ParentModelContainer != null)
                    if (!scannerProps.ParentModelContainer.ModelId.Equals(Guid.Empty))
                        scannerProps.ModelId = scannerProps.ParentModelContainer.ModelId;
            }

            if (scannerProps.Attributes.Filters == null)
                scannerProps.Attributes.Filters = new List<string>();
            if (scannerProps.Attributes.FilterTriggers == null)
                scannerProps.Attributes.FilterTriggers = new List<FilterTrigger>();
            if (scannerProps.TwinCAT3Links == null)
                scannerProps.TwinCAT3Links = new List<ScanningTwinCAT3AttrubuteProps>();
            if (string.IsNullOrEmpty(scannerProps.ServiceName))
                scannerProps.ServiceName = string.Empty;
            if (string.IsNullOrEmpty(scannerProps.RepositoryPath))
                scannerProps.RepositoryPath = string.Empty;
            if (string.IsNullOrEmpty(scannerProps.RepositoryTypeName))
                scannerProps.RepositoryTypeName = string.Empty;
            if (string.IsNullOrEmpty(scannerProps.RepositoryPartTypeName))
                scannerProps.RepositoryPartTypeName = string.Empty;
            if (scannerProps.Attributes.VisibilityConditions == null)
                scannerProps.Attributes.VisibilityConditions = new();
            if (scannerProps.Attributes.EditabilityConditions == null)
                scannerProps.Attributes.EditabilityConditions = new();

            return scannerProps;
        }
        
        public static string UpdateModelKey(PropertyInfo elementProperty, ModelScanningProps modelScanningProps)
        {
            return  $"{GetModelPath(elementProperty, modelScanningProps.ModelPath)}.{GetModelName(elementProperty)}";
        }
    }

    public class ModelScanningContainerMetaDataBuilder : ModelScanningMetaDataBuilderBase
    {
        public static void FillModelContainerMetaData(PropertyInfo elementProperty,
            ModelComponentContainer modelContainer,
            ModelScanningProps modelScanningProps,
            bool isEnumerable,
            int enumerableIndex
            )
        {
            modelContainer.ModelId = modelScanningProps.ModelId;
            modelContainer.ModelLink.Path = GetModelPath(elementProperty, modelScanningProps.ModelPath);
            modelContainer.ModelLink.Name = GetModelName(elementProperty);
            modelContainer.Display.Link.Path = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            modelContainer.Display.Link.Name = CoreLocalizationNameConstants.DisplayName;
            modelContainer.Display.KeyNamespace = modelScanningProps.Attributes.LocalizationNamespace;
            modelContainer.Display.Value = GetLocalizationDisplay(elementProperty);
            modelContainer.Description.Link.Path = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            modelContainer.Description.Link.Name = CoreLocalizationNameConstants.Description;
            modelContainer.Description.KeyNamespace = modelScanningProps.Attributes.LocalizationNamespace;
            modelContainer.Description.Value = GetLocalizationDescription(elementProperty);
            modelContainer.VisibilityAuthorization = modelScanningProps.Attributes.VisibilityAuthorization;
            modelContainer.ManipulationAuthorization = modelScanningProps.Attributes.ManipulationAuthorization;
            modelContainer.IsEnumerableContainer = false;
            modelContainer.IsEnumerable = isEnumerable;
            modelContainer.EnumerationIndex = enumerableIndex;

        }

        public static void FillEnumerableContainerMetaData(PropertyInfo elementProperty,
            ModelComponentContainer modelContainer,
            ModelScanningProps modelScanningProps
            )
        {
            var displayName = GetLocalizationDisplay(elementProperty, true);
            if (string.IsNullOrEmpty(displayName))
            {
                modelContainer.Display.Link.Path = string.Empty;
                modelContainer.Display.Link.Name = string.Empty;
                modelContainer.Display.Value = string.Empty;
            }
            else
            {
                modelContainer.Display.Link.Path = modelScanningProps.LocalizationPath;
                modelContainer.Display.Value = displayName;
            }

            var descriptionName = GetLocalizationDescription(elementProperty, true);
            if (string.IsNullOrEmpty(descriptionName))
            {
                modelContainer.Description.Link.Path = string.Empty;
                modelContainer.Description.Link.Name = string.Empty;
                modelContainer.Description.Value = string.Empty;
            }
            else
            {
                modelContainer.Description.Link.Path = modelScanningProps.LocalizationPath;
                modelContainer.Description.Value = descriptionName;
            }

            modelContainer.ModelLink.Name = "";

        }

        public static void FillPLCLinks(PropertyInfo elementProperty,
        ModelComponentContainer modelContainer,
        ModelScanningProps modelScanningProps,
        bool isEnumerable
        )
        {
            modelContainer.TwinCAT3Links.Clear();
            modelScanningProps.TwinCAT3Links.ForEach(link =>
            {

                var twinCAT3Link = new ModelTwinCAT3Link()
                {
                    Name = link.Name,
                    Platform = ModelUserPlatformEnumeration.TwinCAT3,
                    Ignore = link.IsIgnored,
                    SamplingRate = link.SamplingRate,
                };
                twinCAT3Link.SymbolKey.Path = link.SymbolPath;
                twinCAT3Link.SymbolKey.Name = elementProperty.Name.ToUpper();
                var twinCAT3SymbolNameAttribute = AttributeResolver.GetTwinCAT3SymbolNameAttribute(elementProperty);
                if (twinCAT3SymbolNameAttribute != null)
                    if (twinCAT3SymbolNameAttribute.ModelUserName == link.Name)
                        twinCAT3Link.SymbolKey.Name = twinCAT3SymbolNameAttribute.SymbolName.ToUpper();

                if (isEnumerable)
                    twinCAT3Link.SymbolKey.Name = "";

                modelContainer.TwinCAT3Links.Add(twinCAT3Link);
            });
        }

        public static void FillRepositoryMetaData(PropertyInfo elementProperty, ModelComponentContainer modelContainer, ModelScanningProps modelScanningProps)
        {
            modelContainer.IsRepositoryClient = true;
            modelContainer.IsRepositoryRootContainer = false;
            modelContainer.RepositoryTypeName = modelScanningProps.RepositoryTypeName;
            modelContainer.RepositoryPartTypeName = modelScanningProps.RepositoryPartTypeName;
            modelContainer.RepositoryId = modelScanningProps.RepositoryId;
            modelContainer.RepositoryLink = $"{modelScanningProps.RepositoryPath}.{GetModelName(elementProperty)}";
        }

        public static ModelScanningProps UpdatePostContainerScanningProps(PropertyInfo elementProperty, ModelComponentContainer modelContainer, ModelScanningProps modelScanningProps)
        {
            modelScanningProps.ModelPath = BuildModelPath(GetModelPath(elementProperty, modelScanningProps.ModelPath), GetModelName(elementProperty));
            modelScanningProps.LocalizationPath = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            for (int i = 0; i < modelScanningProps.TwinCAT3Links.Count; i++)
            {
                var linkStruct = modelScanningProps.TwinCAT3Links[i];
                var twinCAT3SymbolName = elementProperty.Name;
                var twinCAT3SymbolNameAttribute = AttributeResolver.GetTwinCAT3SymbolNameAttribute(elementProperty);
                if (twinCAT3SymbolNameAttribute != null)
                    if (twinCAT3SymbolNameAttribute.ModelUserName == linkStruct.Name)
                        twinCAT3SymbolName = twinCAT3SymbolNameAttribute.SymbolName;

                linkStruct.SymbolPath = BuildTwinCAT3SymbolPath(linkStruct.SymbolPath, twinCAT3SymbolName);
                modelScanningProps.TwinCAT3Links[i] = linkStruct;
            }

            if (!string.IsNullOrEmpty(modelScanningProps.RepositoryTypeName))
                modelScanningProps.RepositoryPath = BuildRepositoryPath(modelScanningProps.RepositoryPath, modelContainer, elementProperty);

            return modelScanningProps;
        }

        public static void FillContainerRepositoryData(PropertyInfo elementProperty, ModelComponentContainer modelContainer, ModelScanningProps modelScanningProps)
        {
            modelContainer.IsRepositoryClient = true;
            modelContainer.RepositoryTypeName = modelScanningProps.RepositoryTypeName;
            modelContainer.RepositoryId = modelScanningProps.RepositoryId;
            modelContainer.RepositoryLink = $"{modelScanningProps.RepositoryPath}.{GetModelName(elementProperty)}";
            modelContainer.IsPersistent = true;
        }
    }

    public class ModelScanningContainerCollectionMetaDataBuilder: ModelScanningContainerMetaDataBuilder
    {
        public static string GetEnumerableString(
        int index,
        string startArray = StringConstants.StartArray,
        string endArray = StringConstants.EndArray)
        {
            return startArray + index.ToString() + endArray;
        }

        public static void FillContainerCollectionMetaData(PropertyInfo elementProperty, IModelComponentContainerCollection collectionModelContainer, ModelScanningProps modelScanningProps)
        {
            
            collectionModelContainer.ModelLink.Path = GetModelPath(elementProperty, modelScanningProps.ModelPath);
            collectionModelContainer.ModelLink.Name = GetModelName(elementProperty);
            collectionModelContainer.Display.Link.Path = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            collectionModelContainer.Display.Link.Name = CoreLocalizationNameConstants.DisplayName;
            collectionModelContainer.Display.KeyNamespace = modelScanningProps.Attributes.LocalizationNamespace;
            collectionModelContainer.Display.Value = GetLocalizationDisplay(elementProperty);
            collectionModelContainer.Description.Link.Path = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            collectionModelContainer.Description.Link.Name = CoreLocalizationNameConstants.Description;
            collectionModelContainer.Description.KeyNamespace = modelScanningProps.Attributes.LocalizationNamespace;
            collectionModelContainer.Description.Value = GetLocalizationDescription(elementProperty);
            collectionModelContainer.VisibilityAuthorization = modelScanningProps.Attributes.VisibilityAuthorization;
            collectionModelContainer.ManipulationAuthorization = modelScanningProps.Attributes.ManipulationAuthorization;
            collectionModelContainer.Filters = modelScanningProps.Attributes.Filters;
            collectionModelContainer.ServiceName = modelScanningProps.ServiceName;
        }

        public static void FillRegularModelContainer(PropertyInfo elementProperty, ModelComponentContainer modelContainer, ModelScanningProps modelScanningProps, int enumerationCount)
        {
            modelContainer.ModelLink.Path = GetModelPath(elementProperty, modelScanningProps.ModelPath);
            modelContainer.ModelLink.Name = GetModelName(elementProperty);
            modelContainer.Display.Link.Path = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            modelContainer.Display.Link.Name = CoreLocalizationNameConstants.DisplayName;
            modelContainer.Display.KeyNamespace = modelScanningProps.Attributes.LocalizationNamespace;
            modelContainer.Display.Value = GetLocalizationDisplay(elementProperty);
            modelContainer.Description.Link.Path = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            modelContainer.Description.Link.Name = CoreLocalizationNameConstants.Description;
            modelContainer.Description.KeyNamespace = modelScanningProps.Attributes.LocalizationNamespace;
            modelContainer.Description.Value = GetLocalizationDescription(elementProperty);
            modelContainer.VisibilityAuthorization = modelScanningProps.Attributes.VisibilityAuthorization;
            modelContainer.ManipulationAuthorization = modelScanningProps.Attributes.ManipulationAuthorization;
            modelContainer.Filters = modelScanningProps.Attributes.Filters;
            modelContainer.ServiceName = modelScanningProps.ServiceName;
            modelContainer.IsEnumerable = false;
            modelContainer.IsEnumerableContainer = true;
            modelContainer.EnumerationCount = enumerationCount;
            modelContainer.EnumerationIndex = -1;
        }
         
        public static void FillCollectionRepositoryMetaData(PropertyInfo elementProperty, IModelComponentContainerCollection collectionModelContainer, ModelScanningProps modelScanningProps)
        {
            collectionModelContainer.IsRepositoryClient = true;
            collectionModelContainer.RepositoryTypeName = modelScanningProps.RepositoryTypeName;
            collectionModelContainer.RepositoryPartTypeName = modelScanningProps.RepositoryPartTypeName;
            collectionModelContainer.RepositoryId = modelScanningProps.RepositoryId;
            collectionModelContainer.RepositoryLink = $"{modelScanningProps.RepositoryPath}.{GetModelName(elementProperty)}";
            collectionModelContainer.IsPersistent = true;
        }
        
        public static ModelScanningProps UpdateEnumerableContainerRelatedMetaData(PropertyInfo parentProperty, Type collectionItemType, int collectionIndex,
            string baseModelPath, string baseLocalizationPath, string baseRepositoryPath, List<string> baseTwinCAT3SymbolPathes, ModelScanningProps modelScanningProps)
        {
            modelScanningProps.ModelPath = BuildModelPath(baseModelPath, GetModelName(parentProperty)) + GetEnumerableString(collectionIndex + 1);
            modelScanningProps.LocalizationPath = BuildLocalizationPath(baseLocalizationPath, parentProperty) + StringConstants.Separator + collectionItemType.Name.ToLower();
            for (int i = 0; i < modelScanningProps.TwinCAT3Links.Count; i++)
            {
                var linkStruct = modelScanningProps.TwinCAT3Links[i];
                var twinCAT3SymbolName = parentProperty.Name;
                var twinCAT3SymbolNameAttribute = AttributeResolver.GetTwinCAT3SymbolNameAttribute(parentProperty);
                if (twinCAT3SymbolNameAttribute != null)
                    if (twinCAT3SymbolNameAttribute.ModelUserName == linkStruct.Name)
                        twinCAT3SymbolName = twinCAT3SymbolNameAttribute.SymbolName;

                linkStruct.SymbolPath = BuildTwinCAT3SymbolPath(baseTwinCAT3SymbolPathes[i], twinCAT3SymbolName) + GetEnumerableString(collectionIndex + 1);
                modelScanningProps.TwinCAT3Links[i] = linkStruct;
            }

            if (!string.IsNullOrEmpty(modelScanningProps.RepositoryTypeName))
                modelScanningProps.RepositoryPath = BuildRepositoryPath(baseRepositoryPath, modelScanningProps.ParentModelContainer, parentProperty) + GetEnumerableString(collectionIndex + 1);

            return modelScanningProps;
        }
    }

    public class ModelScanningVariableMetaDataBuilder : ModelScanningMetaDataBuilderBase
    {
        public static void FillVariableTwincat3Links(PropertyInfo elementProperty, ModelVariableBase modelVariableBase, ModelScanningProps modelScanningProps)
        {
            modelVariableBase.TwinCAT3Links.Clear();
            var links = GetModelTwinCAT3Link(elementProperty, modelScanningProps);
            modelVariableBase.TwinCAT3Links.AddRange(links);
        }

        public static void FillModelVariableMetaData(PropertyInfo elementProperty, ModelVariableBase modelVariableBase, ModelScanningProps modelScanningProps)
        {
            modelVariableBase.ModelId = modelScanningProps.ModelId;
            modelVariableBase.Display.Link.Path = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            modelVariableBase.Display.Link.Name = CoreLocalizationNameConstants.DisplayName;
            modelVariableBase.Display.KeyNamespace = modelScanningProps.Attributes.LocalizationNamespace;
            modelVariableBase.Display.Value = GetLocalizationDisplay(elementProperty);
            modelVariableBase.Description.Link.Path = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            modelVariableBase.Description.Link.Name = CoreLocalizationNameConstants.Description;
            modelVariableBase.Description.KeyNamespace = modelScanningProps.Attributes.LocalizationNamespace;
            modelVariableBase.Description.Value = GetLocalizationDescription(elementProperty);
            modelVariableBase.ModelLink.Path = GetModelPath(elementProperty, modelScanningProps.ModelPath);
            modelVariableBase.ModelLink.Name = GetModelName(elementProperty);
            modelVariableBase.EngineeringUnit = modelScanningProps.Attributes.EngineeringUnit;
            modelVariableBase.IsReadOnly = modelScanningProps.Attributes.IsReadOnly;
            modelVariableBase.VisibilityAuthorization = modelScanningProps.Attributes.VisibilityAuthorization;
            modelVariableBase.ManipulationAuthorization = modelScanningProps.Attributes.ManipulationAuthorization;
            modelVariableBase.ValuePrecision = modelScanningProps.Attributes.VariableValuePrecision;
            modelVariableBase.Filters = modelScanningProps.Attributes.Filters;
            modelVariableBase.ServiceName = modelScanningProps.ServiceName;
          
        }
        
        public static void FillModelVariableRepositoryData(PropertyInfo elementProperty, ModelVariableBase modelVariableBase, ModelScanningProps modelScanningProps)
        {
            if (!string.IsNullOrEmpty(modelScanningProps.RepositoryTypeName))
            {
                modelVariableBase.IsRepositoryClient = true;
                modelVariableBase.RepositoryTypeName = modelScanningProps.RepositoryTypeName;
                modelVariableBase.RepositoryPartTypeName = modelScanningProps.RepositoryPartTypeName;
                modelVariableBase.RepositoryId = modelScanningProps.RepositoryId;
                modelVariableBase.RepositoryLink = $"{modelScanningProps.RepositoryPath}.{GetModelName(elementProperty)}";

                if (modelScanningProps.Attributes.RepositoryDataFlags != null)
                {
                    var repositoryDataFlags = (ModelRepositoryDataFlags)modelScanningProps.Attributes.RepositoryDataFlags;
                    if (repositoryDataFlags.HasFlag(ModelRepositoryDataFlags.VariableData))
                        modelVariableBase.IsPersistent = true;
                }
            }
        }
    }

    public class ModelScanningRPCMetaDataBuilder : ModelScanningMetaDataBuilderBase
    {
        public static void FillRPCTwinCAT3Links(PropertyInfo elementProperty, ModelRPCBase modelRPCBase, ModelScanningProps modelScanningProps)
        {
            modelRPCBase.TwinCAT3Links.Clear();
            var links = GetModelTwinCAT3Link(elementProperty, modelScanningProps);
            modelRPCBase.TwinCAT3Links.AddRange(links);
        }

        public static void FillRPCMetaData(ModelRPCBase modelRPCBase, PropertyInfo elementProperty, ModelScanningProps modelScanningProps)
        {
            modelRPCBase.ModelId = modelScanningProps.ModelId;
            modelRPCBase.Display.Link.Path = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            modelRPCBase.Display.Link.Name = CoreLocalizationNameConstants.DisplayName;
            modelRPCBase.Display.KeyNamespace = modelScanningProps.Attributes.LocalizationNamespace;
            modelRPCBase.Display.Value = GetLocalizationDisplay(elementProperty);
            modelRPCBase.Description.Link.Path = BuildLocalizationPath(modelScanningProps.LocalizationPath, elementProperty);
            modelRPCBase.Description.Link.Name = CoreLocalizationNameConstants.Description;
            modelRPCBase.Description.KeyNamespace = modelScanningProps.Attributes.LocalizationNamespace;
            modelRPCBase.Description.Value = GetLocalizationDescription(elementProperty);
            modelRPCBase.ModelLink.Path = modelScanningProps.ModelPath;
            modelRPCBase.ModelLink.Name = elementProperty.Name;
            modelRPCBase.ServiceName = modelScanningProps.ServiceName;
            modelRPCBase.Filters = modelScanningProps.Attributes.Filters;
            modelRPCBase.VisibilityAuthorization = modelScanningProps.Attributes.VisibilityAuthorization;
            modelRPCBase.ManipulationAuthorization = modelScanningProps.Attributes.ManipulationAuthorization;
            }
    }
}
