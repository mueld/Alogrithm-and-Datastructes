using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Attributes.Resolver;
using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public class ModelComponentScanner
    {
        #region fields

        private int _currentlyProcessedVariableScanningIndex = -1;

        private readonly ModelScanningTriggerValidator _scanningTriggersValidator;
        private readonly Dictionary<ModelVariableBase, string> _variableMaximumValueDependencyDictionary = new();
        private readonly Dictionary<ModelVariableBase, string> _variableMinimumValueDependencyDictionary = new();
        private List<IAdditionalModelComponentScannerFunctionality> _additonalFunctionalities = new();     

        #endregion

        #region ctor

        public ModelComponentScanner()
        {
            _scanningTriggersValidator = new();
        }

        public ModelComponentScanner(List<IAdditionalModelComponentScannerFunctionality> additionalFunctionality)
        {
            _scanningTriggersValidator = new();
            _additonalFunctionalities = additionalFunctionality;
        }

        #endregion
        #region props

        public bool DistributeVariables { get; set; } = false;
        public bool ForceValueActivation { get; set; } = true;
        public bool WrapSourceObject { get; set; } = false;
        public ModelScanningTrigger ScanningTriggers { get; set; } = new();

        public List<string> ErrorMessageStack { get; private set; } = new();
        public List<string> WarningMessageStack { get; private set; } = new();

        public List<IServiceModelContainer> ServiceModelContainers { get; private set; } = new();
        public List<IRepositoryModelContainer> RepositoryModelContainers { get; private set; } = new();
        public List<IModelComponentContainerCollection> EnumerableModelContainers { get; private set; } = new();

        public ModelComponentContainer ModelComponentTree { get; private set; }
        public List<IModelVariable> Variables { get; private set; } = new List<IModelVariable>();
        public List<IModelRPC> RPCs { get; private set; } = new List<IModelRPC>();
        public List<IModelVariable> DistributionVariables { get; set; } = new List<IModelVariable>();

        #endregion
        #region methods

        public void AddFunctionality(IAdditionalModelComponentScannerFunctionality functionality)
        {
            if(!_additonalFunctionalities.Contains(functionality))
            {
                _additonalFunctionalities.Add(functionality);
            }
        }

        public bool AddVariable(IModelVariable modelVariable)
        {
            if (modelVariable == null)
                return false;

            Variables.Add(modelVariable);

            return true;
        }

        public bool AddRPC(IModelRPC modelRPC)
        {
            if (modelRPC == null)
                return false;

            RPCs.Add(modelRPC);

            return true;
        }

        #endregion
        #region methods scanning

        public bool Scan(object modelComponent)
        {
            var modelScannerProps = new ModelScanningProps
            {
                Element = modelComponent
            };
                    
            return Scan(modelScannerProps);
        }

        public bool Scan(ModelScanningProps scannerProps)
        {
            if (scannerProps.Element == null)
            {
                ErrorMessageStack.Add($"No [Element] in scanner props defined. Scanning aborted.");
                return false;
            }

            ServiceModelContainers.Clear();
            Variables.Clear();
            RPCs.Clear();
            ModelComponentTree = scannerProps.ParentModelContainer ?? new ModelComponentContainer();
            _currentlyProcessedVariableScanningIndex = 0;

            // Check for required props
            scannerProps = ModelScanningMetaDataBuilder.IntializeModelProps(scannerProps);

            if (scannerProps.ParentModelContainer == null)
                scannerProps.ParentModelContainer = ModelComponentTree;
  
            if (WrapSourceObject)
            {
                var wrapperElement = new ScanningWrapperElement
                {
                    ProxyElement = scannerProps.Element
                };

                scannerProps.Element = wrapperElement;
            }

            var result = ScanModelComponents(scannerProps);
            if (result)
            {

                _additonalFunctionalities.ForEach(f => {
                    if (!f.ScanCompleted())
                        result = false;
                    });
                ResolveVariableMaximumValueDependencies();
                ResolveVariableMinimumValueDependencies();
            }

            return result;
        }

        private bool ScanModelComponents(ModelScanningProps modelScannerProps)
        {
            var elementType = modelScannerProps.Element.GetType();
            var elementProperties = elementType.GetProperties();
            
            for (int i = 0; i < elementProperties.Length; i++)
            {
                var elementProperty = elementProperties[i];
                var elementPropertyValue = GetPropertyValue(modelScannerProps.Element, elementProperty);
                if (elementPropertyValue == null)
                {
                    if (ErrorMessageStack.Count > 0)
                        return false;

                    continue;
                }

                var elementPropertyValueType = elementPropertyValue.GetType();

                //update
                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelComponentContainer)))
                    if (!ResolveModelContainer(elementProperty, modelScannerProps))
                        return false;

                var resetContainer = true;
                var breakpoint = false;

                if (ScanningTriggers.HasTriggers)
                {
                    _scanningTriggersValidator.CheckTiggers(ScanningTriggers, modelScannerProps.ModelKey, modelScannerProps.RepositoryTypeName, modelScannerProps.ServiceName);
                    resetContainer = _scanningTriggersValidator.ResetContainer;
                    breakpoint = _scanningTriggersValidator.Breakpoint;
                }

                if (!resetContainer || breakpoint)
                {
                    continue;
                }

                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelComponentContainerCollection)) && elementPropertyValueType.IsGenericType)
                    if (!ResolveModelContainerCollection(elementProperty, modelScannerProps))
                        return false;

                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelVariable)))
                    if (!ResolveVariable(elementProperty, modelScannerProps))
                        return false;

                if (elementPropertyValueType.GetInterfaces().Contains(typeof(IModelRPC)))
                    if (!ResolveRPCMethod(elementProperty, modelScannerProps))
                        return false;
            }

            return true;
        }

        private object GetPropertyValue(object parentElement, PropertyInfo elementProperty, object[] index = null)
        {
            var elementPropertyValue = elementProperty.GetValue(parentElement, index);
            if (elementPropertyValue != null)
                return elementPropertyValue;

            object activatedValue = null;

            if (elementProperty.PropertyType.GetInterfaces().Contains(typeof(IModelVariable)))
            {
                if (DistributeVariables)
                {
                    var distributionVariable = DistributionVariables
                        .Find(variable => variable.ScanningIndex == _currentlyProcessedVariableScanningIndex);

                    if (distributionVariable == null)
                        ErrorMessageStack.Add($"No corresponding distribution variable for model property type [{elementProperty.PropertyType.Name }] in type [{parentElement.GetType().Name}] available. Processing for this property will be skipped.");

                    activatedValue = TryActivateValue(parentElement, distributionVariable, elementProperty) as ModelVariableBase;
                    if (activatedValue == null)
                        ErrorMessageStack.Add($"Distribution variable for model property type [{elementProperty.PropertyType.Name }] in type [{parentElement.GetType().Name}] could not be activated. Processing for this property will be skipped.");                
                }
                else
                {
                    if (ForceValueActivation)
                        activatedValue = TryActivateValue(parentElement, elementProperty) as ModelVariableBase;

                    if (activatedValue == null)
                        ErrorMessageStack.Add($"Variable property type [{elementProperty.Name }] in type [{parentElement.GetType().Name}] could not be activated. Processing for this property will be skipped.");
                    else
                        WarningMessageStack.Add($"Variable [{elementProperty.Name}] in type [{parentElement.GetType().Name}] was not initialized. A new instance was created.");
                }

                return activatedValue;
            }

            if (elementProperty.PropertyType.GetInterfaces().Contains(typeof(IModelComponentContainer)) ||
                elementProperty.PropertyType.GetInterfaces().Contains(typeof(IModelComponentContainerCollection)))
            {

                if (!ForceValueActivation)
                {
                    WarningMessageStack.Add($"Model property type [{elementProperty.PropertyType.Name }] in type [{parentElement.GetType().Name}] is not activated. Processing for this property will be skipped.");
                    return activatedValue;
                }

                if (elementProperty.PropertyType.GetInterfaces().Contains(typeof(IModelComponentContainer)))
                    activatedValue = TryActivateValue(parentElement, elementProperty) as ModelComponentContainer;

                //if (elementProperty.PropertyType.GetInterfaces().Contains(typeof(IModelComponentContainerCollection)))
                //    activatedValue = TryActivateValue(parentElement, elementProperty) as ModelComponentContainerCollection;

                if (activatedValue == null)
                    ErrorMessageStack.Add($"Model property type [{elementProperty.PropertyType.Name }] in type [{parentElement.GetType().Name}] could not be activated. Processing for this property will be skipped.");
                else
                    WarningMessageStack.Add($"Model property type [{elementProperty.PropertyType.Name}] in type [{parentElement.GetType().Name}] was not initialized. A new instance was created.");
            }

            return activatedValue;
        }

        private object TryActivateValue(object parentElement, PropertyInfo elementProperty)
        {
            var elementPropertyType = elementProperty.PropertyType;
            var activatedValue = Activator.CreateInstance(elementPropertyType);
            if (activatedValue == null)
            {
                ErrorMessageStack.Add($"Value for [{parentElement.GetType().Name}] could not be activated.");
                return null;
            }

            return TryActivateValue(parentElement, activatedValue, elementProperty);
        }

        private object TryActivateValue(object parentElement, object value, PropertyInfo elementProperty)
        {
            try
            {
                elementProperty.SetValue(parentElement, value);
                return value;
            }
            catch (Exception ex)
            {
                ErrorMessageStack.Add($"Unexpected error on setting value [{value}] for [{parentElement.GetType().Name}]. {ex.Message}");
                return null;
            }
        }

        private static ModelScanningProps UpdateScanningProps(PropertyInfo elementProperty, Type elementPropertyValueType,  ModelScanningProps modelScanningProps)
        {
            ModelScanningProps newProps = modelScanningProps;
            if (!elementPropertyValueType.GetInterfaces().Contains(typeof(IModelComponent)))
                return newProps;

            if (elementPropertyValueType.GetInterfaces().Contains(typeof(IServiceModelContainer)) &&
                elementProperty.Name != "ParentModelContainer")
                newProps.ServiceName = elementProperty.Name;

            if (elementPropertyValueType.GetInterfaces().Contains(typeof(IRepositoryModelContainer)) &&
                elementProperty.Name != "ParentModelContainer")
                newProps.RepositoryTypeName = elementPropertyValueType.Name;

            newProps.ModelKey = ModelScanningMetaDataBuilder.UpdateModelKey(elementProperty, modelScanningProps);

            return newProps;
        }

        private bool ResolveModelContainer(PropertyInfo elementProperty, ModelScanningProps modelScanningProps, object[] index = null)
        {
            if (elementProperty.Name == "ParentModelContainer")
                return true;


                // Update pre scanning props
                modelScanningProps = ModelScanningContainerMetaDataBuilder.UpdatePreElementScanningProps(elementProperty,  modelScanningProps);
            
            // Check if this an enumerable model container
            var isEnumerable = (index != null);
            var enumerableIndex = -1;
            if (index != null && index.Length > 0)
                enumerableIndex = (int)index[0];

            // Resolve ModelContainer
            var modelContainer = elementProperty.GetValue(modelScanningProps.Element, index) as ModelComponentContainer;
            modelScanningProps = UpdateScanningProps(elementProperty, modelContainer.GetType(), modelScanningProps);

            if (modelContainer == null)
            {
                ErrorMessageStack.Add($"ModelComponentContainer property type [{elementProperty.PropertyType.Name }] in type [{modelScanningProps.Element.GetType().Name}] could not be activated. Processing for this property will be skipped.");
                return false;
            }
            var resetContainer = true;
            var checkNestedContainer = false;
            var breakpoint = false;
           
            if (ScanningTriggers.HasTriggers)
            {
                _scanningTriggersValidator.CheckTiggers(ScanningTriggers, modelScanningProps.ModelKey, modelScanningProps.RepositoryTypeName, modelScanningProps.ServiceName);
                resetContainer = _scanningTriggersValidator.ResetContainer;
                checkNestedContainer = _scanningTriggersValidator.CheckNestedContainer;
                breakpoint = _scanningTriggersValidator.Breakpoint;
            }

            if (!resetContainer)
            {
                // Update post scanning props
                if (!isEnumerable)
                    modelScanningProps = ModelScanningContainerMetaDataBuilder.UpdatePostContainerScanningProps(elementProperty, modelContainer, modelScanningProps);

                modelScanningProps.Element = modelContainer;
                modelScanningProps.ParentModelContainer = modelContainer;

                try
                {
                    _additonalFunctionalities.ForEach(f => modelScanningProps = f.onModelComponentContainer(modelContainer, elementProperty, modelScanningProps));
                }
                catch (Exception e)
                {
                    ErrorMessageStack.Add(e.Message);
                    return false;
                }

                return ScanModelComponents(modelScanningProps);
            }

            // Clear structure data
            modelContainer.ModelContainers.Clear();
            modelContainer.ModelVariables.Clear();
            modelContainer.ModelRPCs.Clear();
           
            ModelScanningContainerMetaDataBuilder.FillModelContainerMetaData(elementProperty, modelContainer, modelScanningProps, isEnumerable, enumerableIndex);

            if (isEnumerable)
            {
                ModelScanningContainerMetaDataBuilder.FillEnumerableContainerMetaData(elementProperty, modelContainer, modelScanningProps);               
            }

            // Fill TwinCAT3 links
            ModelScanningContainerMetaDataBuilder.FillPLCLinks(elementProperty, modelContainer, modelScanningProps, isEnumerable);

            // Check for service model container
            var modelContainerType = modelContainer.GetType();
            if (modelContainerType.GetInterfaces().Contains(typeof(IServiceModelContainer)))
            {
                // Update scanner props
                if (string.IsNullOrEmpty(modelScanningProps.ServiceName))
                {
                    modelScanningProps.ServiceName = elementProperty.Name;
                }
                else
                {
                    // Service is child of another service...
                    modelScanningProps.ServiceName = elementProperty.Name;
                }

                if (!string.IsNullOrEmpty(modelScanningProps.ServiceName))
                {
                    if (!modelScanningProps.Attributes.Filters.Contains(modelScanningProps.ServiceName))
                        modelScanningProps.Attributes.Filters.Add(modelScanningProps.ServiceName);
                }

                // Add service model container reference to the store
                if (!ServiceModelContainers.Any(smc => smc.Id == modelContainer.Id))
                    ServiceModelContainers.Add(modelContainer as IServiceModelContainer);

               
            }

            // Check for repository model container
            if (modelContainerType.GetInterfaces().Contains(typeof(IRepositoryModelContainer)))
            {
                var repositoryModelContainer = modelContainer as IRepositoryModelContainer;

                // Check name condition for partial created repositories
                if (repositoryModelContainer.RepositoryCreationPolicy == RepositoryCreationPolicyEnumeration.Partial &&
                    string.IsNullOrEmpty(modelContainer.RepositoryTypeName))
                    throw new InvalidOperationException($"Repository container [{modelContainerType.Name}] has a partial creation policy and no repository name defined. You must override the {nameof(modelContainer.RepositoryTypeName)} propety when the interface {nameof(IRepositoryModelContainer)} is implemented and the creation plolicy is set to {RepositoryCreationPolicyEnumeration.Partial}.");

                // Reset path to create the repository link  
                modelScanningProps.RepositoryPath = string.Empty;

                // Make sure the repository type name is present
                if (string.IsNullOrEmpty(modelContainer.RepositoryTypeName))
                    modelContainer.RepositoryTypeName = modelContainerType.Name; //elementProperty.Name;
                modelScanningProps.RepositoryTypeName = modelContainer.RepositoryTypeName;

                // Make sure the repository part type name is present
                if (string.IsNullOrEmpty(modelContainer.RepositoryPartTypeName))
                    modelContainer.RepositoryPartTypeName = modelContainerType.Name;
                modelScanningProps.RepositoryPartTypeName = modelContainer.RepositoryPartTypeName;

                if (!modelScanningProps.Attributes.Filters.Contains(modelScanningProps.RepositoryTypeName))
                    modelScanningProps.Attributes.Filters.Add(modelScanningProps.RepositoryTypeName);

                // Make sure the repository id is present
                // It's important to use the existing repositroy id if available
                if (modelContainer.RepositoryId == Guid.Empty)
                    modelContainer.RepositoryId = modelContainer.Id;
                modelScanningProps.RepositoryId = modelContainer.RepositoryId;

                // Set the default repository part name if needed
                if (string.IsNullOrEmpty(modelContainer.RepositoryPartTypeName))
                    modelContainer.RepositoryPartTypeName = modelContainerType.Name;

                modelContainer.IsRepositoryClient = true;
                modelContainer.IsRepositoryRootContainer = true;
                modelContainer.RepositoryLink = modelContainer.GetType().Name;

                // Add shared model links
                if (!string.IsNullOrEmpty(repositoryModelContainer.ModelLink.Key))
                    if (!repositoryModelContainer.RepositoryRelatedModelLinks.Contains(repositoryModelContainer.ModelLink.Key))
                        repositoryModelContainer.RepositoryRelatedModelLinks.Add(repositoryModelContainer.ModelLink.Key);

                if (repositoryModelContainer.RepositoryCreationPolicy == RepositoryCreationPolicyEnumeration.Shared)
                {
                    var existingRepositoryContainers = RepositoryModelContainers
                        .Where(container => container.RepositoryTypeName == modelContainer.RepositoryTypeName)
                        .ToList();

                    foreach (var existingRepositoryContainer in existingRepositoryContainers)
                    {
                        if (!string.IsNullOrEmpty(repositoryModelContainer.ModelLink.Key))
                            if (!existingRepositoryContainer.RepositoryRelatedModelLinks.Contains(repositoryModelContainer.ModelLink.Key))
                            existingRepositoryContainer.RepositoryRelatedModelLinks.Add(repositoryModelContainer.ModelLink.Key);

                        if (!string.IsNullOrEmpty(existingRepositoryContainer.ModelLink.Key))
                            if (!repositoryModelContainer.RepositoryRelatedModelLinks.Contains(existingRepositoryContainer.ModelLink.Key))
                            repositoryModelContainer.RepositoryRelatedModelLinks.Add(existingRepositoryContainer.ModelLink.Key);

                        modelContainer.RepositoryId = existingRepositoryContainer.RepositoryId;
                        modelScanningProps.RepositoryId = existingRepositoryContainer.RepositoryId;
                    }
                }

                if (repositoryModelContainer.RepositoryCreationPolicy == RepositoryCreationPolicyEnumeration.Partial)
                {
                    modelScanningProps.RepositoryPartTypeName = modelContainer.RepositoryPartTypeName;

                    if (!modelScanningProps.Attributes.Filters.Contains(modelScanningProps.RepositoryPartTypeName))
                        modelScanningProps.Attributes.Filters.Add(modelScanningProps.RepositoryPartTypeName);
                }

                RepositoryModelContainers.Add(repositoryModelContainer);
            }
            else if (!string.IsNullOrEmpty(modelScanningProps.RepositoryTypeName))
            {
                // Inject name of the repository into the filter list
                if (!modelScanningProps.Attributes.Filters.Contains(modelScanningProps.RepositoryTypeName))
                    modelScanningProps.Attributes.Filters.Add(modelScanningProps.RepositoryTypeName);

                // Inject part name of the repository into the filter list
                if (!string.IsNullOrEmpty(modelScanningProps.RepositoryPartTypeName))
                    if (!modelScanningProps.Attributes.Filters.Contains(modelScanningProps.RepositoryPartTypeName))
                        modelScanningProps.Attributes.Filters.Add(modelScanningProps.RepositoryPartTypeName);

                // Fille the repository meta data
                ModelScanningContainerMetaDataBuilder.FillRepositoryMetaData(elementProperty, modelContainer, modelScanningProps);
            }

            // Update current model container props
            modelContainer.ServiceName = modelScanningProps.ServiceName;
            modelContainer.Filters = modelScanningProps.Attributes.Filters;

            // Update post scanning props
            if (!isEnumerable)
                modelScanningProps = ModelScanningContainerMetaDataBuilder.UpdatePostContainerScanningProps(elementProperty, modelContainer, modelScanningProps);

            // Replace next scanning element
            modelScanningProps.Element = modelContainer;

            // Setup parent <-> child relationship       
            modelContainer.ParentModelContainer = modelScanningProps.ParentModelContainer;
            if (modelScanningProps.ParentModelContainer == null)
            {
                modelScanningProps.Element = modelContainer;
            }
            else
            {
                if (!modelScanningProps.ParentModelContainer.ModelContainers.Any(container => container.ModelLink.Key == modelContainer.ModelLink.Key))
                {
                    modelScanningProps.ParentModelContainer.ModelContainers.Add(modelContainer);
                }
            }
               
            // Replace next parent model container
            modelScanningProps.ParentModelContainer = modelContainer;

            try
            {
                _additonalFunctionalities.ForEach(f => modelScanningProps = f.onModelComponentContainer(modelContainer, elementProperty, modelScanningProps));
            }
            catch (Exception e)
            {
                ErrorMessageStack.Add(e.Message);
                return false;
            }

            return ScanModelComponents(modelScanningProps);
        }

        private bool ResolveModelContainerCollection(PropertyInfo elementProperty, ModelScanningProps modelScanningProps)
        {
            if (!string.IsNullOrEmpty(modelScanningProps.ModelPath) &&
                ScanningTriggers.HasModelKeyTriggers &&
                !ScanningTriggers.ModelKeys.Any(key => modelScanningProps.ModelKey.Contains(key)))
                return true;

            // Update pre scanning props
            modelScanningProps = ModelScanningContainerMetaDataBuilder.UpdatePreElementScanningProps(elementProperty, modelScanningProps);

            // Resolve prop value and type
            var collection = elementProperty.GetValue(modelScanningProps.Element, null);
            if (collection == null)
            {
                ErrorMessageStack.Add($"ModelContainerCollection property type [{elementProperty.PropertyType.Name }] in type [{modelScanningProps.Element.GetType().Name}] could not be activated. Processing for this property will be skipped.");
                return false;
            }

            var collectionModelContainer = collection as IModelComponentContainerCollection;
            if (collectionModelContainer != null)
            {
                ModelScanningContainerCollectionMetaDataBuilder.FillContainerCollectionMetaData(elementProperty, collectionModelContainer, modelScanningProps);

                // Add the collection information
                EnumerableModelContainers.Add(collectionModelContainer);
            }

            // Resolve "Count" prop of the collection
            int collectionCount = (int)elementProperty.PropertyType.GetProperty("Count").GetValue(collection, null);

            // Setup a regular model componnet container as a replacement for a ModelContainerCollection. With this we can bypass the generic elements of the collection.
            var modelContainer = new ModelComponentContainer();
            ModelScanningContainerCollectionMetaDataBuilder.FillRegularModelContainer(elementProperty, modelContainer, modelScanningProps, collectionCount);

            // Set repository data
            if (!string.IsNullOrEmpty(modelScanningProps.RepositoryTypeName))
            {
                if (collectionModelContainer != null)
                {
                    ModelScanningContainerCollectionMetaDataBuilder.FillCollectionRepositoryMetaData(elementProperty, collectionModelContainer, modelScanningProps);
                }
                ModelScanningContainerMetaDataBuilder.FillContainerRepositoryData(elementProperty, modelContainer, modelScanningProps);
            }

            // Fill TwinCAT3 links
            ModelScanningContainerMetaDataBuilder.FillPLCLinks(elementProperty, modelContainer, modelScanningProps, false);

            // Setup parent <-> child relationship
            modelContainer.ParentModelContainer = modelScanningProps.ParentModelContainer;
            if (modelScanningProps.ParentModelContainer != null)
                if (!modelScanningProps.ParentModelContainer.ModelContainers.Any(container => container.ModelLink.Key == modelContainer.ModelLink.Key))
                    modelScanningProps.ParentModelContainer.ModelContainers.Add(modelContainer);
            if (modelScanningProps.ParentModelContainer != null)
                if (!modelScanningProps.ParentModelContainer.EnumerableModelContainers.Any(container => container.ModelLink.Key == modelContainer.ModelLink.Key))
                    modelScanningProps.ParentModelContainer.EnumerableModelContainers.Add(collectionModelContainer);

            // Replace next parent model container
            modelScanningProps.ParentModelContainer = modelContainer;

            var baseModelPath = ModelScanningContainerCollectionMetaDataBuilder.GetModelPath(elementProperty, modelScanningProps.ModelPath);
            var baseLocalizationPath = modelScanningProps.LocalizationPath;
            var baseRepositoryPath = modelScanningProps.RepositoryPath;
            var baseTwinCAT3SymbolPathes = new List<string>();
            modelScanningProps.TwinCAT3Links.ForEach(link => baseTwinCAT3SymbolPathes.Add(link.SymbolPath));

            for (int collectionIndex = 0; collectionIndex < collectionCount; collectionIndex++)
            {
                // Get the collection item
                var collectionItemProperty = elementProperty.PropertyType.GetProperty("Item");
                object[] index = { collectionIndex };
                object collectionItem = elementProperty.PropertyType.GetProperty("Item").GetValue(collection, index);
                var collectionItemType = collectionItem.GetType();
                
                modelScanningProps = ModelScanningContainerCollectionMetaDataBuilder.UpdateEnumerableContainerRelatedMetaData(elementProperty, collectionItemType, collectionIndex,
                    baseModelPath, baseLocalizationPath, baseRepositoryPath, baseTwinCAT3SymbolPathes, modelScanningProps);

                // Replace next scanning element
                modelScanningProps.Element = collection;

                try
                {
                    _additonalFunctionalities.ForEach(f => modelScanningProps = f.onModelContainerCollection(collectionModelContainer, elementProperty, modelScanningProps));
                }
                catch (Exception e)
                {
                    ErrorMessageStack.Add(e.Message);
                    return false;
                }

                if (collectionItemType.GetInterfaces().Contains(typeof(IModelComponentContainer)))
                    if (!ResolveModelContainer(collectionItemProperty, modelScanningProps, index))
                        return false;
            }

            return true;
        }
      
        private bool ResolveVariable(PropertyInfo elementProperty, ModelScanningProps modelScanningProps)
        {
            if (!string.IsNullOrEmpty(modelScanningProps.ModelPath) &&
                ScanningTriggers.HasModelKeyTriggers &&
                !ScanningTriggers.ModelKeys.Any(key => modelScanningProps.ModelKey.Contains(key)))
                return true;

            // Update pre scanning props
            modelScanningProps = ModelScanningVariableMetaDataBuilder.UpdatePreElementScanningProps(elementProperty, modelScanningProps);

            // Resolve VariableBase
            var modelVariableBase = elementProperty.GetValue(modelScanningProps.Element, null) as ModelVariableBase;
            if (modelVariableBase == null)
            {
                ErrorMessageStack.Add($"Variable for model property type [{elementProperty.PropertyType.Name }] in type [{modelScanningProps.Element.GetType().Name}] not actiavted. Processing for this property will be skipped.");
                return false;
            }

            // Add filter triggers
            if (modelScanningProps.Attributes.FilterTriggers.Count > 0)
                modelVariableBase.AddFilterTriggers(modelScanningProps.Attributes.FilterTriggers);

            // Fill TwinCAT3 links          
            ModelScanningVariableMetaDataBuilder.FillVariableTwincat3Links(elementProperty, modelVariableBase, modelScanningProps);

            // Fill props
            ModelScanningVariableMetaDataBuilder.FillModelVariableMetaData(elementProperty, modelVariableBase, modelScanningProps);
            modelVariableBase.ScanningIndex = _currentlyProcessedVariableScanningIndex;

            // Set repository data
            ModelScanningVariableMetaDataBuilder.FillModelVariableRepositoryData(elementProperty, modelVariableBase, modelScanningProps);

            // Check for correct value type
            if (modelVariableBase.ValueType.IsPrimitive || modelVariableBase.ValueType.IsEnum || modelVariableBase.ValueType == typeof(string))
            {
                ;
            }
            else if (modelVariableBase.ValueType.GetInterfaces().Contains(typeof(IModelComponentContainer)))
            {
                ErrorMessageStack.Add($"{modelVariableBase.ValueType} is no valid value type for variable {modelVariableBase.ModelLink}.");
                return false;
            }
            else if (
                modelVariableBase.ValueType.IsGenericType &&
                modelVariableBase.ValueType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var argumentType = modelVariableBase.ValueType.GetGenericArguments()[0];
                if (!argumentType.IsPrimitive && !argumentType.IsEnum && (argumentType != typeof(string)))
                {
                    ErrorMessageStack.Add($"{argumentType} is no valid generic value type for variable {modelVariableBase.ModelLink}.");
                    return false;
                }
            }
            else
            {
                ErrorMessageStack.Add($"{modelVariableBase.ValueType} is no valid value type for variable {modelVariableBase.ModelLink}.");
                return false;
            }

            // Setup parent <-> child relationship
            modelVariableBase.ParentModelContainer = modelScanningProps.ParentModelContainer;
            modelScanningProps.ParentModelContainer?.ModelVariables.Add(modelVariableBase);
            
            // Add variable to the scanner list
            AddVariable(modelVariableBase as IModelVariable);

            try
            {
                _additonalFunctionalities.ForEach(f => modelScanningProps = f.onModelVariable(modelVariableBase, elementProperty, modelScanningProps));
            }
            catch (Exception e)
            {
                ErrorMessageStack.Add(e.Message);
                return false;
            }

            _currentlyProcessedVariableScanningIndex++;
            return true;
        }

        private bool ResolveRPCMethod(PropertyInfo elementProperty, ModelScanningProps modelScanningProps)
        {
            if (!string.IsNullOrEmpty(modelScanningProps.ModelPath) &&
                ScanningTriggers.HasModelKeyTriggers &&
                !ScanningTriggers.ModelKeys.Any(key => modelScanningProps.ModelKey.Contains(key)))
                return true;

            // Update pre scanning props
            modelScanningProps = ModelScanningRPCMetaDataBuilder.UpdatePreElementScanningProps(elementProperty, modelScanningProps);

            // Resolve VariableBase    
            var modelRPCBase = elementProperty.GetValue(modelScanningProps.Element, null) as ModelRPCBase;
            if (modelRPCBase == null)
            {
                ErrorMessageStack.Add($"RPCMethod property type [{elementProperty.PropertyType.Name }] in type [{modelScanningProps.Element.GetType().Name}] could not be activated. Processing for this property will be skipped.");
                return false;
            }

            // Add filter triggers
            if (modelScanningProps.Attributes.FilterTriggers.Count > 0)
                modelRPCBase.AddFilterTriggers(modelScanningProps.Attributes.FilterTriggers);

            // Fill TwinCAT3 links
            ModelScanningRPCMetaDataBuilder.FillRPCTwinCAT3Links(elementProperty, modelRPCBase, modelScanningProps);

            // Fill meta data
            ModelScanningRPCMetaDataBuilder.FillRPCMetaData(modelRPCBase, elementProperty, modelScanningProps);

            // Setup parent <-> child relationship
            modelRPCBase.ParentModelContainer = modelScanningProps.ParentModelContainer;
            modelScanningProps.ParentModelContainer?.ModelRPCs.Add(modelRPCBase);

            AddRPC(modelRPCBase as IModelRPC);

            try
            {
                _additonalFunctionalities.ForEach(f => modelScanningProps = f.onModelRPC(modelRPCBase, elementProperty, modelScanningProps));
            }
            catch(Exception e)
            {
                ErrorMessageStack.Add(e.Message);
                return false;
            }


            return true;
        }

        private void ResolveVariableMaximumValueDependencies()
        {
            foreach (var variableMaximumValueDependencyPair in _variableMaximumValueDependencyDictionary)
            {
                var variable = variableMaximumValueDependencyPair.Key;
                var maximumValueVariable = Variables
                    .Where(variable => variable.ModelLink.Key.ToLower() == variableMaximumValueDependencyPair.Value.ToLower())
                    .FirstOrDefault();

                if (maximumValueVariable == null)
                    continue;

                // Set variable dependency for maximum value
                variable.SetValueMaximum(maximumValueVariable);
            }
        }

        private void ResolveVariableMinimumValueDependencies()
        {
            foreach (var variableMinimumValueDependencyPair in _variableMinimumValueDependencyDictionary)
            {
                var variable = variableMinimumValueDependencyPair.Key;
                var minimumValueVariable = Variables
                    .Where(variable => variable.ModelLink.Key.ToLower() == variableMinimumValueDependencyPair.Value.ToLower())
                    .FirstOrDefault();

                if (minimumValueVariable == null)
                    continue;

                // Set variable dependency for minimum value
                variable.SetValueMinimum(minimumValueVariable);
            }
        }

        #endregion
    }

    public class ScanningWrapperElement
    {
        public object ProxyElement { get; set; }
    }

    public struct ModelScanningProps
    {
        public Guid ModelId;
        public string ModelPath;
        public string ModelKey;
        public string LocalizationPath;
        public string RepositoryPath;
        public string RepositoryTypeName;
        public string RepositoryPartTypeName;
        public Guid RepositoryId;
        public string ServiceName;

        public List<ScanningTwinCAT3AttrubuteProps> TwinCAT3Links;

        public ScanningAttributeProps Attributes;

        public object Element;
        public ModelComponentContainer ParentModelContainer;
    }

    public struct ScanningTwinCAT3AttrubuteProps
    {
        public string Name;
        public string SymbolPath;
        public ModelVariableSamplingRateEnumeration SamplingRate;
        public bool IsIgnored;
    }

    public struct ScanningAttributeProps
    {
        public string LocalizationNamespace;
        public UnitValue? EngineeringUnit;
        public ModelIdentityUserRoleEnumeration VisibilityAuthorization;
        public ModelIdentityUserRoleEnumeration ManipulationAuthorization;
        public List<string> VisibilityConditions;
        public List<string> EditabilityConditions;
        public ModelRepositoryDataFlags? RepositoryDataFlags;
        public bool IsReadOnly;
        public List<string> Filters;
        public List<FilterTrigger> FilterTriggers;
        public int VariableValuePrecision;
    }
}
