using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Commands;
using Bruderer.Core.Domain.Exceptions;
using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ComponentAggregate;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Response;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Infrastructure.PLC.Connection;
using Bruderer.Core.Infrastructure.PLC.TwinCAT3.Connection;
using Bruderer.Core.Infrastructure.PLC.TwinCAT3.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.Ads.SumCommand;
using TwinCAT.TypeSystem;

namespace Bruderer.Core.Infrastructure.PLC.TwinCAT3
{
    public abstract class TC3ModelUser : PLCModelUser
    {
        #region fields

        protected volatile Dictionary<Guid, TC3Variable> _TC3VariableDictionay = new Dictionary<Guid, TC3Variable>();

        protected volatile Dictionary<ModelVariableSamplingRateEnumeration, List<TC3VariableBlock>> _SamplingRateVariableBlocksDictionary = new();
        private volatile List<SingleSamplingData> _SingleSamplingDataList = new();

        private static SemaphoreSlim _ReadContinuousSamplingVariableSemaphore = new(1, 1);
        private static SemaphoreSlim _ReadSingleSamplingVariableSemaphore = new(1, 1);
        private static SemaphoreSlim _ReadVariableBlockSemaphore = new(1, 1);
        private static SemaphoreSlim _WriteVariableBlockSemaphore = new(1, 1);
        private static SemaphoreSlim _InvokeRpcSemaphore = new(1, 1);
        private const int VARIABLE_BLOCK_SIZE = 500;

        // TwinCAT specific fields
        private volatile ISymbolCollection<ISymbol> _SessionSymbols;
        private volatile bool _IsConnected = false;
        private volatile bool _IsDisconnected = false;
        private volatile bool _IsRunning = false;
        private volatile bool _IsStopped = false;
        private long _TicksOnLastProcessingLoop = 0;
        private int _ProcessPeriodTimeCounter = 0;

        private readonly ILogger<PLCModelUser> _Logger;
        private TC3Variable _OnlineChangeCntTwinCATVariable = null;
        private bool _OnlineChangeOccured = false;

        #endregion
        #region ctor

        public TC3ModelUser(
            string name,
            IServiceScopeFactory serviceScopeFactory,
            IModelFactory modelFactory,
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
            : base(name, serviceScopeFactory, modelFactory, loggerFactory, configuration)
        {
            _Logger = _LoggerFactory.CreateLogger<PLCModelUser>();

            ConnectionProps = GetConnectionProps();
            if (ConnectionProps == null)
            {
                _Logger.LogError($"Connection props not available. ADS session can not be created.");
            }
            else
            {
                AdsSession = new TC3Session(ConnectionProps, loggerFactory);
            }
        }

        #endregion
        #region props

        public TC3ConnectionProps ConnectionProps { get; private set; } = null;
        public TC3Session AdsSession { get; private set; } = null;

        #endregion
        #region methods

        /// <summary>
        /// Adds a new model variable to the internal store for further usage
        /// </summary>
        /// <returns>
        /// The added <see cref="TC3Variable"/> type.
        /// </returns>
        /// <param name="modelId">ID of the model to which the variable belongs.</param>
        /// <param name="modelVariable">Variable that should be added to the store.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the pased model variable is null
        /// </exception>
        public TC3Variable AddVariable(Guid modelId, IModelVariable modelVariable)
        {
            if (modelVariable == null)
                throw new ArgumentNullException(paramName: nameof(modelVariable), "Passed model variable is null.");

            var tc3Variable = new TC3Variable(modelId, modelVariable);

            //use hashset to check if tc3Variable is already registered.
            _TC3VariableDictionay.Add(modelVariable.Id, tc3Variable);

            //if (_UniqueTwinCAT3VariableList.Add(tc3Variable))
            //{
            //    _TwinCAT3Variables.Add(tc3Variable);
            //}
            return tc3Variable;
        }

        /// <summary>
        /// Adds a new TwinCAT3 variable to the internal store for further usage
        /// </summary>
        /// <param name="twinCAT3Variable">TwinCAT3 variable that should be added to the store.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the pased TwinCAT 3 variable is null
        /// </exception>
        protected void AddVariable(TC3Variable twinCAT3Variable)
        {
            if (twinCAT3Variable == null)
                throw new ArgumentNullException(paramName: nameof(twinCAT3Variable), "Transferred TwinCAT 3 variable is null.");

            _TC3VariableDictionay.Add(twinCAT3Variable.ModelVariable.Id, twinCAT3Variable);

            //use hashset to check if tc3Variable is already registered.
            //if (_UniqueTwinCAT3VariableList.Add(twinCAT3Variable))
            //{
            //    _TwinCAT3Variables.Add(twinCAT3Variable);
            //}
        }

        protected abstract TC3ConnectionProps GetConnectionProps();

        private void RegisterSystemVariables()
        {
            // Setup online change detection
            ModelVariable<uint> OnlineChangeCnt = new ModelVariable<uint>();
            var twinCAT3Link = new ModelTwinCAT3Link();
            twinCAT3Link.Name = Name;
            twinCAT3Link.Platform = ModelUserPlatformEnumeration.TwinCAT3;
            twinCAT3Link.SamplingRate = ModelVariableSamplingRateEnumeration.ms1000;
            twinCAT3Link.SymbolKey.Path = "TwinCAT_SystemInfoVarList";
            twinCAT3Link.SymbolKey.Name = "_AppInfo.OnlineChangeCnt";

            OnlineChangeCnt.TwinCAT3Links.Add(twinCAT3Link);
            OnlineChangeCnt.IsReadOnly = true;
            OnlineChangeCnt.ValueChanged += OnlineChangeCnt_ValueChanged;

             _OnlineChangeCntTwinCATVariable = new TC3Variable(Guid.Empty, OnlineChangeCnt);
            AddVariable(_OnlineChangeCntTwinCATVariable);
        }

        private IModelRPC FindRPCMethod(Guid rpcId)
        {
            // Get used models by this model user
            var usedModels = _ModelFactory.GetUsedModels(Id).ToList();
            if (usedModels.Count <= 0)
            {
                _Logger.LogError($"No model available in {nameof(TC3ModelUser)}. Please add a model with the 'UseModel<T>' method first. Finding Rpc method aborted.");
                return null;
            }

            // Setup return value
            IModelRPC rpcMethod = null;

            // Find the corresponding rpc method in the models
            for (int modelIndex = 0; modelIndex < usedModels.Count; modelIndex++)
            {
                // Check the model
                var model = usedModels[modelIndex];
                if (model == null)
                {
                    _Logger.LogError($"A used model in {nameof(TC3ModelUser)} is null. Finding Rpc method aborted.");
                    continue;
                }

                rpcMethod = model.RPCs.Find(rpcMethod => rpcMethod.Id == rpcId);
                if (rpcMethod != null)
                    break;
            }

            return rpcMethod;
        }

        private Task<SingleSamplingData> CheckFilterTrigger(ModelComponent modelComponent)
        { 
            var singelSamplingVariables = new SingleSamplingData();

            try
            {
                var filters = new List<string>();
                var filterTriggers = modelComponent.GetFilterTriggers();
               
                foreach (var filterTrigger in filterTriggers)
                {
                    if (filterTrigger.Action != FilterTriggerActionEnumeration.Sampling)
                        continue;

                    filters.AddRange(filterTrigger.Filters);
                }

                if (filters.Count < 1)
                {
                    return Task.FromResult(singelSamplingVariables);
                }

                filters = filters
                    .Distinct()
                    .ToList();

                // Get affected TwinCAT3 variables
                var affectedTwinCATVariables = _TC3VariableDictionay.Values
                    .Where(tc3Variable => tc3Variable.ModelVariable.Filters.Intersect(filters).Any())
                    .ToList();

                singelSamplingVariables = new SingleSamplingData(affectedTwinCATVariables);

                return Task.FromResult(singelSamplingVariables);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.ToString());
                
            }

            return Task.FromResult(singelSamplingVariables);
        }

