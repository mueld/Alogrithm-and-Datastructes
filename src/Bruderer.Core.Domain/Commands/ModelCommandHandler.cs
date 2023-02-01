using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Messaging;
using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;

namespace Bruderer.Core.Domain.Commands
{
    public abstract class ModelCommandHandler : CommandHandler
    {
        #region fields

        protected readonly IModelFactory _ModelFactory;

        #endregion
        #region ctor

        protected ModelCommandHandler(
            IModelFactory modelFactory)
        {
            _ModelFactory = modelFactory;
        }

        #endregion
        #region methods

        protected IResponse<IModel> GetModel(Guid modelId)
        {
            IResponse<IModel> response = new Response<IModel>();

            if (_ModelFactory == null)
            {
                // Setup log message
                var logMessage = $"Model factory not available in [{nameof(ModelCommandHandler)}].";

                // Setup localized response message
                var localizedMessage = new LocalizableValue();
                localizedMessage.Link.Name = "modelfactorynotavailable";
                localizedMessage.Link.Path = "commandhandlers.modelcommandhandler.getmodel";
                localizedMessage.KeyNamespace = CoreLocalizationNamespaceConstants.CoreDefault;
                localizedMessage.DynamicValueDictionary.Add("ModuleName", nameof(ModelCommandHandler));
                localizedMessage.Value = logMessage;

                // Setup response
                response.AddMessage(localizedMessage, nameof(GetModel));
                response.Result = ResponseResultEnumeration.Error;

                return response;
            }

            var model = _ModelFactory.GetModel(modelId);
            if (model == null)
            {
                // Setup log message
                var logMessage = $"Model with id [{modelId}] not available in [{nameof(ModelCommandHandler)}].";

                // Setup localized response message
                var localizedMessage = new LocalizableValue();
                localizedMessage.Link.Name = "modelnotavailable";
                localizedMessage.Link.Path = "commandhandlers.modelcommandhandler.getmodel";
                localizedMessage.KeyNamespace = CoreLocalizationNamespaceConstants.CoreDefault;
                localizedMessage.DynamicValueDictionary.Add("ModuleName", nameof(ModelCommandHandler));
                localizedMessage.DynamicValueDictionary.Add("ModelId", modelId.ToString());
                localizedMessage.Value = logMessage;

                // Setup response
                response.AddMessage(localizedMessage, nameof(GetModel));
                response.Result = ResponseResultEnumeration.Error;

                return response;
            }

            response.Payload = model;
            response.Result = ResponseResultEnumeration.Success;
            return response;
        }

        protected IResponse<T> GetModel<T>(Guid modelId)
            where T : IModel
        {
            IResponse<T> response = new Response<T>(default);
            var modelResponse = GetModel(modelId);
            if (modelResponse.Result != ResponseResultEnumeration.Success)
            {
                response.MessageStack.AddRange(modelResponse.MessageStack);
                response.Result = ResponseResultEnumeration.Error;
                return response;
            }

            if (!(modelResponse.Payload is T))
            {
                // Setup log message
                var logMessage = $"Model with id [{modelId}] not available in [{nameof(ModelCommandHandler)}].";

                // Setup localized response message
                var localizedMessage = new LocalizableValue();
                localizedMessage.Link.Name = "modelnotavailable";
                localizedMessage.Link.Path = "commandhandlers.modelcommandhandler.getmodel";
                localizedMessage.KeyNamespace = CoreLocalizationNamespaceConstants.CoreDefault;
                localizedMessage.DynamicValueDictionary.Add("ModuleName", nameof(ModelCommandHandler));
                localizedMessage.DynamicValueDictionary.Add("ModelId", modelId.ToString());
                localizedMessage.Value = logMessage;

                // Setup response
                response.AddMessage(localizedMessage, nameof(GetModel));
                response.Result = ResponseResultEnumeration.Error;

                return response;
            }

            response.Payload = (T)modelResponse.Payload;
            response.Result = ResponseResultEnumeration.Success;
            return response;
        }

        #endregion
    }
}
