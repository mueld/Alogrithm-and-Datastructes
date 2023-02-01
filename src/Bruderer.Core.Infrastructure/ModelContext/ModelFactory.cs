using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bruderer.Core.Infrastructure.ModelContext
{
    public class ModelFactory : IModelFactory
    {
        #region fields

        private readonly ILoggerFactory _LoggerFactory;
        private readonly ILogger<ModelFactory> _Logger;
        private readonly IConfiguration _Configuration;
        private readonly IServiceScopeFactory _ServiceScopeFactory;
        private readonly IServiceProvider _ServiceProvider;
        private readonly List<ModelContext> _ModelContextList = new List<ModelContext>();
        private readonly List<ModelUserContext> _ModelUserContextList = new List<ModelUserContext>();

        #endregion
        #region ctor

        public ModelFactory(
            IServiceProvider serviceProvider,
            IServiceScopeFactory serviceScopeFactory,
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            _ServiceProvider = serviceProvider;
            _ServiceScopeFactory = serviceScopeFactory;
            _LoggerFactory = loggerFactory;
            _Logger = loggerFactory.CreateLogger<ModelFactory>();
            _Logger.LogInformation("Initialize ModelFactory.");
            _Configuration = configuration;
        }

        #endregion
        #region methods

        private bool CreateModelDependency(ModelContext modelContext, ModelUserContext modelUserContext, ModelDependencyTypeEnumeration dependencyType)
        {

            switch (dependencyType)
            {
                case ModelDependencyTypeEnumeration.Provider:
                    {

                        var result = modelContext.AddProvider(modelUserContext.ModelUser);
                        if (!result)
                        {
                            _Logger.LogError($"Setting model user [{modelUserContext.ModelUser.Name}] as provider for model [{modelContext.Model.Name}] failed. Dependency creation aborted.");
                            return false;
                        }

                        result = modelUserContext.AddProvidedModel(modelContext.Model);
                        if (!result)
                        {
                            _Logger.LogError($"Setting model [{modelContext.Model.Name}] as provided element for model user [{modelUserContext.ModelUser.Name}] failed. Dependency creation aborted.");
                            return false;
                        }

                        break;
                    }
                case ModelDependencyTypeEnumeration.Consumer:
                    {
                        var result = modelContext.AddConsumer(modelUserContext.ModelUser);
                        if (!result)
                        {
                            _Logger.LogError($"Setting model user [{modelUserContext.ModelUser.Name}] as consumer for model [{modelContext.Model.Name}] failed. Dependency creation aborted.");
                            return false;
                        }

                        result = modelUserContext.AddConsumedModel(modelContext.Model);
                        if (!result)
                        {
                            _Logger.LogError($"Setting model [{modelContext.Model.Name}] as provided element for model user [{modelUserContext.ModelUser.Name}] failed. Dependency creation aborted.");
                            return false;
                        }

                        break;
                    }
                default:
                    {
                        _Logger.LogError($"Dependency creation aborted. No dependecy type available.");
                        return false;
                    }
            }

            _Logger.LogInformation($"Dependency between [{modelContext.Model.Name}, {modelUserContext.ModelUser.Name}] created.");
            return true;
        }

        #endregion

        #region IModelFactory

        public bool AddModel<TInterface, TClass>()
            where TInterface : IModel
            where TClass : Model, TInterface
        {
            var isModelExisting = _ModelContextList.Any(context => context.Model is TInterface);
            if (isModelExisting)
            {
                _Logger.LogError($"Model of type [{typeof(TInterface)}] already exists.");
                return false;
            }

            var model = Activator.CreateInstance(typeof(TClass), typeof(TClass).Name, _ServiceScopeFactory, this, _LoggerFactory) as TClass;
            _ModelContextList.Add(new ModelContext(model, _LoggerFactory));

            return true;
        }

        public T GetModel<T>()
            where T : IModel
        {
            var isModelExisting = _ModelContextList.Any(context => context.Model is T);
            if (!isModelExisting)
            {
                _Logger.LogError($"Model of type [{typeof(T)}] not found.");
                return default;
            }

            var modelContext = _ModelContextList.Find(context => context.Model is T);
            return (T)modelContext.Model;
        }

        public IModel GetModel(Guid modelId)
        {
            var isModelExisting = _ModelContextList.Any(context => context.Model.Id == modelId);
            if (!isModelExisting)
            {
                _Logger.LogError($"Model with id [{modelId}] not found.");
                return default;
            }

            var modelContext = _ModelContextList.Find(context => context.Model.Id == modelId);
            return modelContext.Model;
        }

        public IModelUser GetModelProvider(Guid modelId)
        {
            // Get the model context
            var modelContext = _ModelContextList.Find(context => context.Model.Id == modelId);
            if (modelContext == null)
            {
                _Logger.LogError($"Model context for model id [{modelId}] not available in model factory.");
                return default;
            }

            return modelContext.Provider;
        }

        public IEnumerable<IModelUser> GetModelConsumers(Guid modelId)
        {
            // Get the model context
            var modelContext = _ModelContextList.Find(context => context.Model.Id == modelId);
            if (modelContext == null)
            {
                _Logger.LogError($"Model context for model id [{modelId}] not available in model factory.");
                return new List<IModelUser>();
            }

            return modelContext.Consumers;
        }

        public IEnumerable<IModelUser> GetModelUsers(Guid modelId)
        {
            // Get the model context
            var modelContext = _ModelContextList.Find(context => context.Model.Id == modelId);
            if (modelContext == null)
            {
                _Logger.LogError($"Model user context for model user id [{modelId}] not available in model factory.");
                return default;
            }

            var modelUserList = new List<IModelUser>();
            modelUserList.Add(modelContext.Provider);
            return modelUserList.Concat(modelContext.Consumers);
        }


        public bool AddModelUser<TInterface, TClass>()
            where TInterface : IModelUser
            where TClass : ModelUser, TInterface
        {
            var isModelUserExisting = _ModelUserContextList.Any(context => context.ModelUser is TInterface);
            if (isModelUserExisting)
            {
                _Logger.LogError($"Model user of type [{typeof(TInterface)}] already exists.");
                return false;
            }

            var modelUser = Activator.CreateInstance(typeof(TClass), typeof(TClass).Name, _ServiceScopeFactory, this, _LoggerFactory, _Configuration) as TClass;
            _ModelUserContextList.Add(new ModelUserContext(modelUser, _LoggerFactory));

            return true;
        }

        public T GetModelUser<T>()
            where T : IModelUser
        {
            var isModelUserExisting = _ModelUserContextList.Any(context => context.ModelUser is T);
            if (!isModelUserExisting)
            {
                _Logger.LogError($"Model user of type [{typeof(T)}] not found.");
                return default;
            }

            var modelUserContext = _ModelUserContextList.Find(context => context.ModelUser is T);
            return (T)modelUserContext.ModelUser;
        }

        public IModelUser GetModelUser(Guid modelUserId)
        {
            var isModelUserExisting = _ModelUserContextList.Any(context => context.ModelUser.Id == modelUserId);
            if (!isModelUserExisting)
            {
                _Logger.LogError($"Model user with id [{modelUserId}] not found.");
                return default;
            }

            var modelUserContext = _ModelUserContextList.Find(context => context.ModelUser.Id == modelUserId);
            return modelUserContext.ModelUser;
        }

        public IEnumerable<IModel> GetProvidedModels(Guid modelUserId)
        {
            // Get the model user context
            var modelUserContext = _ModelUserContextList.Find(context => context.ModelUser.Id == modelUserId);
            if (modelUserContext == null)
            {
                _Logger.LogError($"Model user context for model user id [{modelUserId}] not available in model factory.");
                return default;
            }

            return modelUserContext.ProvidedModels;
        }

        public IEnumerable<IModel> GetConsumedModels(Guid modelUserId)
        {
            // Get the model user context
            var modelUserContext = _ModelUserContextList.Find(context => context.ModelUser.Id == modelUserId);
            if (modelUserContext == null)
            {
                _Logger.LogError($"Model user context for model user id [{modelUserId}] not available in model factory.");
                return default;
            }

            return modelUserContext.ConsumedModels;
        }

        public IEnumerable<IModel> GetUsedModels(Guid modelUserId)
        {
            // Get the model user context
            var modelUserContext = _ModelUserContextList.Find(context => context.ModelUser.Id == modelUserId);
            if (modelUserContext == null)
            {
                _Logger.LogError($"Model user context for model user id [{modelUserId}] not available in model factory.");
                return default;
            }

            return new List<IModel>(modelUserContext.ProvidedModels).Concat(modelUserContext.ConsumedModels);
        }


        public bool CreateModelDependency<TModel, TModelUser>(ModelDependencyTypeEnumeration dependencyType)
            where TModel : IModel
            where TModelUser : IModelUser
        {
            var modelContext = _ModelContextList.Find(context => context.Model is TModel);
            if (modelContext == null)
            {
                _Logger.LogError($"Model context for type [{typeof(TModel)}] not available in model factory. Dependency to model user type [{typeof(TModelUser)}] can not be created.");
                return false;
            }

            var modelUserContext = _ModelUserContextList.Find(context => context.ModelUser is TModelUser);
            if (modelUserContext == null)
            {
                _Logger.LogError($"Model user context for type [{typeof(TModelUser)}] not available in model factory. Dependency to mode type [{typeof(TModel)}] can not be created.");
                return false;
            }

            return CreateModelDependency(modelContext, modelUserContext, dependencyType);
        }

        public bool CreateModelDependency<TModel>(IModelUser modelUser, ModelDependencyTypeEnumeration dependencyType)
            where TModel : IModel
        {
            var modelContext = _ModelContextList.Find(context => context.Model is TModel);
            if (modelContext == null)
            {
                _Logger.LogError($"Model context for type [{typeof(TModel)}] not available in model factory. Dependency to model user [{modelUser.Name}] can not be created.");
                return false;
            }

            var modelUserContext = _ModelUserContextList.Find(context => context.ModelUser.Id == modelUser.Id);
            if (modelUserContext == null)
            {
                _Logger.LogError($"Model user context for [{modelUser.Name}] not available in model factory. Dependency to mode type [{typeof(TModel)}] can not be created.");
                return false;
            }

            return CreateModelDependency(modelContext, modelUserContext, dependencyType);
        }

        #endregion
    }
}
