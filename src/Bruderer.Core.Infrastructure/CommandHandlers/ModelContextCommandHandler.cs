using Bruderer.Core.Domain.Commands;
using Bruderer.Core.Domain.Events;
using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Response;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Infrastructure.CommandHandlers
{
    public class ModelContextCommandHandler : ModelCommandHandler,
        IRequestHandler<ModelVariablesValueChangeCommand, IResponse<bool>>,
        IRequestHandler<ModelVariablesMetadataChangeCommand, IResponse<bool>>,
        IRequestHandler<ModelStructureChangeCommand, IResponse<bool>>,
        IRequestHandler<ModelRpcCommand, IResponse<ModelRPCResponse>>
    {
        #region fields

        private readonly ILogger<ModelContextCommandHandler> _Logger;
        private readonly IMessageBus _MessageBus;
        #endregion
        #region ctor

        public ModelContextCommandHandler(
            IModelFactory modelFactory,
            ILoggerFactory loggerFactory,
            IMessageBus messageBus)
            : base(modelFactory)
        {
            _MessageBus = messageBus;
            _Logger = loggerFactory.CreateLogger<ModelContextCommandHandler>();
        }

        #endregion

        #region IRequestHandlers

        public async Task<IResponse<bool>> Handle(ModelVariablesValueChangeCommand command, CancellationToken cancellationToken)
        {
            if (!command.IsValid())
                return command.ValidationResult;

            IResponse<bool> response = new Response<bool>(false);

            try
            {
                var modelResponse = GetModel(command.ModelId);
                if (modelResponse == null || modelResponse.Result == ResponseResultEnumeration.Error)
                {
                    response.MessageStack.AddRange(modelResponse.MessageStack);
                    response.Result = ResponseResultEnumeration.Error;
                    return response;
                }

                var model = modelResponse.Payload;
                var modelProvider = _ModelFactory.GetModelProvider(model.Id);
                var modelConumsers = _ModelFactory.GetModelConsumers(model.Id);

                // Update model provider first
                // Ignore push if command context origin information is equal with this model provider
                if (modelProvider != null &&
                    modelProvider.Id != command.CallerContext.Id &&
                    !command.IgnoredContextIds.Contains(modelProvider.Id))
                {
                    if (modelProvider.State != ModelUserStateEnumeration.Ok)
                    {
                        var logMessage = $"Pushing model variable value data to model user [{modelProvider.Name}] ignored. Model user is in an inappropriate state.";
                        _Logger.LogWarning(logMessage);
                        response.LogWarning(logMessage);
                    }
                    else
                    {
                        var pushResult = await modelProvider.PushVariableData(command.ModelVariableDEOs, cancellationToken);
                        if (!pushResult)
                        {
                            response.LogError($"Pushing model variable value data to model user [{modelProvider.Name}] failed.");
                            response.Result = ResponseResultEnumeration.Error;
                            response.Payload = false;
                            return response;
                        }
                    }
                }

                // Update the model itself
                // Ignore push if command context origin information is equal with this model
                if (model.Id != command.CallerContext.Id &&
                    !command.IgnoredContextIds.Contains(model.Id))
                {
                    if (model.State != ModelStateEnumeration.Ok)
                    {
                        var logMessage = $"Pushing model variable value data to model [{model.Name}] ignored. Model is in an inappropriate state.";
                        _Logger.LogWarning(logMessage);
                        response.LogWarning(logMessage);
                    }
                    else
                    {
                        var pushResult = await model.PushVariableData(command.ModelVariableDEOs, cancellationToken);
                        if (!pushResult)
                        {
                            response.LogError($"Pushing model variable value data to model [{model.Name}] failed.");
                            response.Result = ResponseResultEnumeration.Error;
                            response.Payload = false;
                            return response;
                        }
                    }
                }

                // Update model all modelconsumers
                foreach (var modelConsumer in modelConumsers)
                {
                    // Ignore push if command context origin information is equal with this model consumer
                    if (modelConsumer.Id == command.CallerContext.Id ||
                        command.IgnoredContextIds.Contains(modelConsumer.Id))
                        continue;

                    if (modelConsumer.State != ModelUserStateEnumeration.Ok)
                    {
                        var logMessage = $"Pushing model variable value data to model user [{modelConsumer.Name}] ignored. Model user is in an inappropriate state.";
                        _Logger.LogWarning(logMessage);
                        response.LogWarning(logMessage);
                    }
                    else
                    {
                        var pushResult = await modelConsumer.PushVariableData(command.ModelVariableDEOs, cancellationToken);
                        if (!pushResult)
                        {
                            response.LogError($"Pushing model variable value data to model user [{modelConsumer.Name}] failed.");
                            response.Result = ResponseResultEnumeration.Error;
                            response.Payload = false;
                            return response;
                        }
                    }
                }

                // Setup and dispatch the event
                var commandVariableIDs = command.ModelVariableDEOs
                    .Select(deo => deo.ModelContext.Id)
                    .ToList();
                var modelVariablesPushedEvent = new ModelVariablesValueChangedEvent(model.Id, commandVariableIDs);
                modelVariablesPushedEvent.ModelName = model.Name;
                modelVariablesPushedEvent.CallerContext = command.CallerContext;

                await _MessageBus.PublishEvent(modelVariablesPushedEvent);

                response.Payload = true;
                response.Result = ResponseResultEnumeration.Success;
                return response;
            }
            catch (Exception ex)
            {
                response.AddMessage(ex);
                response.Payload = false;
                response.Result = ResponseResultEnumeration.Error;
                return response;
            }
        }

        public async Task<IResponse<bool>> Handle(ModelVariablesMetadataChangeCommand command, CancellationToken cancellationToken)
        {
            if (!command.IsValid())
                return command.ValidationResult;

            IResponse<bool> response = new Response<bool>(false);

            try
            {
                var modelResponse = GetModel(command.ModelId);
                if (modelResponse == null || modelResponse.Result == ResponseResultEnumeration.Error)
                {
                    response.MessageStack.AddRange(modelResponse.MessageStack);
                    response.Result = ResponseResultEnumeration.Error;
                    return response;
                }

                var model = modelResponse.Payload;

                // Setup and dispatch the event
                var commandVariableIDs = command.ModelVariableDEOs
                    .Select(deo => deo.ModelContext.Id)
                    .ToList();

                var modelVariablesPushedEvent = new ModelVariablesMetadataChangedEvent(model.Id, commandVariableIDs);
                modelVariablesPushedEvent.ModelName = model.Name;
                modelVariablesPushedEvent.CallerContext = command.CallerContext;

                await _MessageBus.PublishEvent(modelVariablesPushedEvent);

                response.Payload = true;
                response.Result = ResponseResultEnumeration.Success;
                return response;
            }
            catch (Exception ex)
            {
                response.AddMessage(ex);
                response.Payload = false;
                response.Result = ResponseResultEnumeration.Error;
                return response;
            }
        }

        public async Task<IResponse<bool>> Handle(ModelStructureChangeCommand command, CancellationToken cancellationToken)
        {
            if (!command.IsValid())
                return command.ValidationResult;

            IResponse<bool> response = new Response<bool>(false);

            try
            {
                var modelResponse = GetModel(command.ModelId);
                if (modelResponse == null || modelResponse.Result == ResponseResultEnumeration.Error)
                {
                    response.MessageStack.AddRange(modelResponse.MessageStack);
                    response.Result = ResponseResultEnumeration.Error;
                    return response;
                }

                var model = modelResponse.Payload;
                var modelProvider = _ModelFactory.GetModelProvider(model.Id);
                var modelConumsers = _ModelFactory.GetModelConsumers(model.Id);

                // Update the model itself
                if (model.Id != command.CallerContext.Id)
                {
                    if (model.State != ModelStateEnumeration.Ok)
                    {
                        var logMessage = $"Change structure data on model [{model.Name}] ignored. Model is in an inappropriate state.";
                        _Logger.LogWarning(logMessage);
                        response.LogWarning(logMessage);
                    }
                    else
                    {
                        var consumeModelDataResult = await model.ConsumeModelData(command.ModelUpdateOptions, cancellationToken);
                        if (!consumeModelDataResult)
                        {
                            response.LogError($"Consuming model data to change structure on model [{model.Name}] failed.");
                            response.Result = ResponseResultEnumeration.Error;
                            response.Payload = false;
                            return response;
                        }
                    }
                }

                // Update model provider
                if (modelProvider != null &&
                    modelProvider.Id != command.CallerContext.Id)
                {
                    if (modelProvider.State != ModelUserStateEnumeration.Ok)
                    {
                        var logMessage = $"Change structure data on model model user [{modelProvider.Name}] ignored. Model user is in an inappropriate state.";
                        _Logger.LogWarning(logMessage);
                        response.LogWarning(logMessage);
                    }
                    else
                    {
                        var consumeModelDataResult = await modelProvider.ConsumeModelData(command.ModelUpdateOptions, cancellationToken);
                        if (!consumeModelDataResult)
                        {
                            response.LogError($"Consuming model data to change structure on model user [{modelProvider.Name}] failed.");
                            response.Result = ResponseResultEnumeration.Error;
                            return response;
                        }
                    }             
                }

                // Update model all modelconsumers
                foreach (var modelConsumer in modelConumsers)
                {
                    if (modelConsumer.Id != command.CallerContext.Id)
                        continue;

                    if (modelConsumer.State != ModelUserStateEnumeration.Ok)
                    {
                        var logMessage = $"Change structure data on model model user [{modelConsumer.Name}] ignored. Model user is in an inappropriate state.";
                        _Logger.LogWarning(logMessage);
                        response.LogWarning(logMessage);
                    }
                    else
                    {
                        var consumeModelDataResult = await modelConsumer.ConsumeModelData(command.ModelUpdateOptions, cancellationToken);
                        if (consumeModelDataResult)
                        {
                            response.LogError($"Consuming model data on model user [{modelConsumer.Name}] failed.");
                            response.Result = ResponseResultEnumeration.Error;
                            return response;
                        }
                    }
                }

                // Setup affected service parts of the model
                var affectedServiceNames = new List<string>(command.ModelUpdateOptions.OptionServiceNames);

                var modelStructureUpdatedEvent = new ModelStructureChangedEvent(model.Id, affectedServiceNames);
                modelStructureUpdatedEvent.ModelName = model.Name;
                modelStructureUpdatedEvent.CallerContext = command.CallerContext;

                await _MessageBus.PublishEvent(modelStructureUpdatedEvent);

                response.Payload = true;
                response.Result = ResponseResultEnumeration.Success;
                return response;
            }
            catch (Exception ex)
            {
                response.AddMessage(ex);
                response.Payload = false;
                response.Result = ResponseResultEnumeration.Error;
                return response;
            }
        }

        public async Task<IResponse<ModelRPCResponse>> Handle(ModelRpcCommand command, CancellationToken cancellationToken)
        {
            if (!command.IsValid())
                return command.ValidationResult;

            IResponse<ModelRPCResponse> response = new Response<ModelRPCResponse>(default);

            try
            {
                var modelResponse = GetModel(command.ModelId);
                if (modelResponse == null)
                {
                    response.MessageStack.AddRange(modelResponse.MessageStack);
                    response.Result = ResponseResultEnumeration.Error;
                    return response;
                }

                var model = modelResponse.Payload;
                var modelProvider = _ModelFactory.GetModelProvider(model.Id);
                //var modelConsumers = _ModelFactory.GetModelConsumers(model.Id);

                // We dispatch the rpc command only to the model provider
                if (modelProvider != null &&
                    modelProvider.Id != command.CallerContext.Id)
                {
                    response.Payload = await modelProvider.InvokeRpcMethod(command.RpcId, command.InputArguments, cancellationToken);
                }

                response.Result = ResponseResultEnumeration.Success;
                return response;
            }
            catch (Exception ex)
            {
                response.AddMessage(ex);
                response.Payload = default;
                response.Result = ResponseResultEnumeration.Error;           
                return response;
            }
        }

        #endregion
    }
}