        public async Task<IResponse<bool>> SendPushModelVariablesCommand(Dictionary<Guid, List<IModelVariableDEO>> modelVariableDEODictionary, CancellationToken cancellationToken)
        {
            // Setup response object
            IResponse<bool> response = new Response<bool>(false);

            // Return if no updated values are present
            if (modelVariableDEODictionary.Count <= 0)
            {
                response.Payload = true;
                response.Result = ResponseResultEnumeration.Success;
                return response;
            } 

            // Send command with changed variables for each model
            foreach (var modelVariableValuePair in modelVariableDEODictionary)
            {
                // Setup command
                var command = new ModelVariablesValueChangeCommand(modelVariableValuePair.Key, modelVariableValuePair.Value, Id, Name);

                using var scope = _ServiceScopeFactory.CreateScope();
                var messageBus = scope.ServiceProvider.GetService<IMessageBus>();
                var commandResponse = await messageBus.SendCommand(command, cancellationToken);
            }

            response.Payload = true;
            response.Result = ResponseResultEnumeration.Success;
            return response;
        }

        /// <summary>
        /// Reads and evaluates the TwinCAT 3 specific state conditions.
        /// </summary>
        /// <remarks>
        /// This method will automatically be executed by the <see cref="ProcessLoop"/> method.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the linking of model variables failed.
        /// </exception>
        private async Task CheckStates(CancellationToken cancellationToken)
        {
            var exceptionText = $"Error on checking TwinCAT 3 states on model user [{Name}].";

            try
            {
                await AdsSession.CheckConnection(cancellationToken);
                if (AdsSession.ConnectionState == TwinCAT.ConnectionState.Connected && !_IsConnected)
                {
                    _IsConnected = true;
                    _IsDisconnected = false;

                    // Link the variables with a corresponding TwinCAT 3 symbol
                    var modelUdateOptions = new ModelUpdateOptions
                    {
                        OptionModelKeys = _TC3VariableDictionay.Select((kvPair) => kvPair.Value.ModelVariable.ModelLink.Key).ToList()
                        //OptionModelKeys = _TwinCAT3Variables.Select(v => v.ModelVariable.ModelLink.Key).ToList()
                    };

                    await LinkVariables(modelUdateOptions, cancellationToken);

                    // Create variable blocks with 500 variables per sampling rate property
                    await CreateSamplingRateVariableBlocks();

                    await SetConnectionState(PLCConnectionStateEnumeration.Connected, cancellationToken);
                }

                if (AdsSession.ConnectionState != TwinCAT.ConnectionState.Connected && !_IsDisconnected)
                {
                    await SetConnectionState(PLCConnectionStateEnumeration.Error, cancellationToken);
                    await SetRuntimeState(PLCRuntimeStateEnumeration.Stopped, cancellationToken);

                    _IsConnected = false;
                    _IsDisconnected = true;
                    _IsRunning = false;
                    _IsStopped = true;
                }

                // PLC runtime state change to 'Running'
                if ((AdsSession.AdsState == AdsState.Run) && !_IsRunning)
                {
                    _IsRunning = true;
                    _IsStopped = false;

                    ConnectionCounter++;

                    await SetRuntimeState(PLCRuntimeStateEnumeration.Running, cancellationToken);
                }

                // PLC runtime state change to 'Stopped'
                if ((AdsSession.AdsState != AdsState.Run) && !_IsStopped)
                {
                    await SetRuntimeState(PLCRuntimeStateEnumeration.Stopped, cancellationToken);

                    _IsRunning = false;
                    _IsStopped = true;
                }

                // Detect 'OnlineChange'
                if ((AdsSession.AdsState == AdsState.Run) && _OnlineChangeOccured)
                {
                    _Logger.LogInformation($"TwinCAT 3 ADS session [{AdsSession.AddressSpecifier}] online change occured.");
                    _OnlineChangeOccured = false;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
        }

        /// <summary>
        /// Reads the TwinCAT 3 variables previously stored in the internal dictionary field <see cref="_SamplingRateVariableBlocksDictionary"/>.
        /// </summary>
        /// <remarks>
        /// This method will automatically be executed by the <see cref="ProcessLoop"/> method.
        /// </remarks>
        /// <returns>
        /// True if the variables are successfully read, otherwise False.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the linking of model variables failed.
        /// </exception>
        private async Task<bool> ReadContinuousSampledVariables(CancellationToken cancellationToken)
        {
            await _ReadContinuousSamplingVariableSemaphore.WaitAsync();

            var exceptionText = $"Error on reading the continuous samlped TwinCAT 3 variables on model user [{Name}].";

            if (!AdsSession.IsActive)
            {
                _ReadContinuousSamplingVariableSemaphore.Release();
                return false;
            }

            try
            {
                // Increase period time counter to determine the sampling rates
                _ProcessPeriodTimeCounter += (int)ProcessingPeriod.TotalMilliseconds;
                if (_ProcessPeriodTimeCounter > (int)ModelVariableSamplingRateEnumeration.ms1000)
                    _ProcessPeriodTimeCounter = (int)ProcessingPeriod.TotalMilliseconds;

                // Get the variable blocks per sampling rate
                var samplingRateVariableBlockPairs = _SamplingRateVariableBlocksDictionary.ToArray();

                //task list to execute reading process for each sampling rate in parallel
                var samplingRateReadTasks = new List<Task>();

                for (int i = 0; i < samplingRateVariableBlockPairs.Length; i++)
                {
                    // Extract sampling rate and the corresponding variable blocks
                    var samplingRate = samplingRateVariableBlockPairs[i].Key;
                    var tc3VariableBlocks = samplingRateVariableBlockPairs[i].Value;

                    // Init empty action to avoid 'null' executions
                    //samplingRateReadActions[i] = () => { };

                    if ((int)samplingRate < 50)
                        continue;

                    // If no chunks are present there is nothing to do
                    if (tc3VariableBlocks.Count < 1)
                        continue;

                    // Check if the variables for this sampling rate needs to be processed
                    if (_ProcessPeriodTimeCounter % (int)samplingRate != 0)
                        continue;

                    Task t = Task.Run(async () =>
                    {
                        // Setup dictionary to store the result of the reading command
                        var resultDictionary = new Dictionary<Guid,List<IModelVariableDEO>>();

                        for (int blockIndex = 0; blockIndex < tc3VariableBlocks.Count; blockIndex++)
                        {
                            var blockVariableValueDictionary = await ReadVariableBlock(tc3VariableBlocks[blockIndex], cancellationToken);
                            if (blockVariableValueDictionary == null)
                            {
                                _Logger.LogError($"{exceptionText} Reading TwinCAT 3 variables failed on variable block index {blockIndex}.");
                            }
                            else
                            {
                                // Add the returned content to the concurrent dictionary
                                foreach (var blockvariableValuePair in blockVariableValueDictionary)
                                {
                                    if(!resultDictionary.ContainsKey(blockvariableValuePair.Key.ModelId))
                                    {
                                        resultDictionary.Add(blockvariableValuePair.Key.ModelId, new List<IModelVariableDEO>());
                                    }
                                    var deo = blockvariableValuePair.Key.ModelVariable.GetDEO();
                                    deo.Payload.Value = blockvariableValuePair.Value;
                                    resultDictionary[blockvariableValuePair.Key.ModelId].Add(deo);
                                }
                            }
                        }
                        var sendResponse = await SendPushModelVariablesCommand(resultDictionary, cancellationToken);
                    });

                    samplingRateReadTasks.Add(t);
                }

                Task.WaitAll(samplingRateReadTasks.ToArray());

                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
            finally
            {
                _ReadContinuousSamplingVariableSemaphore.Release();
            }
        }

        private async Task<IResponse<bool>> ReadSingleSampledVariables(CancellationToken cancellationToken)
        {
            await _ReadSingleSamplingVariableSemaphore.WaitAsync();

            // Setup response object
            IResponse<bool> response = new Response<bool>(true, ResponseResultEnumeration.Success);

            if (_SingleSamplingDataList.Count < 1)
            {
                _ReadSingleSamplingVariableSemaphore.Release();
                return response;
            }

            try
            {
                var singleSamplingDataRemoveList = new List<SingleSamplingData>();
               
                foreach (var singleSamplingData in _SingleSamplingDataList)
                {
                    //singleSamplingData.ProcessPeriodTimeCounter += (int)ProcessingPeriod.TotalMilliseconds;
                    //if (singleSamplingData.ProcessPeriodTimeCounter < (int)singleSamplingData.SamplingTrigger)
                    //    continue;

                    var pulledVariables = await PullVariableData(singleSamplingData.TC3Variables, cancellationToken);
                    await SendPushModelVariablesCommand(pulledVariables, cancellationToken);
                    singleSamplingDataRemoveList.Add(singleSamplingData);
                }
              
                _SingleSamplingDataList.RemoveAll(data => singleSamplingDataRemoveList.Contains(data));

                response.Result = ResponseResultEnumeration.Success;
                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Unexpected error on reading single sampling variables. {ex}");

                response.Result = ResponseResultEnumeration.Error;
                return response;
            }
            finally
            {
                _ReadSingleSamplingVariableSemaphore.Release();
            }
        }

        /// <summary>
        /// Links the prvious consumed model variables with a corresponding TwinCAT 3 symbol.
        /// </summary>
        /// <remarks>
        /// Unlinked model variables are stored in the list property <see cref="PLCModelUser.UnlinkedVariables"/>.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the linking of model variables failed.
        /// </exception>
        private async Task LinkVariables(ModelUpdateOptions modelUpdateOptions, CancellationToken cancellationToken)
        {
            await _ReadContinuousSamplingVariableSemaphore.WaitAsync();
            await _ReadSingleSamplingVariableSemaphore.WaitAsync();
            await _ReadVariableBlockSemaphore.WaitAsync();
            await _WriteVariableBlockSemaphore.WaitAsync();
            await _InvokeRpcSemaphore.WaitAsync();

            var exceptionText = $"Error on linking model variables at TwinCAT 3 PLC model user [{Name}].";

            try
            {
                // Get all symbols
                var getSymbolsResult = await AdsSession.SymbolServer.GetSymbolsAsync(cancellationToken);
                if (getSymbolsResult.Failed)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"Cannot resolving the TwinCAT 3 symbols on session {AdsSession.AddressSpecifier}. ADS error  {getSymbolsResult.ErrorCode}.", paramName: nameof(getSymbolsResult)));

                // Set TwinCAT symbols
                _SessionSymbols = getSymbolsResult.Symbols;

                // Setup (un)linking helpers
                if (!modelUpdateOptions.HasUpdateOptions)
                {
                    UnlinkedVariables.Clear();
                }
                else
                {
                    if (modelUpdateOptions.OptionModelKeys.Count > 0)
                        UnlinkedVariables.RemoveAll(variable => modelUpdateOptions.OptionModelKeys.Contains(variable.ModelLink.Key));

                    if (modelUpdateOptions.OptionModelComponentIds.Count > 0)
                        UnlinkedVariables.RemoveAll(variable => modelUpdateOptions.OptionModelComponentIds.Contains(variable.Id));

                    if (modelUpdateOptions.OptionServiceNames.Count > 0)
                        UnlinkedVariables.RemoveAll(variable => modelUpdateOptions.OptionServiceNames.Contains(variable.ServiceName));
                }
                var linkedVariableCount = 0;
                var nonLinkedVariableCount = 0;

                // Filter variables by the update options
                var twinCATVariables = _TC3VariableDictionay.Values
                  .Where(tc3Variable => modelUpdateOptions.OptionModelKeys.Count == 0 || modelUpdateOptions.OptionModelKeys.Contains(tc3Variable.ModelVariable.ModelLink.Key))
                  .Where(tc3Variable => modelUpdateOptions.OptionModelComponentIds.Count == 0 || modelUpdateOptions.OptionModelComponentIds.Contains(tc3Variable.ModelVariable.Id))
                  .Where(tc3Variable => modelUpdateOptions.OptionServiceNames.Count == 0 || modelUpdateOptions.OptionServiceNames.Contains(tc3Variable.ModelVariable.ServiceName))
                  .ToArray();
                //var twinCATVariables = _TwinCAT3Variables
                //    .Where(tc3Variable => modelUpdateOptions.OptionModelKeys.Count == 0 || modelUpdateOptions.OptionModelKeys.Contains(tc3Variable.ModelVariable.ModelLink.Key))
                //    .Where(tc3Variable => modelUpdateOptions.OptionModelComponentIds.Count == 0 || modelUpdateOptions.OptionModelComponentIds.Contains(tc3Variable.ModelVariable.Id))
                //    .Where(tc3Variable => modelUpdateOptions.OptionServiceNames.Count == 0 || modelUpdateOptions.OptionServiceNames.Contains(tc3Variable.ModelVariable.ServiceName))
                //    .ToArray();

                // Check each TwinCAT variable and link it with the ADS session symbol
                foreach (var tc3Variable in twinCATVariables)
                {
                    if (tc3Variable == null)
                    {
                        _Logger.LogDebug($"A TwinCAT 3 variable in model user {Name} is null.");
                        continue;
                    }

                    // Clear symbol
                    if (tc3Variable.Symbol != null)
                        tc3Variable.Symbol = null;

                    // Get the corresponding TwinCAT3 Link
                    var twinCAT3Link = tc3Variable.ModelVariable.TwinCAT3Links.FirstOrDefault(link => link.Name == Name);
                    if (twinCAT3Link == null)
                    {
                        _Logger.LogError($"Cannot resolving the TwinCAT 3 link on variable [{tc3Variable.ModelVariable.ModelLink.Key}] at model user [{Name}].");
                        continue;
                    }

                    // Clear link props
                    twinCAT3Link.IsConnected = false;

                    // Get the TwinCAT3 symbol
                    var result = _SessionSymbols.TryGetInstance(twinCAT3Link.SymbolKey.Key, out ISymbol symbol);
                    if (!result || symbol == null)
                    {
                        _Logger.LogWarning($"TwinCAT 3 symbol for key [{twinCAT3Link.SymbolKey.Key}] not found.");
                        nonLinkedVariableCount++;
                        UnlinkedVariables.Add(tc3Variable.ModelVariable);
                        continue;
                    }

                    // Update props
                    linkedVariableCount++;
                    tc3Variable.Symbol = symbol;
                    twinCAT3Link.IsConnected = true;
                }

                _Logger.LogInformation($"TwinCAT symbol linking completed. {linkedVariableCount} variables are linked. {nonLinkedVariableCount} variables have been failed to linking.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
            finally
            {
                _ReadContinuousSamplingVariableSemaphore.Release();
                _ReadSingleSamplingVariableSemaphore.Release();
                _ReadVariableBlockSemaphore.Release();
                _WriteVariableBlockSemaphore.Release();
                _InvokeRpcSemaphore.Release();
            }
        }

        /// <summary>
        /// Creates variable blocks based on the sampling rate property of the prvious consumed model variables.
        /// </summary>
        /// <remarks>
        /// The created TwinCAT 3 variable blocks are stored in the internal dictionary field <see cref="_SamplingRateVariableBlocksDictionary"/>.
        /// </remarks>
        private async Task CreateSamplingRateVariableBlocks()
        {
            await _ReadContinuousSamplingVariableSemaphore.WaitAsync();
            await _ReadSingleSamplingVariableSemaphore.WaitAsync();
            await _ReadVariableBlockSemaphore.WaitAsync();
            await _WriteVariableBlockSemaphore.WaitAsync();
            await _InvokeRpcSemaphore.WaitAsync();

            var exceptionText = $"Error on creating sampling variable blocks at TwinCAT 3 PLC model user [{Name}].";

            try
            {
                var multipleAddedVariabels = new List<TC3Variable>();
                _SamplingRateVariableBlocksDictionary.Clear();
                foreach (var tc3Variable in _TC3VariableDictionay.Values)
                {
                    // Get the corresponding TwinCAT3 Link
                    var twinCAT3Link = tc3Variable.ModelVariable.TwinCAT3Links.FirstOrDefault(link => link.Name == Name);
                    if (twinCAT3Link == null)
                    {
                        _Logger.LogError($"Cannot resolving the TwinCAT link for model user [{Name}] on variable [{tc3Variable.ModelVariable.ModelLink.Key}].");
                        continue;
                    }

                    if (!twinCAT3Link.IsConnected)
                        continue;

                    // Get defined sampling rate
                    var samplingRate = twinCAT3Link.SamplingRate;

                    // Create variable blocks per sampling rate
                    if (!_SamplingRateVariableBlocksDictionary.ContainsKey(samplingRate))
                    {
                        // Create new key with content
                        var tc3VariableBlocks = new List<TC3VariableBlock>();
                        var tc3VariableBlock = new TC3VariableBlock();
                        tc3VariableBlock.Variables.Add(tc3Variable);
                        tc3VariableBlock.SymbolCollection.Add(tc3Variable.Symbol);
                        tc3VariableBlock.Count++;

                        tc3VariableBlocks.Add(tc3VariableBlock);
                        _SamplingRateVariableBlocksDictionary.Add(samplingRate, tc3VariableBlocks);

                        _Logger.LogInformation($"TwinCAT 3 variable block with sample rate [{samplingRate}] created.");
                    }
                    else
                    {
                        // Get the last block
                        var tc3VariableBlocks = _SamplingRateVariableBlocksDictionary[samplingRate];
                        var tc3VariableBlock = tc3VariableBlocks.Last();

                        // Make sure a block does not exeed the maximum block size
                        if (tc3VariableBlock.Count >= VARIABLE_BLOCK_SIZE)
                        {
                            tc3VariableBlocks.Add(new TC3VariableBlock());
                            tc3VariableBlock = tc3VariableBlocks.Last();
                        }

                        // Add content
                        try
                        {
                            tc3VariableBlock.Variables.Add(tc3Variable);
                            tc3VariableBlock.SymbolCollection.Add(tc3Variable.Symbol);
                            tc3VariableBlock.Count++;
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                    }
                }

                _Logger.LogInformation($"TwinCAT variable block building completed.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
            finally
            {
                _ReadContinuousSamplingVariableSemaphore.Release();
                _ReadSingleSamplingVariableSemaphore.Release();
                _ReadVariableBlockSemaphore.Release();
                _WriteVariableBlockSemaphore.Release();
                _InvokeRpcSemaphore.Release();
            }
        }

        /// <summary>
        /// Creates variable blocks by the passed TwinCAT 3 variables. Optional new values can be passed. The new values ​​for the TwinCAT 3 variables are assigned indexed, so the number of elements must match.
        /// </summary>
        /// <remarks>
        /// Block size ist defined by the VARIABLE_BLOCK_SIZE constant.
        /// </remarks>
        /// <param name="tc3Variables">TwinCAT 3 variables which are to be combined into a block.</param>
        /// <param name="variableValues">Optional new values for the passed <paramref name="tc3Variables"/>.</param>
        /// <returns>
        /// A list of <see cref="TC3VariableBlock"/> types.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a passed argument is null
        /// </exception>
        /// <exception cref="CountMismatchException">
        /// Thrown when the count of the two passed list does not match
        /// </exception>
        private Task<List<TC3VariableBlock>> CreateVariableBlocks(List<TC3Variable> tc3Variables, List<object> variableValues = null)
        {
            // Setup response object
            List<TC3VariableBlock> variableBlocks = new();

            if (tc3Variables == null)
                throw new ArgumentNullException(paramName: nameof(tc3Variables), $"Passed TwinCAT 3 variables are null.");

            // Helpers
            var hasValues = variableValues != null;

            // Check the item count of each list
            if (variableValues != null)
            {
                if (tc3Variables.Count != variableValues.Count)
                {
                    throw new CountMismatchException(
                        sourceContext: nameof(tc3Variables),
                        sourceCount: tc3Variables.Count,
                        targetContext: nameof(variableValues),
                        targetCount: variableValues.Count);
                }
            }

            // Create variable blocks
            for (int index = 0; index < tc3Variables.Count; index++)
            {
                var tc3Variable = tc3Variables[index];
                object value = null;
                if (hasValues)
                    value = variableValues[index];

                // Get the corresponding TwinCAT3 Link
                var twinCAT3Link = tc3Variable.ModelVariable.TwinCAT3Links.FirstOrDefault(link => link.Name == Name);
                if (twinCAT3Link == null)
                {
                    _Logger.LogError($"Cannot resolving the TwinCAT link for model user [{Name}] on variable [{tc3Variable.ModelVariable.ModelLink.Key}].");
                    continue;
                }

                if (!twinCAT3Link.IsConnected)
                    continue;

                // Make sure one block is existing
                var tc3VariableBlock = variableBlocks.LastOrDefault();
                if (tc3VariableBlock == null)
                {
                    tc3VariableBlock = new TC3VariableBlock();
                    variableBlocks.Add(tc3VariableBlock);
                }

                // Make sure a block does not exeed the maximum block size
                if (tc3VariableBlock.Count >= VARIABLE_BLOCK_SIZE)
                {
                    variableBlocks.Add(new TC3VariableBlock());
                    tc3VariableBlock = variableBlocks.Last();
                }

                // Add content
                tc3VariableBlock.Variables.Add(tc3Variable);
                if (hasValues)
                    tc3VariableBlock.VariableValues.Add(value);
                tc3VariableBlock.SymbolCollection.Add(tc3Variable.Symbol);
                tc3VariableBlock.Count++;
            }

            return Task.FromResult(variableBlocks);
        }

        /// <summary>
        /// Reads a TwinCAT 3 variable block with ADS from the PLC.
        /// </summary>
        /// <param name="tc3VariableBlock">TwinCAT 3 variable block to write.</param>
        /// <returns>
        /// A dictionary containing the TwinCAT 3 variable as <see cref="TC3Variable"/> and the value as <see cref="object"/> that was read
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a passed argument is null
        /// </exception>
        /// <exception cref="CountMismatchException">
        /// Thrown when a passed argument is null
        /// </exception>
        /// <exception cref="TC3ReadSymbolException">
        /// Thrown when reading from the corresponding TwinCAT 3 symbols failed
        /// </exception>
        private async Task<Dictionary<TC3Variable, object>> ReadVariableBlock(TC3VariableBlock tc3VariableBlock, CancellationToken cancellationToken)
        {
            await _ReadVariableBlockSemaphore.WaitAsync();

            try
            {
                if (tc3VariableBlock == null)
                    throw new ArgumentNullException(paramName: nameof(tc3VariableBlock), $"Passed TwinCAT 3 variable block is null.");

                // Setup result object
                Dictionary<TC3Variable, object> resultDictionary = new();

                // Setup ADS read command
                SumSymbolRead readCommand = new SumSymbolRead(AdsSession.Connection, tc3VariableBlock.SymbolCollection);

                // Execute ADS read command and evaluate the return value
                ResultSumValues resultSumValues = await readCommand.ReadAsync(cancellationToken);
                if (resultSumValues.Failed)
                    throw new TC3ReadSymbolException(Name, resultSumValues);

                // Check the length of the readed values
                if (resultSumValues.Values.Length != tc3VariableBlock.Count)
                    throw new CountMismatchException(
                        sourceContext: $"Passed TwinCAT 3 variable block",
                        sourceCount: tc3VariableBlock.Count,
                        targetContext: $"Variable data read out via ADS",
                        targetCount: resultSumValues.Values.Length);

                // If the values ​​read are the same as the existing ones, the variable is not updated
                for (int i = 0; i < tc3VariableBlock.Count; i++)
                {
                    if (tc3VariableBlock.Variables[i].ModelVariable.IsValueEqual(resultSumValues.Values[i]))
                        continue;

                    // Add the TwinCAT 3 variable and the new value to the response
                    resultDictionary.Add(tc3VariableBlock.Variables[i], resultSumValues.Values[i]);
                }

                return resultDictionary;
            }
            catch (Exception ex)
            {
                throw new TC3ReadSymbolException(Name, tc3VariableBlock, ex);
            }
            finally
            {
                _ReadVariableBlockSemaphore.Release();
            }
        }

        /// <summary>
        /// Writes a TwinCAT 3 variable block with ADS to the PLC.
        /// </summary>
        /// <param name="tc3VariableBlock">TwinCAT 3 variable block which is to be written.</param>
        /// <returns>
        /// A dictionary containing the TwinCAT 3 variable as <see cref="TC3Variable"/> and the value as <see cref="object"/> that was written
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a passed argument is null
        /// </exception>
        /// <exception cref="TC3WriteSymbolException">
        /// Thrown when writing to the corresponding TwinCAT 3 symbols failed
        /// </exception>
        private async Task<Dictionary<TC3Variable, object>> WriteVariableBlock(TC3VariableBlock tc3VariableBlock, CancellationToken cancellationToken)
        {
            await _WriteVariableBlockSemaphore.WaitAsync();

            // Create helpers
            List<object> writeValues = new();
            SymbolCollection writeSymbols = new();

            // Dev helpers
            TC3Variable actualVariable;
            object actualValue = null;
            Type actualValueType = null;
            int actualIndex = 0;

            try
            {
                if (tc3VariableBlock == null)
                    throw new ArgumentNullException(paramName: nameof(tc3VariableBlock), $"Passed TwinCAT 3 variable block is null.");

                // Setup result object
                Dictionary<TC3Variable, object> resultDictionary = new();

                if (tc3VariableBlock.VariableValues.Count <= 0)
                {
                    _Logger.LogWarning($"Passed TwinCAT 3 variable block has no variable values to write.");
                    return resultDictionary;
                }

                for (int i = 0; i < tc3VariableBlock.Variables.Count; i++)
                {
                    var tc3Variable = tc3VariableBlock.Variables[i];
                    actualVariable = tc3Variable;
                    actualIndex = i;

                    // Get the new value
                    var writeRequestValue = tc3VariableBlock.VariableValues[i];
                    if (writeRequestValue == null)
                    {
                        _Logger.LogError($"Value in a variable block is null for the passed model variable [{tc3Variable.ModelVariable.ModelLink.Key}].");
                        continue;
                    }

                    var writeRequestValueType = writeRequestValue.GetType();
                    actualValue = writeRequestValue;
                    actualValueType = writeRequestValueType;

                    // Make sure that the value type matches the correct type for ADS
                    if (tc3Variable.ModelVariable.ValueType.IsGenericType)
                    {
                        // Generic enumerables must be an Array
                        if ((tc3Variable.ModelVariable.ValueType.IsInterface && (tc3Variable.ModelVariable.ValueType.GetGenericTypeDefinition() == typeof(IEnumerable<>))) ||
                            (!tc3Variable.ModelVariable.ValueType.IsInterface && !tc3Variable.ModelVariable.ValueType.GetInterfaces().Any(iface =>
                            iface.IsGenericType &&
                            iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))))
                        {
                            // Make sure the IEnumerable<> matches the correct ADS type as Array
                            var marshaledValue = MarshalingGenericType(writeRequestValue);
                            if (marshaledValue != null)
                            {
                                writeValues.Add(marshaledValue);
                            }
                            else
                            {
                                _Logger.LogWarning($"Variable {tc3Variable.ModelVariable.ModelLink.Name} uses an unsupported generic data type ({tc3Variable.ModelVariable.ValueType}).");
                                continue;
                            }
                        }
                        else
                        {
                            _Logger.LogWarning($"Variable {tc3Variable.ModelVariable.ModelLink.Name} uses an unsupported generic data type ({tc3Variable.ModelVariable.ValueType}).");
                            continue;
                        }
                    }
                    else if (tc3Variable.ModelVariable.ValueType == typeof(string) &&
                        string.IsNullOrEmpty((string)writeRequestValue))
                    {
                        // Make sure string value is not 'Null'
                        writeValues.Add(string.Empty);
                    }
                    else
                    {
                        // Default behavior. No conversion or cast needed.
                        writeValues.Add(writeRequestValue);
                    }

                    // Setup response dictionary
                    resultDictionary.Add(tc3Variable, writeRequestValue);

                    // Adding TwinCAT symbol for write command
                    writeSymbols.Add(tc3Variable.Symbol);
                }

                // Setup ADS write command
                SumSymbolWrite writeCommand = new(AdsSession.Connection, writeSymbols);

                // Execute ADS write command and evaluate the return value
                ResultSumCommand resultSumCommand = await writeCommand.WriteAsync(writeValues.ToArray(), cancellationToken);
                if (resultSumCommand.Failed)
                    throw new TC3WriteSymbolException(Name, resultSumCommand);

                return resultDictionary;
            }
            catch (Exception ex)
            {
                throw new TC3WriteSymbolException(Name, tc3VariableBlock, ex);
            }
            finally
            {
                _WriteVariableBlockSemaphore.Release();
            }
        }

        /// <summary>
        /// Pulls data from the PLC by the passed Informations of <paramref name="tc3Variables"/>.
        /// </summary>
        /// <remarks>
        /// After a successful pull of the data from the PLC the changed model variable values will be dispatched as <see cref="ModelVariablesValueChangeCommand"/> command. So the changed values can be catched via the regular flow in a corresponding event handler.
        /// </remarks>
        /// <param name="tc3Variables">List of TwinCAT 3 variables to be pulled.</param>
        /// <param name="cancellationToken">CancellationToken for the task.</param>
        /// <returns>
        /// True if the data is pulled and dispatched, otherwise False.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a passed argument is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the data could not be pulled from the PLC.
        /// </exception>
        private async Task<Dictionary<Guid,List<IModelVariableDEO>>> PullVariableData(List<TC3Variable> tc3Variables, CancellationToken cancellationToken)
        {
            var exceptionText = $"Error on pulling data from the PLC on model user [{Name}].";

            // Setup dictionary for processed TwinCAT variables and their new value
            var tc3VariableValueDictionary = new Dictionary<TC3Variable, object>();
            var modelVariableDictionary = new Dictionary<Guid,List<IModelVariableDEO>>();


            try
            {
                if (tc3Variables == null)
                    throw new ArgumentNullException(paramName: nameof(tc3Variables), $"{exceptionText} Passed TwinCAT 3 variable list is null.");

                // Really nothing to do here
                if (tc3Variables.Count < 1)
                    return modelVariableDictionary;

                if (AdsSession.ConnectionState != TwinCAT.ConnectionState.Connected)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"TwinCAT 3 ADS is not connected.", paramName: nameof(AdsSession.ConnectionState)));

                if (AdsSession.AdsState != AdsState.Run)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"TwinCAT 3 ADS is not in state [{AdsState.Run}].", paramName: nameof(AdsSession.AdsState)));

                // Create variable blocks. A block usually consists of 500 variables, which increases the efficiency of data transfer
                var variableBlocks = await CreateVariableBlocks(tc3Variables);
                if (variableBlocks.Count <= 0)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"No variable blocks present by the passed TwinCAT 3 variables.", paramName: nameof(variableBlocks)));



                // Execute ADS read command for each variable block
                for (int blockIndex = 0; blockIndex < variableBlocks.Count; blockIndex++)
                {
                    // OFI -> Using a nested try-catch to handle exceptions per variable block

                    var blockVariableValueDictionary = await ReadVariableBlock(variableBlocks[blockIndex], cancellationToken);
                    if (blockVariableValueDictionary == null)
                    {
                        _Logger.LogError($"{exceptionText} Reading TwinCAT 3 variables failed on variable block index {blockIndex}.");
                    }
                    else
                    {
                        // Concat dictionary with each processed variable block result
                        foreach(var kvpair in blockVariableValueDictionary )
                        {
                            tc3VariableValueDictionary.Add(kvpair.Key, kvpair.Value);
                        }
                       
                        //tc3VariableValueDictionary = tc3VariableValueDictionary
                        //    .Concat(blockVariableValueDictionary)
                        //    .ToDictionary(k => k.Key, v => v.Value);
                    }
                }
                
                 tc3VariableValueDictionary.ToList().ForEach((kVPair)=>
                {
                    if(!modelVariableDictionary.ContainsKey(kVPair.Key.ModelId))
                    {
                        modelVariableDictionary.Add(kVPair.Key.ModelId, new List<IModelVariableDEO>());
                    }
                    
                    var deo = kVPair.Key.ModelVariable.GetDEO(false);
                    deo.Payload.Value = kVPair.Value;
                    modelVariableDictionary[kVPair.Key.ModelId].Add(deo);
                });
                //var response = await SendPushModelVariablesCommand(tc3VariableValueDictionary, cancellationToken);
                return modelVariableDictionary;

                //return response.Result == ResponseResultEnumeration.Success;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
        }

