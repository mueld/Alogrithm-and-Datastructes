using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    public class ModelComponentContainer : ModelComponent, IModelComponentContainer
    {
        #region ctor

        public ModelComponentContainer()
        {}

        public ModelComponentContainer(IModelComponentContainerCollection modelComponentContainerCollection)
        {
        }

        #endregion
        #region methods

        public override bool ShouldSerializeId()
        {
            return false;
        }
        public bool ShouldSerializeDisplay()
        {
            return false;
        }
        public bool ShouldSerializeDescription()
        {
            return false;
        }
        public bool ShouldSerializeVisibilityAuthorization()
        {
            return false;
        }
        public bool ShouldSerializeManipulationAuthorization()
        {
            return false;
        }
        public bool ShouldSerializeIsRepositoryClient()
        {
            return false;
        }
        public bool ShouldSerializeRepositoryModelContainer()
        {
            return false;
        }
        public bool ShouldSerializeModelContainers()
        {
            return false;
        }
        public bool ShouldSerializeEnumerableModelContainers()
        {
            return false;
        }
        public bool ShouldSerializeModelVariables()
        {
            return false;
        }
        public bool ShouldSerializeModelRPCs()
        {
            return false;
        }
        public bool ShouldSerializeCollectedVariables()
        {
            return false;
        }

        /// <summary>
        /// Finds recursively all <see cref="ModelVariableBase"/> types in the passed model component container. 
        /// </summary>
        /// <param name="modelComponentContainer">The model component container to look up.</param>
        /// <returns>
        /// A lsit of the found <see cref="ModelVariableBase"/> types.
        /// </returns>
        private List<ModelVariableBase> FindVariables(ModelComponentContainer modelComponentContainer)
        {
            var collectedVariableList = new List<ModelVariableBase>();
            collectedVariableList.AddRange(modelComponentContainer.ModelVariables);

            foreach (var mcc in modelComponentContainer.ModelContainers)
                collectedVariableList.AddRange(FindVariables(mcc));

            return collectedVariableList;
        }

        /// <summary>
        /// Finds recursively all <see cref="ModelComponentContainer"/> types in the passed model component container. 
        /// </summary>
        /// <param name="modelComponentContainer">The model component container to look up.</param>
        /// <returns>
        /// A lsit of the found <see cref="ModelComponentContainer"/> types.
        /// </returns>
        private List<ModelComponentContainer> FindContainers(ModelComponentContainer modelComponentContainer)
        {
            var collectedContainerList = new List<ModelComponentContainer>();
            collectedContainerList.AddRange(modelComponentContainer.ModelContainers);

            foreach (var mcc in modelComponentContainer.ModelContainers)
                collectedContainerList.AddRange(FindContainers(mcc));

            return collectedContainerList;
        }

        /// <summary>
        /// Finds recursively all <see cref="ModelComponentContainer"/> types with a implementation of <typeparamref name="T"/> in the passed model component container. 
        /// </summary>
        /// <param name="modelComponentContainer">The model component container to look up.</param>
        /// <returns>
        /// A lsit of the found <see cref="ModelComponentContainer"/> types.
        /// </returns>
        private List<ModelComponentContainer> FindContainers<T>(ModelComponentContainer modelComponentContainer)
                
        {
            var collectedContainerList = new HashSet<ModelComponentContainer>();
            //if (modelComponentContainer.GetType().GetInterfaces().Contains(typeof(T)))
            //{
            //    collectedContainerList.Add(modelComponentContainer);
            //}

            foreach (var mcc in modelComponentContainer.ModelContainers)
            {
                if(mcc.GetType().GetInterfaces().Contains(typeof(T)))
                {
                    collectedContainerList.Add(mcc);
                }
                    FindContainers<T>(mcc).ForEach(c => collectedContainerList.Add(c));
                    //collectedContainerList.AddRange(FindContainers<T>(mcc));
                
            }

            return collectedContainerList.ToList();
        }

        /// <summary>
        /// Finds recursively all <see cref="IModelComponentContainerCollection"/> types in the passed model component container. 
        /// </summary>
        /// <param name="modelComponentContainer">The model component container to look up.</param>
        /// <returns>
        /// A lsit of the found <see cref="IModelComponentContainerCollection"/> types.
        /// </returns>
        private List<IModelComponentContainerCollection> FindEnumerableContainers(ModelComponentContainer modelComponentContainer)
        {
            var collectedEnumerableContainerList = new List<IModelComponentContainerCollection>();
            collectedEnumerableContainerList.AddRange(modelComponentContainer.EnumerableModelContainers);

            foreach (var emcc in modelComponentContainer.ModelContainers)
                collectedEnumerableContainerList.AddRange(FindEnumerableContainers(emcc));

            return collectedEnumerableContainerList;
        }

        /// <summary>
        /// Updates the model structure by the passed model repository data. 
        /// </summary>
        /// <remarks>
        /// The model structure is represented by the used <see cref="IModelComponentContainerCollection"/> types and its children. So update the model structure means to add or remove child elements in these containers based on the Information of the passed <paramref name="modelRepositoryData"/> parameter.
        /// </remarks>
        /// <param name="modelRepositoryData">Contains the model repository data to update the model structure.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model structure could not be updated.
        /// </exception>
        private void UpdateModelStructure(ModelRepositoryData modelRepositoryData)
        {
            var exceptionText = $"Error on updating the model structure on model component container [{ModelLink.Key}].";

            try
            {
                var enumerableModelContainers = FindComponents<IModelComponentContainerCollection>();

                // Update model structure
                foreach (var modelContainer in modelRepositoryData.ModelContainers)
                {
                    if (!modelContainer.IsEnumerableContainer)
                        continue;

                    var affectedEnumerableModelContainers = enumerableModelContainers
                        .Where(container => container.RepositoryLink == modelContainer.RepositoryLink)
                        .ToList();

                    if (affectedEnumerableModelContainers.Count <= 0)
                        continue;

                    foreach (var affectedEnumerableModelContainer in affectedEnumerableModelContainers)
                    {
                        if (affectedEnumerableModelContainer.Count == modelContainer.EnumerationCount)
                            continue;

                        if (affectedEnumerableModelContainer.Count > modelContainer.EnumerationCount)
                        {
                            do
                            {
                                affectedEnumerableModelContainer.RemoveModelContainer();
                            } while (affectedEnumerableModelContainer.Count != modelContainer.EnumerationCount);
                        }
                        else
                        {
                            do
                            {
                                affectedEnumerableModelContainer.AddModelContainer();
                            } while (affectedEnumerableModelContainer.Count != modelContainer.EnumerationCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
        }

        /// <summary>
        /// Updates the model data by the passed model repository data. 
        /// </summary>
        /// <remarks>
        /// The model data is represented by the used <see cref="IModelVariable"/> types. So update the model data means to update the value of these model variables based on the Information of the passed <paramref name="modelRepositoryData"/> parameter.
        /// </remarks>
        /// <param name="modelRepositoryData">Contains the model repository data to update the model structure.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model data could not be updated.
        /// </exception>
        private void UpdateModelData(ModelRepositoryData modelRepositoryData)
        {
            var exceptionText = $"Error on updating the model data on model component container [{ModelLink.Key}].";

            try
            {
                var modelVariables = FindComponents<ModelVariableBase>();

                foreach (var newModelVariableData in modelRepositoryData.ModelVariables)
                {
                    // Get the affected model variable
                    var affectedModelVariable = modelVariables
                        .FirstOrDefault(variable => variable.RepositoryLink == newModelVariableData.RepositoryLink);

                    // Check if the model variable is present
                    // If no suitable model variable was found, it means that we have too few filter criteria
                    if (affectedModelVariable == null)
                    {
                        continue;
                    }

                    affectedModelVariable.Take(newModelVariableData as IModelVariable, false, true);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
        }

        #endregion

        #region IModelComponentContainer

        [NotMapped]
        public LocalizableValue Display { get; set; } = new();
        [NotMapped]
        public LocalizableValue Description { get; set; } = new();

        [NotMapped]
        public ModelIdentityUserRoleEnumeration VisibilityAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;
        [NotMapped]
        public ModelIdentityUserRoleEnumeration ManipulationAuthorization { get; set; } = ModelIdentityUserRoleEnumeration.Undefined;

        [NotMapped]
        public List<string> VisibilityConditions { get; set; } = new();
        [NotMapped]
        public List<string> EditabilityConditions { get; set; } = new();

        public bool IsEnumerableContainer { get; set; } = false;
        public bool IsEnumerable { get; set; } = false;
        public int EnumerationIndex { get; set; } = -1;
        public int EnumerationCount { get; set; } = -1;

        [NotMapped]
        public bool IsRepositoryClient { get; set; } = false;
        public virtual string RepositoryTypeName { get; set; } = string.Empty;
        public virtual string RepositoryPartTypeName { get; set; } = string.Empty;
        public Guid RepositoryId { get; set; } = Guid.Empty;
        public string RepositoryLink { get; set; } = string.Empty;
        public bool IsRepositoryRootContainer { get; set; } = false;

        [NotMapped]
        public List<IModelComponentContainerCollection> EnumerableModelContainers { get; set; } = new();
        public List<ModelComponentContainer> ModelContainers { get; set; } = new();
        public List<ModelVariableBase> ModelVariables { get; set; } = new();
        [NotMapped]
        public List<ModelRPCBase> ModelRPCs { get; set; } = new();

        public bool Scan(ModelScanningProps scanningProps)
        {
            // Set this container as the root element
            scanningProps.Element = this;
            //scannerProps.ParentModelContainer = this;

            EnumerableModelContainers.Clear();
            ModelContainers.Clear();
            ModelVariables.Clear();
            ModelRPCs.Clear();

            // Setup model scanner and scan this model container and its structure
            var modelComponentScanner = new ModelComponentScanner();
            modelComponentScanner.WrapSourceObject = true;
            return modelComponentScanner.Scan(scanningProps);
        }

        public object? Clone(ModelComponentContainerCloneOptions? cloneOptions = default)
        {
            var exceptionText = $"Error on cloning model component container [{ModelLink.Key}].";

            try
            {
                // Activate new instance
                var clone = Activator.CreateInstance(GetType());
                if (clone == null)
                    throw new ArgumentNullException(paramName: nameof(clone), $"New instance of this type is null.");

                // Cast clone to a IModelComponentContainer
                var mccClone = clone as IModelComponentContainer;
                if (mccClone == null)
                    throw new ArgumentNullException(paramName: nameof(clone), $"New instance of this type as {nameof(IModelComponentContainer)} is null.");

                // Setup regular scanning props
                var scanningProps = new ModelScanningProps();
                scanningProps.ModelPath = ModelLink.Key;
                scanningProps.LocalizationPath = Display.Link.Path;
                scanningProps.ServiceName = ServiceName;
                scanningProps.Attributes.Filters = Filters;

                scanningProps.TwinCAT3Links = new();
                foreach (var tc3Link in TwinCAT3Links)
                {
                    var tc3ScanningProps = new ScanningTwinCAT3AttrubuteProps
                    {
                        SamplingRate = tc3Link.SamplingRate,
                        IsIgnored = tc3Link.Ignore,
                        SymbolPath = tc3Link.SymbolKey.Key,
                        Name = tc3Link.Name
                    };

                    scanningProps.TwinCAT3Links.Add(tc3ScanningProps);
                }

                // Check for passed clone options
                if (cloneOptions != default)
                {
                    // Clone options for the clone itslef 
                    mccClone.RepositoryId = cloneOptions.RepositoryId;

                    // Clone options for the scanning props
                    scanningProps.Attributes.RepositoryDataFlags = cloneOptions.RepositoryDataFlags;
                    scanningProps.Attributes.LocalizationNamespace = cloneOptions.LocalizationNamespace;
                }

                // Scan the clone
                var consumeResult = mccClone.Scan(scanningProps);
                if (!consumeResult)
                    throw new ArgumentException($"Scanning of the new instance has failed.", paramName: nameof(consumeResult));

                // Set the repository related model links if available
                if (this is IRepositoryModelContainer sourceRepositoryModelContainer &&
                    clone is IRepositoryModelContainer)
                {
                    (clone as IRepositoryModelContainer).RepositoryRelatedModelLinks = (this as IRepositoryModelContainer).RepositoryRelatedModelLinks;
                }

                if (cloneOptions == default)
                    return clone;

                if (cloneOptions.CloneEntityIDs)
                {
                    // Clone entity ids of all variables
                    var sourceModelVariables = FindComponents<ModelVariableBase>();
                    var cloneModelVariables = mccClone.FindComponents<ModelVariableBase>();
                    if (sourceModelVariables.Count == cloneModelVariables.Count)
                    {
                        for (int i = 0; i < sourceModelVariables.Count; i++)
                        {
                            cloneModelVariables[i].Id = sourceModelVariables[i].Id;
                        }
                    }
                }

                return clone;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
        }

        public List<T> FindComponents<T>()
            where T : IModelComponent
        {
            if (typeof(T) == typeof(ModelComponentContainer))
            {
                var result = FindContainers(this);
                return result as List<T>;
            }
            else if (typeof(T) == typeof(IModelComponentContainerCollection))
            {
                var result = FindEnumerableContainers(this);
                return result as List<T>;
            }
            else if (typeof(T) == typeof(ModelVariableBase))
            {
                var result = FindVariables(this);
                return result as List<T>;
            }
            else
            {
                return new List<T>();
            }
        }

        public List<T> FindComponents<T>(ModelScanningProps scanningProps)
          where T : IModelComponent
        {
            var scanResult = Scan(scanningProps);
            if (!scanResult)
            {
                // Handle?
                return new List<T>();
            }

            return FindComponents<T>();
        }

        public List<ModelComponentContainer> FindExplicitContainers<T>()
        {
            return FindContainers<T>(this);
        }

        public bool ConsumeModelData(ModelRepositoryData modelRepositoryData)
        {
            var exceptionText = $"Error on consuming model repository data on model component container [{ModelLink.Key}].";

            if (modelRepositoryData == null)
                throw new InvalidOperationException(exceptionText,
                    new ArgumentNullException(paramName: nameof(modelRepositoryData), $"The passed model repository data is null."));

            if (modelRepositoryData.Context.RepositoryTypeName != RepositoryTypeName)
                throw new InvalidOperationException(exceptionText,
                    new ArgumentException($"The passed repository type name [{modelRepositoryData.Context.RepositoryTypeName}] does not equals [{RepositoryTypeName}] on model component container [{ModelLink.Key}].", paramName: nameof(modelRepositoryData.Context.RepositoryTypeName)));

            try
            {
                UpdateModelStructure(modelRepositoryData);
                UpdateModelData(modelRepositoryData);

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
        }

        [NotMapped]
        [Obsolete("This Prop is Deprecated. Use FindComponents instead.", false)]
        public List<ModelVariableBase> CollectedVariables { get; set; } = new();
        [Obsolete("This Method is Deprecated. Use FindComponents instead.", false)]
        public bool CollectVariables()
        {
            var scannerProps = new ModelScanningProps
            {
                Element = this,
                ModelPath = ModelLink.Key
            };

            return CollectVariables(scannerProps);
        }
        [Obsolete("This Method is Deprecated. Use FindComponents instead.", false)]
        public bool CollectVariables(ModelScanningProps scannerProps)
        {
            // Prepare local values
            CollectedVariables.Clear();

            // Setup model scanner and scan this model container and its structure
            var modelComponentScanner = new ModelComponentScanner();
            //modelComponentScanner.FillVariableMetaData = collectMetadata;

            scannerProps.Element = this;

            var result = modelComponentScanner.Scan(scannerProps);

            CollectedVariables = modelComponentScanner.Variables.Cast<ModelVariableBase>().ToList();

            // Order variables by the "RepositoryIndex" prop
            CollectedVariables = CollectedVariables
                .OrderBy(variable => variable.ScanningIndex)
                .ToList();

            return result;
        }
        [Obsolete("This Method is Deprecated. You should remove all dependencies.", false)]
        public bool DistributeVariables()
        {
            // We need variables for distribution
            if (CollectedVariables.Count <= 0)
                return false;

            // Order variables by the "DbIndex" prop
            // This ensures that we distribute the variables in the same order as we collected them and stored them in the database
            CollectedVariables = CollectedVariables
                .OrderBy(variable => variable.ScanningIndex)
                .ToList();

            // Setup model scanner and scan this model container and its structure
            var modelComponentScanner = new ModelComponentScanner();
            modelComponentScanner.DistributeVariables = true;
            modelComponentScanner.DistributionVariables = CollectedVariables.Cast<IModelVariable>().ToList();

            return modelComponentScanner.Scan(this);
        }

        #endregion
    }

    public class ModelComponentContainerCloneOptions
    {
        public ModelRepositoryDataFlags? RepositoryDataFlags { get; set; } = null;
        public Guid RepositoryId { get; set; } = Guid.Empty;
        public string LocalizationNamespace { get; set; } = string.Empty;
        public bool CloneEntityIDs { get; set; } = false;
    }
}
