using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Exceptions;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public abstract class Model : IModel
    {
        #region fields

        protected readonly ILoggerFactory _LoggerFactory;
        protected readonly ILogger<Model> _Logger;
        protected readonly IServiceScopeFactory _ServiceScopeFactory;

        protected ModelStateEnumeration _LastState = ModelStateEnumeration.Unknown;

        private volatile List<IServiceModelContainer> _ServiceModelContainers = new();
        private volatile List<IRepositoryModelContainer> _RepositoryModelContainers = new();
        private volatile List<IModelComponentContainerCollection> _EnumerableModelContainers = new();

        private volatile ModelComponentContainer _ModelComponentTree = new();
        private Dictionary<Guid, IModelVariable> _Variables = new();
        private volatile List<IModelRPC> _RPCs = new();

        #endregion
        #region ctor

        public Model(
            string name,
            IServiceScopeFactory serviceScopeFactory,
            ILoggerFactory loggerFactory)
        {
            Id = Guid.NewGuid();
            Name = name;

            _LoggerFactory = loggerFactory;
            _Logger = loggerFactory.CreateLogger<Model>();
            _ServiceScopeFactory = serviceScopeFactory;
        }

        #endregion
        #region methods

        protected virtual Task SetState(ModelStateEnumeration newState, CancellationToken cancellationToken)
        {
            // Set state
            _LastState = State;
            State = newState;
            _Logger.LogInformation($"Model [{Name}] changed state to [{newState}].");

            return Task.CompletedTask;
        }

        private static bool CompareModelKeyPredicate(string elementModelKey, string optionModelKey)
        {
            var splittedOptionModelLink = optionModelKey.Split(StringConstants.Separator);
            var splittedElementLink = elementModelKey.Split(StringConstants.Separator);

            if (splittedOptionModelLink.Length == splittedElementLink.Length)
            {
                return (elementModelKey.Equals(optionModelKey));
            }
            else if (splittedElementLink.Length > splittedOptionModelLink.Length)
            {
                var canCompare = true;
                for (int i = 0; i < splittedOptionModelLink.Length; i++)
                {
                    if (!splittedElementLink[i].Equals(splittedOptionModelLink[i]))
                    {
                        canCompare = false;
                        break;
                    }
                }

                if (!canCompare)
                    return false;

                return elementModelKey.Contains(optionModelKey);
            }

            return false;
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
        /// <exception cref="ModelRepositoryContainerArgumentException">
        /// Thrown when a model repository container not could be found with the given arguments.
        /// </exception>
        private void UpdateModelStructure(ModelRepositoryData modelRepositoryData, CancellationToken cancellationToken)
        {
            var exceptionText = $"Error on updating the model structure on model [{Name}].";

            // Get the affetced model repository container
            // At this point we can't query by the repository id to find the the correct instance in the model
            // The repository id will be set by the passed model repository data
            var affectedRepositoryContainers = _RepositoryModelContainers
                .Where(container => modelRepositoryData.Context.RepositoryTypeName == container.RepositoryTypeName)
                .Where(container => modelRepositoryData.Context.RepositoryRelatedModelLinks.Contains(container.ModelLink.Key))
                .ToList();

            try
            {
                // Update the repository id on the affected model repository container
                if (affectedRepositoryContainers.Count > 0)
                {
                    if (modelRepositoryData.Context.RepositoryRelatedModelLinks.Count == 1 &&
                        affectedRepositoryContainers.Count > 1)
                        throw new InvalidOperationException(exceptionText,
                            new ArgumentException($"Multiple repository containers named [{modelRepositoryData.Context.RepositoryTypeName}] were found for the defined criteria.", paramName: nameof(affectedRepositoryContainers.Count)));

                    foreach (var affectedRepositoryContainer in affectedRepositoryContainers)
                    {
                        // Take repository id from passed repository data
                        (affectedRepositoryContainer as ModelComponentContainer).RepositoryId = modelRepositoryData.Context.RepositoryId;
                    }
                }
                else
                {
                    throw new ModelRepositoryContainerArgumentException(Name, modelRepositoryData.Context.RepositoryTypeName, modelRepositoryData.Context.RepositoryRelatedModelLinks);
                }

                var addedEnumerableModelContainers = new List<IModelComponentContainerCollection>();

                // Update the model structure
                foreach (var modelContainer in modelRepositoryData.ModelContainers)
                {
                    if (!modelContainer.IsEnumerableContainer)
                        continue;

                    var affectedEnumerableModelContainers = _EnumerableModelContainers
                        .Where(container => container.RepositoryLink == modelContainer.RepositoryLink)
                        .Where(container => modelRepositoryData.Context.RepositoryRelatedModelLinks.Any(sharedModelLink => container.ModelLink.Key.StartsWith(sharedModelLink)))
                        .ToList();

                    // discover new added childreens
                    if (affectedEnumerableModelContainers.Count <= 0)
                    {
                        affectedEnumerableModelContainers = addedEnumerableModelContainers
                       .Where(container => container.RepositoryLink == modelContainer.RepositoryLink)
                       .Where(container => modelRepositoryData.Context.RepositoryRelatedModelLinks.Any(sharedModelLink => container.ModelLink.Key.StartsWith(sharedModelLink)))
                       .ToList();
                        if (affectedEnumerableModelContainers.Count <= 0)
                            continue;
                    }

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
                                var addedContainer = affectedEnumerableModelContainer.Containers.ElementAt(affectedEnumerableModelContainer.Count - 1);

                                ModelScanningProps scanningProps = new();
                                scanningProps.ModelPath = affectedEnumerableModelContainer.ModelLink.Key + $"[{affectedEnumerableModelContainer.Count}]";
                                scanningProps.RepositoryPath = affectedEnumerableModelContainer.RepositoryLink;

                                var seperator = affectedEnumerableModelContainer.ModelLink.Name + $"[{affectedEnumerableModelContainer.Count}].";
                                var containerChildreens = addedContainer.FindComponents<IModelComponentContainerCollection>(scanningProps);
                                foreach (var childreen in containerChildreens)
                                {
                                    var splitedModelLink = childreen.ModelLink.Key.Split(seperator);
                                    var parsedModelName = splitedModelLink[splitedModelLink.Length - 1];

                                    // Create repositorylink from parent container
                                    childreen.RepositoryLink = affectedEnumerableModelContainer.RepositoryLink + $"[{affectedEnumerableModelContainer.Count}]." + parsedModelName;
                                    addedEnumerableModelContainers.Add(childreen);
                                }

                            } while (affectedEnumerableModelContainer.Count != modelContainer.EnumerationCount);
                        }
                    }
                }
            }
            catch (ModelRepositoryContainerArgumentException ex)
            {
                throw ex;
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
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
        private void UpdateModelData(ModelRepositoryData modelRepositoryData, CancellationToken cancellationToken)
        {
            var exceptionText = $"Error on updating the model data on model [{Name}].";

            var affectedRepositoryModelContainers = RepositoryModelContainers
                .Where(container => container.RepositoryTypeName == modelRepositoryData.Context.RepositoryTypeName)
                .Where(container => container.RepositoryId == modelRepositoryData.Context.RepositoryId)
                .ToList();

            try
            {
                foreach (var affectedRepositoryModelContainer in affectedRepositoryModelContainers)
                {
                    var affectedRepositoryModelContainerVariables = affectedRepositoryModelContainer.FindComponents<ModelVariableBase>();

                    foreach (var newModelVariableData in modelRepositoryData.ModelVariables)
                    {
                        // Get the affected model variable
                        var affectedModelVariable = affectedRepositoryModelContainerVariables
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
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
        }

        #endregion

        #region IModel

        public Guid Id { get; }
        public string Name { get; }
        public ModelStateEnumeration State { get; private set; } = ModelStateEnumeration.Unknown;

        public List<IServiceModelContainer> ServiceModelContainers
        {
            get { return _ServiceModelContainers; }
            private set { _ServiceModelContainers = value; }
        }
        public List<IRepositoryModelContainer> RepositoryModelContainers
        {
            get { return _RepositoryModelContainers; }
            private set { _RepositoryModelContainers = value; }
        }
        public List<IModelComponentContainerCollection> EnumerableModelContainers
        {
            get { return _EnumerableModelContainers; }
            private set { _EnumerableModelContainers = value; }
        }
        public ModelComponentContainer ComponentTree
        {
            get { return _ModelComponentTree; }
            private set { _ModelComponentTree = value; }
        }
        public List<IModelVariable> Variables
        {
            get { return _Variables.Values.ToList(); }

        }
      
        public List<IModelRPC> RPCs
        {
            get { return _RPCs; }
            private set { _RPCs = value; }
        }

        public List<ModelCondition> Conditions { get; set; } = new List<ModelCondition>();
       
        public abstract Task StartAsync(CancellationToken cancellationToken);
        public abstract Task StopAsync(CancellationToken cancellationToken);

        public async Task<bool> ConsumeModelData(ModelUpdateOptions modelUpdateOptions, CancellationToken cancellationToken)
        {
            var exceptionText = $"Error on consuming model update data on model [{Name}].";

            // Release model data first
            var releaseModelDataResult = await ReleaseModelData(modelUpdateOptions, cancellationToken);
            if (!releaseModelDataResult)
                throw new InvalidOperationException(exceptionText,
                    new ArgumentException($"The affected model data could not be released.", paramName: nameof(releaseModelDataResult)));

            // Setup model scanner
            var modelComponentScanner = new ModelComponentScanner();
            modelComponentScanner.AddFunctionality(new ModelConditionScanner(modelComponentScanner, Conditions));
            
            var scannerProps = new ModelScanningProps();
            scannerProps.ModelId = Id;
            scannerProps.Element = this;
            var result = false;

            // Scan this model
            if (modelUpdateOptions.HasUpdateOptions)
            {
                scannerProps.ParentModelContainer = ComponentTree;

                modelComponentScanner.ScanningTriggers.ModelKeys = modelUpdateOptions.OptionModelKeys;
                modelComponentScanner.ScanningTriggers.ServiceNames = modelUpdateOptions.OptionServiceNames;
                modelComponentScanner.ScanningTriggers.TriggerContext.RepositoryNameContext = modelUpdateOptions.RepositoryNameContext;

                result = modelComponentScanner.Scan(scannerProps);
            }
            else
            {
                result = modelComponentScanner.Scan(scannerProps);
            }

            // Check scanning result
            if (!result)
                throw new ModelComponentScanningException($"Model_[{Name}]_ConsumeModelData", modelComponentScanner);

            if (!modelUpdateOptions.HasUpdateOptions)
            {
                ServiceModelContainers = modelComponentScanner.ServiceModelContainers;
                RepositoryModelContainers = modelComponentScanner.RepositoryModelContainers;
                EnumerableModelContainers = modelComponentScanner.EnumerableModelContainers;
                _Variables.Clear();
                modelComponentScanner.Variables.ForEach(variable => _Variables.TryAdd(variable.Id, variable));
                RPCs = modelComponentScanner.RPCs;
                ComponentTree = modelComponentScanner.ModelComponentTree;
            }
            else
            {
                ServiceModelContainers.AddRange(modelComponentScanner.ServiceModelContainers);
                RepositoryModelContainers.AddRange(modelComponentScanner.RepositoryModelContainers);
                EnumerableModelContainers.AddRange(modelComponentScanner.EnumerableModelContainers);
                modelComponentScanner.Variables.ForEach(variable => _Variables.TryAdd(variable.Id, variable));
                RPCs.AddRange(modelComponentScanner.RPCs);
            }     

            return true;
        }

        public async Task<bool> ConsumeModelData(ModelRepositoryData modelRepositoryData, CancellationToken cancellationToken)
        {
            var exceptionText = $"Error on consuming model repository data on model [{Name}].";

            try
            {
                if (modelRepositoryData == null)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentNullException(paramName: nameof(modelRepositoryData), $"The passed model repository data is null."));

                UpdateModelStructure(modelRepositoryData, cancellationToken);

                ModelUpdateOptions modelUpdateOptions = new(modelRepositoryData);
                var consumeResult = await ConsumeModelData(modelUpdateOptions, cancellationToken);
                if (!consumeResult)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"Consuming model update data failed.", paramName: nameof(consumeResult)));

                UpdateModelData(modelRepositoryData, cancellationToken);

                return true;
            }
            catch (ModelRepositoryContainerArgumentException ex)
            {
                // We only log this exeption
                // The exception is thrown when we load data from a repository that is no longer available in the model
                _Logger.LogError(ex.Message);
                return true;
            }
            catch (InvalidOperationException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
        }

        public Task<bool> ReleaseModelData(ModelUpdateOptions modelUpdateOptions, CancellationToken cancellationToken)
        {
            if (!modelUpdateOptions.HasUpdateOptions)
            {
                ServiceModelContainers.Clear();
                RepositoryModelContainers.Clear();
                EnumerableModelContainers.Clear();
                Variables.Clear();
                RPCs.Clear();
                Conditions.ForEach(c => c.ClearObservers());
            }
            else
            {
                List<IModelConditionObserver> removeableModelConditionObservers = new();

                if (modelUpdateOptions.OptionModelKeys.Count > 0)
                { 
                    Variables.Where(element => modelUpdateOptions.OptionModelKeys
                        .Any(optionModelKey => CompareModelKeyPredicate(element.ModelLink.Key, optionModelKey)))
                        .ToList()
                        .ForEach(v => removeableModelConditionObservers.Add(v as IModelConditionObserver));

                    RPCs.Where(element => modelUpdateOptions.OptionModelKeys
                        .Any(optionModelKey => CompareModelKeyPredicate(element.ModelLink.Key, optionModelKey)))
                        .ToList()
                        .ForEach(rpc => removeableModelConditionObservers.Add(rpc as IModelConditionObserver));

                    Conditions.ForEach(condition => condition.RemoveObserver(removeableModelConditionObservers));

                    ServiceModelContainers
                        .RemoveAll(element => modelUpdateOptions.OptionModelKeys.Any(optionModelKey => CompareModelKeyPredicate(element.ModelLink.Key, optionModelKey)));

                    RepositoryModelContainers
                        .RemoveAll(element => modelUpdateOptions.OptionModelKeys.Any(optionModelKey => CompareModelKeyPredicate(element.ModelLink.Key, optionModelKey)));

                    EnumerableModelContainers
                        .RemoveAll(element => modelUpdateOptions.OptionModelKeys.Any(optionModelKey => CompareModelKeyPredicate(element.ModelLink.Key, optionModelKey)));

                    var variablesToRemove = Variables
                        .Where(element => modelUpdateOptions.OptionModelKeys.Any(optionModelKey => CompareModelKeyPredicate(element.ModelLink.Key, optionModelKey))).Select(v=>v.Id).ToList();

                    variablesToRemove.ForEach(v => _Variables.Remove(v));
                    

                    RPCs
                        .RemoveAll(element => modelUpdateOptions.OptionModelKeys.Any(optionModelKey => CompareModelKeyPredicate(element.ModelLink.Key, optionModelKey)));
                }

                if (modelUpdateOptions.OptionModelComponentIds.Count > 0)
                {
                    Variables.Where(element => modelUpdateOptions.OptionModelComponentIds
                        .Any(id => element.Id == id))
                        .ToList()
                        .ForEach(v => removeableModelConditionObservers.Add(v as IModelConditionObserver));

                    RPCs.Where(element => modelUpdateOptions.OptionModelComponentIds
                        .Any(id => element.Id == id))
                        .ToList()
                        .ForEach(rpc => removeableModelConditionObservers.Add(rpc as IModelConditionObserver));

                    Conditions.ForEach(condition => condition.RemoveObserver(removeableModelConditionObservers));

                    ServiceModelContainers
                        .RemoveAll(element => modelUpdateOptions.OptionModelComponentIds.Any(id => element.Id == id));

                    RepositoryModelContainers
                        .RemoveAll(element => modelUpdateOptions.OptionModelComponentIds.Any(id => element.Id == id));

                    EnumerableModelContainers
                        .RemoveAll(element => modelUpdateOptions.OptionModelComponentIds.Any(id => element.Id == id));

                    modelUpdateOptions.OptionModelComponentIds.ForEach(element => _Variables.Remove(element));

                    //Variables
                    //    .RemoveAll(element => modelUpdateOptions.OptionModelComponentIds.Any(id => element.Id == id));

                    RPCs
                        .RemoveAll(element => modelUpdateOptions.OptionModelComponentIds.Any(id => element.Id == id));
                }

                if (modelUpdateOptions.OptionServiceNames.Count > 0)
                {
                    Variables
                      .Where(element => modelUpdateOptions.OptionServiceNames
                      .Any(serviceName => element.ServiceName == serviceName))
                      .ToList()
                      .ForEach(variable => removeableModelConditionObservers.Add(variable as IModelConditionObserver));

                    RPCs
                      .Where(element => modelUpdateOptions.OptionServiceNames
                      .Any(serviceName => element.ServiceName == serviceName))
                      .ToList()
                      .ForEach(rpc => removeableModelConditionObservers.Add(rpc as IModelConditionObserver));

                    Conditions.ForEach(condition => condition.RemoveObserver(removeableModelConditionObservers));

                    ServiceModelContainers
                        .RemoveAll(element => modelUpdateOptions.OptionServiceNames.Any(serviceName => element.ServiceName == serviceName));
                    RepositoryModelContainers
                        .RemoveAll(element => modelUpdateOptions.OptionServiceNames.Any(serviceName => element.ServiceName == serviceName));
                    EnumerableModelContainers
                        .RemoveAll(element => modelUpdateOptions.OptionServiceNames.Any(serviceName => element.ServiceName == serviceName));
                    Variables
                        .Where(element => modelUpdateOptions.OptionServiceNames.Any(serviceName => element.ServiceName.Equals(serviceName)))
                        .Select(v=> v.Id).ToList()
                        .ForEach(id=> _Variables.Remove(id));
                    RPCs
                        .RemoveAll(element => modelUpdateOptions.OptionServiceNames.Any(serviceName => element.ServiceName == serviceName));
                }
            }

            return Task.FromResult(true);
        }

        public Task<bool> PushVariableData(IEnumerable<IModelVariableDEO> modelVariableDEOs, CancellationToken cancellationToken)
        {
            // Pre-sorting the affected data
            var modelVariableDEOLinks = modelVariableDEOs
                .Select(deo => deo.ModelContext.ModelLink.Key)
                .ToList();
            var affectedVariables = Variables
                .Where(variable => modelVariableDEOLinks.Contains(variable.ModelLink.Key))
                .ToList();

            try
            {
                foreach (var modelVariableDEO in modelVariableDEOs)
                {
                    if (modelVariableDEO == null)
                        throw new ArgumentNullException(paramName: nameof(modelVariableDEO), $"Variable data could not be pushed to the model [{Name}]. A passed model variable DEO elemnt is null.");

                    IModelVariable modelVariable = affectedVariables
                        .FirstOrDefault(variable => variable.Id == modelVariableDEO.ModelContext.Id);
                        
                    if (modelVariable == null)
                        throw new ArgumentNullException(paramName: nameof(modelVariable), $"Variable data could not be pushed to the model [{Name}]. A corresponding model variable could not be resolved for [{modelVariableDEO.ModelContext.ModelLink.Key}] with id [{modelVariableDEO.ModelContext.Id}].");

                    modelVariable.Take(modelVariableDEO);
                }
            }
            catch (Exception ex)
            {
                throw new PushModelVariableDataException(Name, modelVariableDEOs, ex);
            }

            return Task.FromResult(true);
        }

        public List<IModelVariable> FindModelVariablesByID(List<Guid> IDs)
        {
            var foundVariables = new List<IModelVariable>();
            foreach (var id in IDs)
            {
                var foundVariable = _Variables.GetValueOrDefault(id);         
                if(foundVariable != null)
                {
                    foundVariables.Add(foundVariable);
                }
            }

            return foundVariables;
        }

        #endregion
    }

    public class ModelUpdateOptions
    {
        public ModelUpdateOptions() { }
        public ModelUpdateOptions(ModelRepositoryData modelRepositoryData)
        {
            OptionModelKeys = modelRepositoryData.Context.RepositoryRelatedModelLinks;
            RepositoryIdContext = modelRepositoryData.Context.RepositoryId;
            RepositoryNameContext = modelRepositoryData.Context.RepositoryTypeName;
        }

        public List<string> OptionModelKeys { get; set; } = new();
        public List<Guid> OptionModelComponentIds { get; set; } = new();
        public List<string> OptionServiceNames { get; set; } = new();
        public bool HasUpdateOptions
        {
            get
            {
                return OptionModelKeys.Count > 0 ||
                    OptionServiceNames.Count > 0 ||
                    OptionModelComponentIds.Count > 0;
            }
        }

        public Guid RepositoryIdContext { get; set; } = Guid.Empty;
        public string RepositoryNameContext { get; set; } = string.Empty;
    }

    public class ModelInstanceOptions
    {
        public List<string> GeneralContainerFilters { get; set; } = new();
        public List<string> VariableSourceContainerFilters { get; set; } = new();
    }
}
