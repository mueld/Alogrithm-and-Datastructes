using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelUserAggregate;
using System.Collections.Generic;

namespace Bruderer.Core.Application.DTO
{
    // OFI -> Check for auto mapping solutions as https://github.com/AutoMapper/AutoMapper
    public class ModelComponentContainerDTO : ModelComponentContainerBaseDTO
    {
        #region ctor

        public ModelComponentContainerDTO()
        { }

        public ModelComponentContainerDTO(IModelComponentContainer modelComponentContainer)
            : base(modelComponentContainer)
        {
            Take(modelComponentContainer);
        }

        #endregion
        #region props

        public List<ModelTwinCAT3Link> TwinCAT3Links { get; set; } = new();
        public List<ModelOPCUALink> OPCUALinks { get; set; } = new();
        public List<ModelComponentContainerDTO> ModelContainers { get; } = new List<ModelComponentContainerDTO>();
        public List<ModelVariableDTO> ModelVariables { get; } = new List<ModelVariableDTO>();

        #endregion
        #region methods

        private void Take(IModelComponentContainer modelComponentContainer)
        {
            TwinCAT3Links = new List<ModelTwinCAT3Link>(modelComponentContainer.TwinCAT3Links);
            OPCUALinks = new List<ModelOPCUALink>(modelComponentContainer.OPCUALinks);

            for (int i = 0; i < modelComponentContainer.ModelVariables.Count; i++)
                ModelVariables.Add(new ModelVariableDTO(modelComponentContainer.ModelVariables[i]));

            for (int i = 0; i < modelComponentContainer.ModelContainers.Count; i++)
                ModelContainers.Add(new ModelComponentContainerDTO(modelComponentContainer.ModelContainers[i]));
        }

        #endregion
    }
}
