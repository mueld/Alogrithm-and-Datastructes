using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.ComponentModel;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets.Components
{

    /// <summary>
    /// Variable count : 3
    /// </summary>
    public class PayloadAdaptorType : ModelComponentContainer
    {
        [DisplayName("MaxPayloadWeight")]
        [Description("MaxPayloadWeight")]
        [EngineeringUnit(UnitsEnumeration.Kilogram)]
        public ModelVariable<float> MaxPayloadWeight { get; set; } = new((float)0.0);

        [DisplayName("Payloads")]
        [Description("Payloads")]
        public virtual ModelComponentContainerCollection<PayloadType> Payloads { get; set; } = new(2);

        [DisplayName("ReleasePayload")]
        [Description("ReleasePayload")]
        public ModelRPC<ReleasePayloadInputArgument, ReleasePayloadOuputArgument> ReleasePayload { get; } = new();
    }

    public class ReleasePayloadInputArgument : ModelRPCInputArgumentContainer
    {
        [DisplayName("PayloadIndex")]
        [Description("PayloadIndex")]
        public ModelRPCInputArgument<ushort> PayloadIndex { get; set; } = new ModelRPCInputArgument<ushort>();

        [DisplayName("DelayTicks")]
        [Description("DelayTicks")]
        public ModelRPCInputArgument<long> DelayTicks { get; set; } = new ModelRPCInputArgument<long>();
    }

    public class ReleasePayloadOuputArgument : ModelRPCOutputArgumentContainer
    {
        [DisplayName("Result")]
        [Description("Result")]
        public ModelRPCInputArgument<int> Result { get; set; } = new ModelRPCInputArgument<int>();
    }
}
