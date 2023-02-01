using Bruderer.Core.Application.DTO;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Application.EventHandlers
{
    public abstract class ModelEventHandler
    {
        #region fields

        protected readonly ILoggerFactory _LoggerFactory;
        protected readonly IModelFactory _ModelFactory;
        private readonly ILogger<ModelEventHandler> _Logger;

        #endregion
        #region ctor

        protected ModelEventHandler(
            ILoggerFactory loggerFactory,
            IModelFactory modelFactory)
        {
            _LoggerFactory = loggerFactory;
            _Logger = loggerFactory.CreateLogger<ModelEventHandler>();
            _ModelFactory = modelFactory;
        }

        #endregion
        #region methods

        protected IModel GetModel(Guid modelId)
        {
            if (_ModelFactory == null)
            {
                _Logger.LogError($"Model factory not available in [{nameof(ModelEventHandler)}].");
                return default;
            }

            var model = _ModelFactory.GetModel(modelId);
            if (model == null)
            {
                _Logger.LogError($"Model with id [{modelId}] not available in [{nameof(ModelEventHandler)}].");
                return default;
            }

            return model;
        }

        protected IModelUser GetModelUser(Guid modelUserId)
        {

            if (_ModelFactory == null)
            {
                _Logger.LogError($"Model factory not available in [{nameof(ModelEventHandler)}].");
                return default;
            }

            var modelUser = _ModelFactory.GetModelUser(modelUserId);
            if (modelUser == null)
            {
                _Logger.LogError($"Model user with id [{modelUserId}] not available in [{nameof(ModelEventHandler)}].");
                return default;
            }

            return modelUser;
        }

        protected Dictionary<string, List<ModelVariableDTO>> GetServiceVariabeDTODictionary(List<IModelVariable> modelVariables)
        {
            var serviceVariableDTODictionary = new Dictionary<string, List<ModelVariableDTO>>();
            foreach (var modelVariable in modelVariables)
            {
                var serviceName = modelVariable.ServiceName;

                if (serviceVariableDTODictionary.ContainsKey(serviceName))
                {
                    var variableList = serviceVariableDTODictionary[serviceName];
                    variableList.Add(new ModelVariableDTO(modelVariable));
                }
                else
                {
                    var variableList = new List<ModelVariableDTO>();
                    variableList.Add(new ModelVariableDTO(modelVariable));

                    serviceVariableDTODictionary.Add(serviceName, variableList);
                }
            }

            return serviceVariableDTODictionary;
        }

        #endregion
    }
}
