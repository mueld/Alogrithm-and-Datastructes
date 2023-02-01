using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Test.Model.TestModel
{
    public class TestModelChildreen : ModelComponentContainer, IServiceModelContainer
    {
        [VisibilityCondtionsAttribute("TestCondition")]
        public ModelVariable<bool> IsEnabled { get; set; } = new ModelVariable<bool>();
        public ModelVariable<ushort> PLCVersion { get; set; } = new ModelVariable<ushort>();
        public ModelVariable<bool> IsMuted { get; set; } = new ModelVariable<bool>();
        public ModelVariable<ushort> Test { get; set; } = new ModelVariable<ushort>();
        public ModelRPC<RPCEmptyInputArgument, RPCEmptyOutputArgument> TestRPC { get; } = new();
        bool IServiceModelContainer.IsEnabled { get; }

        #region IServiceModelContainer
        public List<ModelVariableBase> GetAllActiveVariables()
        {
            return new();
        }
        #endregion
    }

}
