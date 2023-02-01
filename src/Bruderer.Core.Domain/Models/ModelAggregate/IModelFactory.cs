using Bruderer.Core.Domain.Models.ModelUserAggregate;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public interface IModelFactory
    {
        bool AddModel<TInterface, TClass>()
            where TInterface : IModel
            where TClass : Model, TInterface;
        T GetModel<T>() where T : IModel;
        IModel GetModel(Guid modelId);
        IModelUser GetModelProvider(Guid modelId);
        IEnumerable<IModelUser> GetModelConsumers(Guid modelId);
        IEnumerable<IModelUser> GetModelUsers(Guid modelId);

        bool AddModelUser<TInterface, TClass>()
            where TInterface : IModelUser
            where TClass : ModelUser, TInterface;
        T GetModelUser<T>() where T : IModelUser;
        IModelUser GetModelUser(Guid modelUserId);
        IEnumerable<IModel> GetProvidedModels(Guid modelUserId);
        IEnumerable<IModel> GetConsumedModels(Guid modelUserId);
        IEnumerable<IModel> GetUsedModels(Guid modelUserId);

        bool CreateModelDependency<TModel, TModelUser>(ModelDependencyTypeEnumeration dependencyType) where TModel : IModel where TModelUser : IModelUser;
        bool CreateModelDependency<TModel>(IModelUser modelUser, ModelDependencyTypeEnumeration dependencyType) where TModel : IModel;
    }
}