        /// <summary>
        /// Pushes variable data to the PLC. The new values ​​for the variables are assigned indexed, so the number of elements must match.
        /// </summary>
        /// <param name="tc3Variables">Contains the affected TwinCAT 3 variables.</param>
        /// <param name="variableValues">Contains the new values for the passed <paramref name="variableValues"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a passed argument is null.
        /// </exception>
        /// <exception cref="CountMismatchException">
        /// Thrown when the count of the two passed list does not match.
        /// </exception>       
        /// <exception cref="PushModelVariableDataException">
        /// Thrown when the new variable data could not be pushed.
        /// </exception>
        private async Task<bool> PushVariableData(List<TC3Variable> tc3Variables, List<object> variableValues, CancellationToken cancellationToken)
        {
            var exceptionText = $"Error on pushing data to the PLC on model user [{Name}].";

            try
            {
                if (tc3Variables == null)
                    throw new ArgumentNullException(paramName: nameof(tc3Variables), $"{exceptionText} Passed TwinCAT 3 variable list is null.");

                if (variableValues == null)
                    throw new ArgumentNullException(paramName: nameof(variableValues), $"{exceptionText} Passed list for new variable values is null.");

                // Really nothing to do here
                if (tc3Variables.Count < 1 && variableValues.Count < 1)
                    return true;

                if (tc3Variables.Count != variableValues.Count)
                {
                    throw new CountMismatchException(
                        sourceContext: nameof(tc3Variables),
                        sourceCount: tc3Variables.Count,
                        targetContext: nameof(variableValues),
                        targetCount: variableValues.Count);
                }

                if (AdsSession.ConnectionState != TwinCAT.ConnectionState.Connected)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"TwinCAT 3 ADS is not connected.", paramName: nameof(AdsSession.ConnectionState)));

