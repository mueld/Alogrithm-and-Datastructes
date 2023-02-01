using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using System.Collections.Generic;

namespace Bruderer.Core.Application.DTO
{
    // OFI -> Check for auto mapping solutions as https://github.com/AutoMapper/AutoMapper
    public class ModelStructureContainerDTO : ModelComponentContainerBaseDTO
    {
        #region ctor

        public ModelStructureContainerDTO()
        { }

        public ModelStructureContainerDTO(IModelComponentContainer modelComponentContainer)
            : base(modelComponentContainer)
        {
            Take(modelComponentContainer);
        }

        #endregion
        #region props
        
        public List<ModelStructureContainerDTO> ModelStructureContainers { get; } = new();
        public List<string> ModelVariableKeys { get; } = new();

        #endregion
        #region methods

        private void Take(IModelComponentContainer modelComponentContainer)
        {

            for (int i = 0; i < modelComponentContainer.ModelVariables.Count; i++)
                ModelVariableKeys.Add(modelComponentContainer.ModelVariables[i].ModelLink.Key);

            for (int i = 0; i < modelComponentContainer.ModelContainers.Count; i++)
                ModelStructureContainers.Add(new ModelStructureContainerDTO(modelComponentContainer.ModelContainers[i]));
        }

        #endregion
    }
}
