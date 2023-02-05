using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Test.Model.Traversal.TestModel
{
    public  class TraversalTestModel : ModelComponentContainer
    {
        [TwinCAT3SamplingRate("TraversalTestModel", ModelVariableSamplingRateEnumeration.ms50)]
        public TraversalTestModelServiceChild Service1 { get; set; } = new();

        [TwinCAT3SamplingRate("TraversalTestModel", ModelVariableSamplingRateEnumeration.ms1000)]
        public TraversalTestModelServiceChild Service2 { get; set; } = new();
    }

    public class TraversalTestModelServiceChild : ModelComponentContainer, IServiceModelContainer
    {
        public ModelVariable<ushort> PLCVersion { get; set; } = new ModelVariable<ushort>();
        public ModelVariable<bool> IsMuted { get; set; } = new ModelVariable<bool>();

        [TwinCAT3SamplingRate("TraversalTestModel", ModelVariableSamplingRateEnumeration.ms200)]
        public ModelComponentContainerCollection<TraversalTestModelChild> Childreens { get; set; } = new ModelComponentContainerCollection<TraversalTestModelChild>(5);

        public ModelVariable<bool> State { get; set; } = new ModelVariable<bool>();

        public TraversalTestModelServiceChild2 ChildService { get; set; } = new();

        bool IServiceModelContainer.IsEnabled { get; }
    }

    public class TraversalTestModelServiceChild2 : ModelComponentContainer, IServiceModelContainer
    {
        public ModelVariable<ushort> PLCVersion { get; set; } = new ModelVariable<ushort>();
        public ModelVariable<bool> IsMuted { get; set; } = new ModelVariable<bool>();

        public ModelComponentContainerCollection<TraversalTestModelChild> Childreens { get; set; } = new ModelComponentContainerCollection<TraversalTestModelChild>(5);

        public ModelVariable<bool> State { get; set; } = new ModelVariable<bool>();

        bool IServiceModelContainer.IsEnabled { get; }
    }



    public class TraversalTestModelChild :  ModelComponentContainer
    {
        public ModelVariable<bool> IsEnabled { get; set; } = new ModelVariable<bool>();
        public ModelVariable<ushort> PLCVersion { get; set; } = new ModelVariable<ushort>();
        public ModelVariable<bool> IsMuted { get; set; } = new ModelVariable<bool>();
        public ModelVariable<ushort> Test { get; set; } = new ModelVariable<ushort>();
        public ModelRPC<RPCEmptyInputArgument, RPCEmptyOutputArgument> TestRPC { get; } = new();
    }

}
