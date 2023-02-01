using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.Collections.Generic;
using System.Linq;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public class ModelRepositoryData
    {
        #region ctor

        public ModelRepositoryData() { }

        public ModelRepositoryData(IRepositoryModelContainer sourceContainer)
        {
            Context = new(sourceContainer);
            ModelContainers = sourceContainer
                    .FindComponents<ModelComponentContainer>()
                    .Where(container => container.IsPersistent)
                    .Where(container => container.RepositoryTypeName == sourceContainer.RepositoryTypeName)
                    .ToList();
            ModelVariables = sourceContainer
                    .FindComponents<ModelVariableBase>()
                    .Where(variable => variable.IsPersistent)
                    .Where(container => container.RepositoryTypeName == sourceContainer.RepositoryTypeName)
                    .ToList();
        }

        #endregion
        #region props

        public ModelRepositoryDataContext Context { get; set; } = new();
        public List<ModelComponentContainer> ModelContainers { get; set; } = new();
        public List<ModelVariableBase> ModelVariables { get; set; } = new();

        #endregion
    }
}