                if (AdsSession.AdsState != AdsState.Run)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"TwinCAT 3 ADS is not in state [{AdsState.Run}].", paramName: nameof(AdsSession.AdsState)));

                // Create variable blocks. A block usually consists of 500 variables, which increases the efficiency of data transfer
                var variableBlocks = await CreateVariableBlocks(tc3Variables, variableValues);
                if (variableBlocks.Count <= 0)
                {
                    _Logger.LogError($"Pushing variable data failed. No TwinCAT 3 variable blocks present.");
                    return false;
                }

                // Setup dictionary for processed TwinCAT variables and their new value
                var tc3VariableValueDictionary = new Dictionary<TC3Variable, object>();

                // Execute ADS write command for each variable block
                for (int blockIndex = 0; blockIndex < variableBlocks.Count; blockIndex++)
                {
                    try
                    {
                        var resultDictionary = await WriteVariableBlock(variableBlocks[blockIndex], cancellationToken);
                        if (resultDictionary.Count <= 0)
                        {
                            _Logger.LogError($"Pushing variable data failed. Writing variable block on index [{blockIndex}] has returend empty.");
                            return false;
                        }
                        else
                        {
                            // Concat dictionary with each processed result from writing the variable block
                            tc3VariableValueDictionary = tc3VariableValueDictionary.Concat(resultDictionary).ToDictionary(k => k.Key, v => v.Value);
                        }
                    }
                    catch(Exception e)
                    {
                        _Logger.LogError(e.Message);
                    }
                   
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new PushModelVariableDataException(Name, tc3Variables.Select( t => t.ModelVariable) , ex);
            }
        }

        private object MarshalingGenericType(object value)
        {
            var valueType = value.GetType();
            Type valueArgumentType = null;

            if (valueType.HasElementType)
            {
                valueArgumentType = valueType.GetElementType();
            }

            if (valueType.IsGenericType)
            {
                var valueTypeGenericArguments = valueType.GetGenericArguments();
                if (valueTypeGenericArguments.Length <= 0)
                {
                    return value;
                }

                valueArgumentType = valueTypeGenericArguments[0];
            }

            if (valueArgumentType == null)
                return value;

            if (valueArgumentType == typeof(bool))
            {
                var enumerableValue = value as IEnumerable<bool>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(byte))
            {
                var enumerableValue = value as IEnumerable<byte>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(sbyte))
            {
                var enumerableValue = value as IEnumerable<sbyte>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(char))
            {
                var enumerableValue = value as IEnumerable<char>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(decimal))
            {
                var enumerableValue = value as IEnumerable<decimal>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(double))
            {
                var enumerableValue = value as IEnumerable<double>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(float))
            {
                var enumerableValue = value as IEnumerable<float>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(int))
            {
                var enumerableValue = value as IEnumerable<int>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(uint))
            {
                var enumerableValue = value as IEnumerable<uint>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(long))
            {
                var enumerableValue = value as IEnumerable<long>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(ulong))
            {
                var enumerableValue = value as IEnumerable<ulong>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(short))
            {
                var enumerableValue = value as IEnumerable<short>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(ushort))
            {
                var enumerableValue = value as IEnumerable<ushort>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(string))
            {
                var enumerableValue = value as IEnumerable<string>;
                return enumerableValue.ToArray();
            }
            else if (valueArgumentType == typeof(object))
            {
                var enumerableValue = value as IEnumerable<object>;
                return enumerableValue.ToArray();
            }
            else
            {
                return null;
            }
        }

        #endregion
        #region events

        private void OnlineChangeCnt_ValueChanged(object sender, ModelVariableChangedEventArgs e)
        {
            _OnlineChangeOccured = true;
        }

        #endregion

        #region ModelUser

        protected override async Task Initialize(CancellationToken cancellationToken)
        {
            var exceptionText = $"Error on initializing TwinCAT 3 model user [{Name}].";

            try
            {
                Platform = ModelUserPlatformEnumeration.TwinCAT3;
                _Logger.LogInformation($"Model user [{Name}] on platform [{Platform}] is initializing...");

                // Invoke component data update
                var componentName = $"PLC.{Platform}.{Name}";
                var componentVersion = AdsSession.TwinCATVersion.ToString();
                await UpdateComponentData(componentName, componentVersion, ComponentStateEnumeration.Ok, new List<IResponseMessage>(), cancellationToken);

                // Consume model variables
                //var consumeResponse = await ConsumeModelData(new ModelUpdateOptions(), cancellationToken);

                // Connect ADS session
                var isConnected = await AdsSession.Connect(cancellationToken);
                if (!isConnected)
                    _Logger.LogError($"TwinCAT 3 connection to [{AdsSession.AddressSpecifier}] could not be established.");

                // Start processing loop
                await _ProcessingLoopService.StartAsync(cancellationToken);

                _Logger.LogInformation($"TwinCAT 3 model user is initialized.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
        }

        protected override async Task Terminate(CancellationToken cancellationToken)
        {
            var exceptionText = $"Error on terminating TwinCAT 3 model user [{Name}].";

            try
            {
                _Logger.LogWarning($"TwinCAT 3 model user is terminating...");

                // Stop processing loop
                await _ProcessingLoopService.StopAsync(cancellationToken);

                // Disconnect ADS session
                var isDisconnected = await AdsSession.Disconnect(cancellationToken);
                if (isDisconnected)
                {
                    await SetConnectionState(PLCConnectionStateEnumeration.Disconnected, cancellationToken);
                }
                else
                {
                    _Logger.LogError($"TwinCAT 3 connection to [{AdsSession.AddressSpecifier}] could not be disconnected.");

                    await SetConnectionState(PLCConnectionStateEnumeration.Error, cancellationToken);
                }

                // Invoke component data update
                await UpdateComponentData(ComponentStateEnumeration.Error, cancellationToken);

                _Logger.LogWarning($"TwinCAT 3 model user is terminated.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
        }

        #endregion

        #region PLCModelUser

        protected override async Task ProcessLoop(CancellationToken cancellationToken, long ticksByStart)
        {
            try
            {
                // Check states
                await CheckStates(cancellationToken);

                // Read continuous sampled variables
                await ReadContinuousSampledVariables(cancellationToken);

                // Read single sampled variables
                // await ReadSingleSampledVariables(cancellationToken);

                _TicksOnLastProcessingLoop = ticksByStart;
            }
            catch (Exception ex)
            {
                _Logger.LogInformation($"Unexpected error in process loop of model user [{Name}]. {ex}");
            }
        }

        #endregion

        #region IModelUser

        public override async Task<bool> ConsumeModelData(ModelUpdateOptions modelUpdateOptions, CancellationToken cancellationToken)
        {
            var exceptionText = $"Error on consuming model update data on model user [{Name}].";

            // Get used models by this model user
            var usedModels = _ModelFactory.GetUsedModels(Id).ToList();
            if (usedModels.Count <= 0)
                throw new InvalidOperationException(exceptionText,
                    new ArgumentException($"No model available for this model user.", paramName: nameof(usedModels.Count)));

            // Release model data first
            var releaseDataResult = await ReleaseModelData(modelUpdateOptions, cancellationToken);

            // Check for register system variables
            if (!modelUpdateOptions.HasUpdateOptions)
                RegisterSystemVariables();

            var newNoneSampledTc3Variables = new List<TC3Variable>();
            var newNoneSampledModelVariableValues = new List<object>();

            for (int modelIndex = 0; modelIndex < usedModels.Count; modelIndex++)
            {
                // Check the model
                var model = usedModels[modelIndex];
                if (model == null)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentNullException(paramName: nameof(model), $"No model at index [{modelIndex}] available for this model user."));

                // Check each variable of the model and create a TwinCAT 3 variable
                var modelVariables = model.Variables.ToArray();
                if (modelUpdateOptions.HasUpdateOptions)
                    modelVariables = model.Variables
                        .Where(variable => modelUpdateOptions.OptionModelKeys.Count == 0 || modelUpdateOptions.OptionModelKeys.Contains(variable.ModelLink.Key))
                        .Where(variable => modelUpdateOptions.OptionModelComponentIds.Count == 0 || modelUpdateOptions.OptionModelComponentIds.Contains(variable.Id))
                        .Where(variable => modelUpdateOptions.OptionServiceNames.Count == 0 || modelUpdateOptions.OptionServiceNames.Contains(variable.ServiceName))
                        .ToArray();

                for (int variableIndex = 0; variableIndex < modelVariables.Length; variableIndex++)
                {
                    var variable = modelVariables[variableIndex];
                    if (variable == null)
                    {
                        _Logger.LogDebug($"A variable in model [{model.Name}, {model.Id}] of model user [{Name}] is null. Consuming data for this variable will be skipped.");
                        continue;
                    }

                    if (!variable.TwinCAT3Links.Any(link => link.Name == Name))
                        continue;

                    if (variable.TwinCAT3Links.Any(link => link.Ignore))
                        continue;

                    if (variable.TwinCAT3Links.Any(link => link.SamplingRate == ModelVariableSamplingRateEnumeration.Undefined))
                        continue;

                    // Add the new TwinCAT 3 variable
                    var tc3Variable = AddVariable(model.Id, variable);

                    // We store the new none sampled TwinCAT 3 variables and their new values
                    if (variable.TwinCAT3Links.Any(link => link.SamplingRate == ModelVariableSamplingRateEnumeration.None))
                    {
                        newNoneSampledTc3Variables.Add(tc3Variable);
                        newNoneSampledModelVariableValues.Add(variable.GetValue());
                    }
                }
            }

            // Consuming of model update data is here complete but no further operations are possible if the ADS session is not in the correct states
            if (AdsSession.ConnectionState != TwinCAT.ConnectionState.Connected ||
                AdsSession.AdsState != AdsState.Run)
                return true;

            // Link the variables with a corresponding TwinCAT 3 symbol
            await LinkVariables(modelUpdateOptions, cancellationToken);

            // Create variable blocks per sampling rate
            await CreateSamplingRateVariableBlocks();

            // We push the new consumed model variable values to the PLC
            // We only push the none sampled data, because the others will be sampled by the regular process loop anyway
            var pushVariableDataResult = await PushVariableData(newNoneSampledTc3Variables, newNoneSampledModelVariableValues, cancellationToken);
            if (!pushVariableDataResult)
            {
                _Logger.LogError($"Consuming new model data not completed. Pushing none sampled values to the TwinCAT 3 PLC failed.");
                return false;
            }

            return true;
        }

        public override async Task<bool> ReleaseModelData(ModelUpdateOptions modelUpdateOptions, CancellationToken cancellationToken)
        {
            // Clear internal TwinCAT 3 variable strore
            if (!modelUpdateOptions.HasUpdateOptions)
            {
                _TC3VariableDictionay.Clear();
            }
            else
            {

                if (modelUpdateOptions.OptionModelKeys.Count > 0)
                {
                    var itemsToRemove = new List<Guid>();
                   foreach(var item in _TC3VariableDictionay)
                    {
                        if (modelUpdateOptions.OptionModelKeys.Contains(item.Value.ModelVariable.ModelLink.Key))
                        {
                            itemsToRemove.Add(item.Key);
                        }
                    }
                    foreach (var itemToRemove in itemsToRemove)
                    {
                        _TC3VariableDictionay.Remove(itemToRemove);
                    }
                }

                if (modelUpdateOptions.OptionModelComponentIds.Count > 0)
                {
                    var itemsToRemove = new List<Guid>();
                    foreach (var item in _TC3VariableDictionay)
                    {
                        if (modelUpdateOptions.OptionModelComponentIds.Contains(item.Value.ModelVariable.Id))
                        {
                            itemsToRemove.Add(item.Key);
                        }
                    }
                    foreach (var itemToRemove in itemsToRemove)
                    {
                        _TC3VariableDictionay.Remove(itemToRemove);
                    }
                }

                if (modelUpdateOptions.OptionServiceNames.Count > 0)
                {
                    var itemsToRemove = new List<Guid>();
                    foreach (var item in _TC3VariableDictionay)
                    {
                        if (modelUpdateOptions.OptionServiceNames.Contains(item.Value.ModelVariable.ServiceName))
                        {
                            itemsToRemove.Add(item.Key);
                        }
                    }
                    foreach (var itemToRemove in itemsToRemove)
                    {
                        _TC3VariableDictionay.Remove(itemToRemove);
                    }
                }

                // Create variable blocks per sampling rate
                await CreateSamplingRateVariableBlocks();
            }

            return true;
        }

        public override async Task<bool> PushVariableData(IEnumerable<IModelVariableDEO> modelVariableDEOs, CancellationToken cancellationToken)
        {
            try
            {
                // Pre-sorting the affected data
                var modelVariableDEOIDs = modelVariableDEOs
                    .Select(deo => deo.ModelContext.Id)
                    .ToList();

                var affectedTwinCATVariables = new List<TC3Variable>();

                foreach (var id in modelVariableDEOIDs)
                {
                    if(_TC3VariableDictionay.ContainsKey(id))
                    {
                        affectedTwinCATVariables.Add(_TC3VariableDictionay[id]);
                    }
                }
                
                // Get affected TwinCAT3 variables
                //var affectedTwinCATVariables = _TwinCAT3Variables
                //    .Where(tc3Variable => modelVariableDEOIDs.Contains(tc3Variable.ModelVariable.Id))
                //    .OrderBy(tc3Variable => tc3Variable.ModelVariable.Id)
                //    .ToList();

                var modelVariableDEOValues = modelVariableDEOs
                    .Where(deo => affectedTwinCATVariables.Any(tc3Variable => tc3Variable.ModelVariable.Id == deo.ModelContext.Id))
                    //.OrderBy(deo => deo.ModelContext.Id)
                    .Select(deo => deo.Payload.Value)
                    .ToList();

                return await PushVariableData(affectedTwinCATVariables, modelVariableDEOValues, cancellationToken);
            }
            catch (PushModelVariableDataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PushModelVariableDataException(Name, modelVariableDEOs, ex);
            }
        }

        public override async Task<Dictionary<Guid,List<IModelVariableDEO>>> PullVariableData(CancellationToken cancellationToken)
        {
            try
            {
                return await PullVariableData(_TC3VariableDictionay.Values.ToList(), cancellationToken);
            }
            catch (InvalidOperationException)
            {
                throw;
            }      
        }

        public override async Task<Dictionary<Guid, List<IModelVariableDEO>>> PullVariableData(List<Guid> variabledIDs, CancellationToken cancellationToken)
        {
            try
            {
                var affectedTwinCATVariables = new List<TC3Variable>();

                // Get affected TwinCAT3 variables
                foreach (var id in variabledIDs)
                {
                    if (_TC3VariableDictionay.ContainsKey(id))
                    {
                        affectedTwinCATVariables.Add(_TC3VariableDictionay[id]);
                    }
                }
                
                //var affectedTwinCATVariables = _TwinCAT3Variables
                //    .Where(tc3Variable => variabledDEOs.Any((Deo) => Deo.ModelContext.Id.Equals(tc3Variable.ModelVariable.Id)))
                //    .ToList();

                var pulledVariables =  await PullVariableData(affectedTwinCATVariables, cancellationToken);
                SendPushModelVariablesCommand(pulledVariables, cancellationToken);
                return pulledVariables;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public override async Task<ModelRPCResponse> InvokeRpcMethod(Guid rpcId, IList<object> inputArguments, CancellationToken cancellationToken)
        {
            await _InvokeRpcSemaphore.WaitAsync();

            var exceptionText = $"Error on invoking rpc model component on model user [{Name}].";
            var modelRPCResponse = new ModelRPCResponse();
            modelRPCResponse.Result = ModelRPCResponseResultEnumeration.Failed;
            modelRPCResponse.ReturnValue = null;

            try
            {
                // Check ADS session
                if (!AdsSession.IsActive)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"TwinCAT 3 ADS session is not active.", paramName: nameof(AdsSession.IsActive)));

                // Get corresponding rpc model component
                var rpcMethod = FindRPCMethod(rpcId);
                if (rpcMethod == null)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"No corresponding rpc model component for id [{rpcId}] found.", paramName: nameof(FindRPCMethod)));

                // Updating exception text
                exceptionText = $"Error on invoking rpc model component [{rpcMethod.ModelLink.Key}] on model user [{Name}].";

                // Get corresponding TwinCAT3 link
                var rpcTwinCAT3Link = rpcMethod.TwinCAT3Links.FirstOrDefault(link => link.Name == Name);
                if (rpcTwinCAT3Link == null)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"No TwinCAT 3 link found.", paramName: nameof(rpcTwinCAT3Link)));

                // Invoke rpc method on plc
                var resultRpcMethod = await AdsSession.Connection.InvokeRpcMethodAsync(rpcTwinCAT3Link.SymbolKey.Path, rpcTwinCAT3Link.SymbolKey.Name, inputArguments.ToArray(), cancellationToken);
                if (!resultRpcMethod.Succeeded)
                {
                    modelRPCResponse.Result = ModelRPCResponseResultEnumeration.Failed;
                    modelRPCResponse.ReturnValue = null;
                    return modelRPCResponse;
                }

                // Check for affected model variable by the filter trigger property
                var baseRPCMethod = rpcMethod as ModelRPCBase;
                if (baseRPCMethod != null)
                {
                    var affectedVariables = await CheckFilterTrigger(baseRPCMethod);
                    if(affectedVariables.TC3Variables.Count > 0)
                    {
                        var pullVariableDataResult = await PullVariableData(affectedVariables.TC3Variables, cancellationToken);
                        if (pullVariableDataResult.Count == 0)
                        {
                            _Logger.LogError($"TwinCAT 3 variables with trigger for rpc [{rpcMethod.ModelLink.Key}] could not be pulled.");
                        }
                        else
                        {
                            SendPushModelVariablesCommand(pullVariableDataResult, cancellationToken);
                        }
                    }                  
                }

                modelRPCResponse.Result = ModelRPCResponseResultEnumeration.Success;
                modelRPCResponse.ReturnValue = resultRpcMethod.ReturnValue;
                return modelRPCResponse;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
            finally
            {
                _InvokeRpcSemaphore.Release();
            }
        }

        public override async Task<RPCResponse<T>> InvokeRpcMethod<T>(Guid rpcId, IList<object> inputArguments, CancellationToken cancellationToken)
        {
            try
            {
                await _InvokeRpcSemaphore.WaitAsync();
            }
            catch (Exception)
            {

                throw;
            }


            var exceptionText = $"Error on invoking generic rpc model component on model user [{Name}].";
            var rpcResponse = new RPCResponse<T>();
            rpcResponse.Result = ModelRPCResponseResultEnumeration.Failed;
            rpcResponse.ReturnValue = default;

            try
            {
                // Check ADS session
                if (!AdsSession.IsActive)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"TwinCAT 3 ADS session is not active.", paramName: nameof(AdsSession.IsActive)));

                // Get corresponding Rpc method
                var rpcMethod = FindRPCMethod(rpcId);
                if (rpcMethod == null)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"No corresponding rpc model component for id [{rpcId}] found.", paramName: nameof(FindRPCMethod)));

                // Updating exception text
                exceptionText = $"Error on invoking generic rpc model component [{rpcMethod.ModelLink.Key}] on model user [{Name}].";

                // Get corresponding TwinCAT3 link
                var rpcTwinCAT3Link = rpcMethod.TwinCAT3Links.FirstOrDefault(link => link.Name == Name);
                if (rpcTwinCAT3Link == null)
                    throw new InvalidOperationException(exceptionText,
                        new ArgumentException($"No TwinCAT 3 link found.", paramName: nameof(rpcTwinCAT3Link)));

                // Invoke rpc method on plc
                // ToDo!!! -> Setup correct return/output type with 'T' and Beckhoff 'AnyTypeSpecifier'
                var resultRpcMethod = await AdsSession.Connection.InvokeRpcMethodAsync(rpcTwinCAT3Link.SymbolKey.Path, rpcTwinCAT3Link.SymbolKey.Name, inputArguments.ToArray(), cancellationToken);
                if (!resultRpcMethod.Succeeded)
                {
                    rpcResponse.Result = ModelRPCResponseResultEnumeration.Failed;
                    rpcResponse.ReturnValue = default;
                    return rpcResponse;
                }

                rpcResponse.Result = ModelRPCResponseResultEnumeration.Success;
                rpcResponse.ReturnValue = rpcMethod.GetOutputArgumentContainer<T>(resultRpcMethod.ReturnValue); ;
                return rpcResponse;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(exceptionText, ex);
            }
            finally
            {
                _InvokeRpcSemaphore.Release();
            }
        }

        #endregion
    }

    internal class SingleSamplingData
    {
        #region ctor

        public SingleSamplingData()
        { }

        public SingleSamplingData(List<TC3Variable> tc3Variables)
        {
            TC3Variables = tc3Variables;
        }

        #endregion
        #region props

        public ModelVariableSamplingRateEnumeration SamplingTrigger { get; set; } = ModelVariableSamplingRateEnumeration.ms100;
        public int ProcessPeriodTimeCounter { get; set; } = 0;
        public List<TC3Variable> TC3Variables { get; set; } = new();

        #endregion
    }
}
