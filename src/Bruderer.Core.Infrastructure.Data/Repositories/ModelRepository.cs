using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bruderer.Core.Infrastructure.Data.Repositories
{
    public abstract class ModelRepository : IModelRepository
    {
        #region fields

        private readonly ILogger<ModelRepository> _Logger;

        #endregion
        #region ctor

        public ModelRepository(ILoggerFactory loggerFactory)
        {
            _Logger = loggerFactory.CreateLogger<ModelRepository>();
        }

        #endregion

        public abstract void Dispose();

        public abstract Task<List<T>> GetRepositoryInfos<T>()
            where T : ModelRepositoryInfo;

        public abstract Task<ModelRepositoryData> PullModelData<T>(string repositoryPartTypeName, Guid id)
             where T : IRepositoryModelContainer;
        public abstract Task<ModelRepositoryData> PullModelData<T>(T repositoryContainer, Guid id)
            where T : IRepositoryModelContainer;

        public abstract Task<bool> PushModelData<T>(ModelRepositoryData modelRepositoryData)
            where T : IRepositoryModelContainer;
        public abstract Task<bool> PushModelData<T>(T repositoryContainer)
            where T : IRepositoryModelContainer;

        public abstract Task<bool> RemoveModelData<T>(Guid id)
            where T : IRepositoryModelContainer;
    }
}
